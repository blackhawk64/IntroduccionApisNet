using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido*")]
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} no acepta mas de {1} caracteres")]
        public string Nombre { get; set; }
        [Range(18,90)]
        [NotMapped]
        public int Edad { get; set; }
        [CreditCard]
        [NotMapped]
        public string TarjetaCredito { get; set; }
        [Url]
        [NotMapped]
        public string UrlAutor { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
