using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute: HATEOASFilterAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var debeIncluir = IncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            //var modelo = resultado.Value as AutorDTO ??
            //    throw new ArgumentException("Se esperaba instancia de AutorDTO");
            var autorDTO = resultado.Value as AutorDTO;
            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ??
                    throw new ArgumentException("Se esperaba instancia o listado de AutorDTO");

                autoresDTO.ForEach(async autorDTO => await generadorEnlaces.GenerarEnlaces(autorDTO));
                resultado.Value = autoresDTO;
            }
            else
            {
                await generadorEnlaces.GenerarEnlaces(autorDTO);
            }
            
            await next();
        }
    }
}
