using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            var libro = await context.Libros.Include(li => li.Comentarios).FirstOrDefaultAsync(li => li.Id == id);

            return mapper.Map<LibroDTO>(libro);
        }

        [HttpGet("TodosLosLibros")]
        public async Task<List<LibroDTO>> GetAll()
        {
            var libros = await context.Libros.Include(li => li.Comentarios).ToListAsync();
            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpPost]
        public async Task<ActionResult<Libro>> Post(LibroCreacionDTO libroCreacionDTO)
        {
            //var ExisteAutor = await context.Autores.AnyAsync(a => a.Id == libro.AutorId);

            //if (!ExisteAutor)
            //{
            //    return BadRequest($"El autor ingresado no existe. Id ingresado: {libro.AutorId}");
            //}

            var libro = mapper.Map<Libro>(libroCreacionDTO);

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
