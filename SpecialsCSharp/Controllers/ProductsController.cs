using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpecialsCSharp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    [HttpGet]
    public string GetProducts()
    {
      return "Hello World (from Carl)";
    }
  }
}
