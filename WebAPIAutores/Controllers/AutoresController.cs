using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entidades;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton, ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        [ResponseCache(Duration = 10)]
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                ServicioA_Transient = servicio.ObtenerTransient(),
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton(),
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),
                AutoresController_Scoped = servicioScoped.Guid
            });
        }

        [HttpGet("TodosLosAutores")]
        [Authorize]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            logger.LogInformation("Se mostrara listado de todos los autores...");
            logger.LogWarning("test warning...");
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if (existeAutor)
            {
                return BadRequest($"Ya existe un Autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id) {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la url");
            }

            var AutorExiste = await context.Autores.AnyAsync(a => a.Id == id);
            if (!AutorExiste)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id) {
            var AutorExiste = await context.Autores.AnyAsync(a => a.Id == id);
            if (!AutorExiste)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
