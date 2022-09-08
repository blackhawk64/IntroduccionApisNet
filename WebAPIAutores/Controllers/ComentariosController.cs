using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(lb => lb.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ComentarioPorId", new {id = comentario.Id, libroId = libroId}, comentarioDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(lb => lb.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(c => c.LibroId == libroId).ToListAsync();
            return Ok(mapper.Map<List<ComentarioDTO>>(comentarios));
        }

        [HttpGet("{id:int}", Name = "ComentarioPorId")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }
    }
}
