using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Autor>> Get()
        {
            return new List<Autor>()
            {
                new Autor() { Id = 1, Nombre = "Alexis"},
                new Autor() { Id = 2, Nombre = "Sandra"}
            };
        }
    }
}
