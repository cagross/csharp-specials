using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpecialsC_.API.Helpers;
using SpecialsCSharp.Services;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
  /// <summary>
  /// Retrieves items based on the provided search criteria.
  /// </summary>
  /// <param name="request">The search criteria containing zip code and radius.
  /// <para>
  /// The <paramref name="request"/> parameter is expected to have the following properties:
  /// </para>
  /// <list type="bullet">
  ///   <item>
  ///     <term>zip</term>
  ///     <description>A string representing the zip code.</description>
  ///   </item>
  ///   <item>
  ///     <term>radius</term>
  ///     <description>An integer representing the search radius in miles.</description>
  ///   </item>
  /// </list>
  /// </param>
  [HttpPost]
  public async Task<IActionResult> ItemsPost([FromBody] SearchParameters request)
  {
    var _httpHelperWrapper = new HttpHelperWrapper();
    var itemsService = new ItemsService(_httpHelperWrapper);
    try
    {
      var data = await itemsService.DataAll(request.zip, request.radius);
      string json = JsonConvert.SerializeObject(data);
      return Content(json, "application/json");
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Internal server error: {ex.Message}");
    }
  }

  public ItemsController()
  {
  }

}
