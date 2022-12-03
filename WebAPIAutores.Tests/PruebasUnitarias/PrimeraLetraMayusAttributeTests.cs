using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusAttributeTests
    {
        [TestMethod]
        public void PrimeraMinusculaDevuelveError()
        {
            // Preparacion
            var primeraMayuscula = new PrimeraLetraMayusAttribute();
            var valorPrueba = "alexis";
            var contextoValidacion = new ValidationContext(new
            {
                Nombre = valorPrueba
            });

            // Ejecucion
            var resultado = primeraMayuscula.GetValidationResult(valorPrueba, contextoValidacion);

            // Verificacion
            Assert.AreEqual("La primera letra del nombre debe ser mayúscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void EsUnValorNulo()
        {
            // Preparacion
            var valorNulo = new PrimeraLetraMayusAttribute();
            string valorPrueba = null;
            var contextoValidacion = new ValidationContext(new
            {
                Nombre = valorNulo
            });

            // Ejecucion
            var resultado = valorNulo.GetValidationResult(valorPrueba, contextoValidacion);

            // Verificacion
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void PrimeraMayusculaEsCorrecto()
        {
            // Preparacion
            var valorNulo = new PrimeraLetraMayusAttribute();
            string valorPrueba = "Alexis";
            var contextoValidacion = new ValidationContext(new
            {
                Nombre = valorNulo
            });

            // Ejecucion
            var resultado = valorNulo.GetValidationResult(valorPrueba, contextoValidacion);

            // Verificacion
            Assert.IsNull(resultado);
        }
    }
}