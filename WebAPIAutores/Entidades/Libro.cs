using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [PrimeraLetraMayus]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
    }
}
