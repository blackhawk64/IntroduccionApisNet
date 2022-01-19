using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            return await context.Libros.Include(li => li.Autor).FirstOrDefaultAsync(li => li.Id == id);
        }

        [HttpGet("TodosLosLibros")]
        public async Task<List<Libro>> GetAll()
        {
            return await context.Libros.Include(a => a.Autor).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Libro>> Post(Libro libro)
        {
            var ExisteAutor = await context.Autores.AnyAsync(a => a.Id == libro.AutorId);

            if (!ExisteAutor)
            {
                return BadRequest($"El autor ingresado no existe. Id ingresado: {libro.AutorId}");
            }

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var LibroExiste = await context.Libros.AnyAsync(a => a.Id == id);
            if (!LibroExiste)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
