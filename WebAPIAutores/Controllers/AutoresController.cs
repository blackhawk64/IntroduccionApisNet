using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("TodosLosAutores")]
        public async Task<ActionResult<List<Autor>>> Get()
        {
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
