﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Filtros;
using WebAPIAutores.Utilidades;

namespace WebAPIAutores.Controllers.V2
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraPresente("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("TodosLosAutores", Name = "TodosLosAutoresV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autores);
            //return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpPost(Name = "CrearAutorV2")]
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

        [HttpGet("AutorPorId/{id:int}", Name = "AutorPorIdV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> GetPorId(int id)
        {
            var autor = await context.Autores
                .Include(al => al.AutoresLibros)
                .ThenInclude(lb => lb.Libro)
                .FirstOrDefaultAsync(a => a.Id == id);

            var DTO = mapper.Map<AutorDTOConLibros>(autor);

            return autor == null ? NotFound() : DTO;
        }

        [HttpGet("PorNombreAutor/{nombre}", Name = "PorNombreAutorV2")]
        public async Task<List<AutorDTO>> GetPorNombre(string nombre)
        {
            var autores = await context.Autores.Where(a => a.Nombre.Contains(nombre)).ToListAsync(); ;

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPut("{id:int}", Name = "ActualizarAutorV2")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {

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

        [HttpDelete("{id:int}", Name = "EliminarAutorV2")]
        //[Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            var AutorExiste = await context.Autores.AnyAsync(a => a.Id == id);
            if (!AutorExiste)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
