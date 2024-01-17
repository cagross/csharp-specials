using Microsoft.AspNetCore.Mvc;

public class HomeController : ControllerBase
{
  private readonly IHostEnvironment _env;
  public HomeController(IHostEnvironment env)
  {
    _env = env;
  }
  [HttpGet("/")]
  public IActionResult Index()
  {
    var webRoot = _env.ContentRootFileProvider.GetDirectoryContents("wwwroot");
    var indexHtml = webRoot.FirstOrDefault(file => file.Name == "index.html");
    if (indexHtml != null)
    {
      var stream = indexHtml.CreateReadStream();
      return File(stream, "text/html");
    }
    // If index.html is not found, return a not found response
    return NotFound();
  }
}
