using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpecialsC_.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public string GetProducts() {
            return "Hello World";
        }
    }
}
