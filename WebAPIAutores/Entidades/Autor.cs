using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido*")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no acepta mas de {1} caracteres")]
        [PrimeraLetraMayus]
        public string Nombre { get; set; }
    }
}
