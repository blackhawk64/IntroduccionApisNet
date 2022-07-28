using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido*")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no acepta mas de {1} caracteres")]
        //[PrimeraLetraMayus]
        public string Nombre { get; set; }

        //[Range(18,90)]
        //[NotMapped]
        //public int Edad { get; set; }

        //[CreditCard]
        //[NotMapped]
        //public string TarjetaCredito { get; set; }

        //[Url]
        //[NotMapped]
        //public string UrlAutor { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula",
                        new string[] {nameof(Nombre)});
                }
            }
        }
    }
}
