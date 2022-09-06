using AutoMapper;
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
            return Ok();
        }

        [HttpGet("AutorPorId/{id:int}")]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(a => a.Id == id);

            return autor == null ? NotFound() : mapper.Map<AutorDTO>(autor);
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
