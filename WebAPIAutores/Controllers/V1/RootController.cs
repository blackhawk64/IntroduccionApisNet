using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();
            var esAdmin = await authorizationService.AuthorizeAsync(User, "EsAdmin");

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("TodosLosAutores", new { }), descripcion: "Mostrar todos los autores", metodo: "GET"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("CrearAutor", new { }), descripcion: "Crear un nuevo autor", metodo: "POST"));
            }

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("CrearLibro", new { }), descripcion: "Crear un nuevo libro", metodo: "POST"));


            return datosHateoas;
        }
    }
}
