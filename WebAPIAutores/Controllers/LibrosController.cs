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

        [HttpGet("{id:int}", Name = "LibroPorId")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            //var libro = await context.Libros.Include(li => li.Comentarios).FirstOrDefaultAsync(li => li.Id == id);
            var libro = await context.Libros
                .Include(al => al.AutoresLibros)
                .ThenInclude(a => a.Autor)
                .FirstOrDefaultAsync(li => li.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(al => al.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
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
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autores = await context.Autores.Where(autor => libroCreacionDTO.AutoresIds.Contains(autor.Id))
                                .Select(a => a.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autores.Count)
            {
                return BadRequest("Uno o mas autores ingresados no existe");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            if (libro.AutoresLibros != null)
            {
                int i = 1;
                foreach (var autor in libro.AutoresLibros)
                {
                    autor.Orden = i;
                    i++;
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("LibroPorId", new {id = libro.Id}, libroDTO);
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
