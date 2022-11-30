using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(al => al.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpGet("TodosLosLibros", Name = "TodosLosLibros")]
        public async Task<List<LibroDTO>> GetAll()
        {
            var libros = await context.Libros.Include(li => li.Comentarios).ToListAsync();
            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpPost(Name = "CrearLibro")]
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

            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("LibroPorId", new {id = libro.Id}, libroDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDb = await context.Libros
                                .Include(l => l.AutoresLibros)
                                .FirstOrDefaultAsync(x => x.Id == id);
            if (libroDb == null)
            {
                return NotFound();
            }

            libroDb = mapper.Map(libroCreacionDTO, libroDb);

            AsignarOrdenAutores(libroDb);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "BorrarLibro")]
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
        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                int i = 1;
                foreach (var autor in libro.AutoresLibros)
                {
                    autor.Orden = i;
                    i++;
                }
            }
        }

        [HttpPatch("{id:int}", Name = "PatchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDb = await context.Libros.FirstOrDefaultAsync(l => l.Id == id);

            if (libroDb == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDb);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDb);

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
