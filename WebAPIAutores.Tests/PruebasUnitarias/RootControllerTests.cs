using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPIAutores.Controllers.V1;
using WebAPIAutores.Tests.Mocks;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiEsUsuarioAdministrador()
        {
            // Preparacion
            var authService = new AuthorizationServiceMock();
            authService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authService);
            rootController.Url = new UrlHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiNoUsuarioAdministrador()
        {
            // Preparacion
            var authService = new AuthorizationServiceMock();
            authService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authService);
            rootController.Url = new UrlHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiNoUsuarioAdministradorConMoq()
        {
            // Preparacion
            var mockAuth = new Mock<IAuthorizationService>();
            mockAuth.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            ))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuth.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
            ))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            var rootController = new RootController(mockAuth.Object);

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
            ))
                .Returns(string.Empty);

            rootController.Url = mockUrl.Object;

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}
