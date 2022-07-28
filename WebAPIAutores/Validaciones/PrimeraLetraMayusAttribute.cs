using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.Validaciones
{
    public class PrimeraLetraMayusAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra del nombre debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
