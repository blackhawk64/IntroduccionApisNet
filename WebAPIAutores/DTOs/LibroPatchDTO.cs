using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class LibroPatchDTO
    {
        [PrimeraLetraMayus]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
