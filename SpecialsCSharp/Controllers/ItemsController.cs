using Microsoft.AspNetCore.Mvc;
using System.IO;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
  [HttpPost]
  public IActionResult GetItems()
  {
    try
    {
      // Read the JSON data from sample.json
      string json = System.IO.File.ReadAllText("sample.json");

      // Return the JSON object as the response
      return Content(json, "application/json");
    }
    catch (Exception ex)
    {
      // Handle exceptions here if needed
      return StatusCode(500, $"Internal server error: {ex.Message}");
    }
  }
}
