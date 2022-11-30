using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService
                ,IHttpContextAccessor httpContextAccessor
                ,IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = httpContextAccessor
                            .HttpContext
                            .RequestServices
                            .GetRequiredService<IUrlHelperFactory>();

            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }
        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var esAdmin = await authorizationService.AuthorizeAsync(httpContext.User, "EsAdmin");

            return esAdmin.Succeeded;
        }
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                    Url.Link("AutorPorId", new { id = autorDTO.Id }),
                    descripcion: "self",
                    metodo: "GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    Url.Link("ActualizarAutor", new { id = autorDTO.Id }),
                    descripcion: "Actualizar un autor",
                    metodo: "PUT"));
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                        Url.Link("EliminarAutor", new { id = autorDTO.Id }),
                        descripcion: "Eliminar un autor",
                        metodo: "DELETE"));
            }
        }
    }
}
