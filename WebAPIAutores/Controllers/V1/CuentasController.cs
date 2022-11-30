using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager
            , IConfiguration configuration
            , SignInManager<IdentityUser> signInManager
            , IDataProtectionProvider dataProtectionProvider
            , HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("Poj9Sp$dR");
        }

        [HttpGet("encriptar", Name = "EncriptarAlgo")]
        public ActionResult Encriptar()
        {
            var textoPlano = "AlexisAlcan";
            var textoCifrado = dataProtector.Protect(textoPlano);

            var textoDescifrado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDescifrado
            });
        }

        [HttpGet("encriptarPorTiempo", Name = "EncriptarPorTiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            var protectorLimitado = dataProtector.ToTimeLimitedDataProtector();

            var textoPlano = "AlexisAlcan";
            var textoCifrado = protectorLimitado.Protect(textoPlano, TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textoDescifrado = protectorLimitado.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDescifrado
            });
        }

        [HttpGet("hash/{textoPlano}", Name = "HashTextoPlano")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultadoUno = hashService.Hash(textoPlano);
            var resultadoDos = hashService.Hash(textoPlano);

            return Ok(new
            {
                textoPlano,
                Hash1 = resultadoUno,
                Hash2 = resultadoDos
            });
        }


        [HttpPost("registrar", Name = "RegistrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "LoginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password
                    , isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("No se pudo completar el Login");
            }
        }

        [HttpGet("RenovarToken", Name = "RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault().Value;
            var credencialesUsuario = new CredencialesUsuario
            {
                Email = emailClaim
            };

            return await ConstruirToken(credencialesUsuario);
        }

        [HttpPost("HacerAdmin", Name = "HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("EsAdministrador", "1"));

            return NoContent();
        }

        [HttpPost("RemoverAdmin", Name = "RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdministrador", "1"));

            return NoContent();
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>(){
                new Claim("email", credencialesUsuario.Email),
                new Claim("testValue", "this a test value for claims")
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDb = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDb);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddMonths(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims
                , expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }
    }
}
