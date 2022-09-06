using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayus]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }

    }
}
