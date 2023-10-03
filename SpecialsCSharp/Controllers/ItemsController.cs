using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> ItemsPost([FromBody] SearchModel request)
  {
    try
    {
      var data = await DataAll(request.zip, request.radius);
      string json = JsonConvert.SerializeObject(data);

      return Content(json, "application/json");
    }
    catch (Exception ex)
    {
      // Handle exceptions here if needed
      return StatusCode(500, $"Internal server error: {ex.Message}");
    }
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public virtual async Task<object> DataAll(string zip, int radius)
  {
    // Read the JSON data from sample.json and deserialize it
    string json = await System.IO.File.ReadAllTextAsync("sample.json");
    var data = JsonConvert.DeserializeObject<object>(json);

    return data;
  }

}

public class SearchModel
{
  public string zip { get; set; }
  public int radius { get; set; }
}
