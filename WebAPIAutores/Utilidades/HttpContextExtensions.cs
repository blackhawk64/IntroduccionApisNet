﻿using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("TotalRegistros", cantidad.ToString());
        }
    }
}
