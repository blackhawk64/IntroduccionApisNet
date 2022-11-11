using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Filtros;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("TodosLosAutores")]
        [AllowAnonymous]
        public async Task<List<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
            //return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorDto)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorDto.Nombre);
            if (existeAutor)
            {
                return BadRequest($"Ya existe un Autor con el nombre {autorDto.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorDto);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("AutorPorId", new { id = autor.Id }, autorDTO);
        }

        [HttpGet("AutorPorId/{id:int}", Name = "AutorPorId")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(al => al.AutoresLibros)
                .ThenInclude(lb => lb.Libro)
                .FirstOrDefaultAsync(a => a.Id == id);

            return autor == null ? NotFound() : mapper.Map<AutorDTOConLibros>(autor);
        }

        [HttpGet("PorNombreAutor/{nombre}")]
        public async Task<List<AutorDTO>> Get(string nombre)
        {
            var autores = await context.Autores.Where(a => a.Nombre.Contains(nombre)).ToListAsync();;

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id) {

            var AutorExiste = await context.Autores.AnyAsync(a => a.Id == id);
            if (!AutorExiste)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
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
            return NoContent();
        }
    }
}
