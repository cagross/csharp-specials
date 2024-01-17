using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpecialsC_.API.Helpers;
using System.Text.RegularExpressions;
using System.Dynamic;
using Newtonsoft.Json.Linq;

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
  private readonly HttpHelperWrapper _httpHelperWrapper;
  [ActivatorUtilitiesConstructor]
  public ItemsController(HttpHelperWrapper httpHelperWrapper)
  {
    _httpHelperWrapper = httpHelperWrapper;
  }

  public ItemsController()
  {
    // You can initialize any default values or set up dependencies here
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public async Task<object> StoreData(string zip, int radius)
  {
    var urlAPIData = $"https://giantfood.com/apis/store-locator/locator/v1/stores/GNTL?storeType=GROCERY&q={zip}&maxDistance={radius}&details=true";
    var fetchedStoreData = await _httpHelperWrapper.SpFetchJson(urlAPIData);

    var storesArray = (fetchedStoreData as JObject)?["stores"] as JArray;

    var storeData = new ExpandoObject();
    if (storesArray != null)
    {
      foreach (var storeToken in storesArray)
      {
        if (storeToken is JObject store)
        {
          var storeNo = (string)store["storeNo"];
          var addressArray = AddressInfo.CreateAddressArray(
              "Giant Food",
              (string)store["address1"],
              $"{(string)store["city"]}, {(string)store["state"]} {(string)store["zip"]}"
          );

          ((IDictionary<string, object>)storeData).Add(storeNo, addressArray);
        }
      }
    }
    return storeData;
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public virtual async Task<object> DataAll(string zip, int radius)
  {
    var storeCodeLoc = await StoreData(zip, radius);

    if (storeCodeLoc == null)
    {
      // Handle the error case
      return storeCodeLoc;
    }

    var storeDataAll = new Dictionary<string, object>();
    int max = Math.Min(((IDictionary<string, object>)storeCodeLoc).Count, 2);

    foreach (var storeCode in ((IDictionary<string, object>)storeCodeLoc).Keys.Take(max))

    {
      var storeLocation = (string[])((IDictionary<string, object>)storeCodeLoc)[storeCode];
      var items = await ApiData(storeCode); // Assuming apiData can handle this parameter
      storeDataAll.Add(storeCode, new
      {
        storeLocation,
        items
      });
    }

    return storeDataAll;
  }
  [ApiExplorerSettings(IgnoreApi = true)]
  public async Task<object[]> ApiData(string storeCode)
  {
    // Define the URL for the first SpFetch call.
    var urlAPIFlyer = "https://circular.giantfood.com/flyers/giantfood?type=2&show_shopping_list_integration=1&postal_code=22204&use_requested_domain=true&store_code=" + storeCode + "&is_store_selection=true&auto_flyer=&sort_by=#!/flyers/giantfood-weekly?flyer_run_id=406535";

    // Call the first SpFetch method to get flyer info.
    var flyerInfo = await _httpHelperWrapper.SpFetchText(urlAPIFlyer);

    // Extract flyerID from the flyer info.
    var posFlyerID = flyerInfo.IndexOf("current_flyer_id");
    var flyerID = flyerInfo.Substring(posFlyerID + 18, 7);

    // Define the URL for fetching data.
    var urlAPIData = "https://circular.giantfood.com/flyer_data/" + flyerID + "?locale=en-US";

    // Call the second SpFetch method to get the data.
    var dataAll = await _httpHelperWrapper.SpFetchJson(urlAPIData);

    if (dataAll is JObject jObject && jObject.TryGetValue("items", out JToken itemsToken) && itemsToken is JArray itemsArray)
    {
      var filteredItems = ProductFilter(itemsArray.ToObject<List<Dictionary<string, object>>>(), 1);

      var myVal = filteredItems.Select(item =>
      {
        var myItem = (dataAll as JObject)?.GetValue("items")?[item];

        if (itemsArray[item] is JObject itemObject)
        {
          if (itemObject["current_price"] is JValue currentValue && currentValue.Value == null)
          {
            // Handle the case where "current_price" is null
            // You can leave this block blank or add appropriate logic
          }
          else
          {
            // Calculate and set "unit_price" property using UnitPrice method
            itemsArray[item]["unit_price"] = UnitPrice(itemObject.ToObject<Dictionary<string, object>>());
          }
        }
        return myItem;
      }).ToArray();
      return myVal;
    }
    else
    {
      // Handle the case where "Items" is not present or has an unexpected type
      Console.WriteLine("Error: 'Items' property not found or has an unexpected type.");
      return Array.Empty<object>();
    }
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public decimal UnitPrice(Dictionary<string, object> item)
  {
    Console.WriteLine("CAG: starting UnitPrice method. Console.WriteLine");
    string weightString = "";
    string pattern = @"[0-9]+";
    decimal weight = 0.0M; // Initialize weight with a default value
    decimal unitPrice = 0.0M; // Initialize unitPrice with a default value
    string partial;
    string currentPriceStr;
    decimal currentPrice;

    string priceText = (string)item["price_text"] ?? "";
    int posLbText = priceText.IndexOf("lb", StringComparison.OrdinalIgnoreCase);
    if (posLbText >= 0)
    {
      currentPriceStr = (string)item["current_price"];
      unitPrice = decimal.Parse(currentPriceStr);
    }
    else
    {
      bool hasEa = priceText.Contains("/ea", StringComparison.OrdinalIgnoreCase);
      if (hasEa || priceText == "")
      {
        if (item["description"] != null)
        {
          int divisor = 1;
          if (item["pre_price_text"] != null)
          {
            if (((string)item["pre_price_text"]).Length > 0)

            {
              int posSlash = ((string)item["pre_price_text"]).LastIndexOf("/");
              partial = ((string)item["pre_price_text"]).Substring(0, posSlash);
              int.TryParse(partial, out divisor);
            }
          }
          int posOz = ((string)item["description"]).IndexOf("oz.", StringComparison.OrdinalIgnoreCase);
          if (posOz >= 0)
          {
            partial = ((string)item["description"]).Substring(0, posOz);
            Match match = Regex.Match(partial, pattern);
            if (match.Success)
            {
              weightString = match.Value;
              int parsedWeight = int.Parse(weightString);
              weight = parsedWeight / 16m;
            }
          }
          int posLb = ((string)item["description"]).IndexOf("lb.", StringComparison.OrdinalIgnoreCase);
          if (posLb >= 0)
          {
            partial = ((string)item["description"]).Substring(0, posLb);
            Match match = Regex.Match(partial, pattern);
            if (match.Success)
            {
              weightString = match.Value;
              int parsedWeight = int.Parse(weightString);
              weight = parsedWeight;
            }
          }
          currentPriceStr = (string)item["current_price"];
          currentPrice = decimal.Parse(currentPriceStr);
          if (weight != 0)
          {
            unitPrice = currentPrice / weight / divisor;
          }

        }
      }
    }
    return unitPrice;
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public List<int> ProductFilter(List<Dictionary<string, object>> dataItems, int filterCode)
  {
    List<int> filteredItems = new List<int>();
    List<string> arrMeatDeli = new List<string> { "Meat", "Deli" };
    int index = 0;
    foreach (var item in dataItems)
    {
      switch (filterCode)
      {
        case 1:
          if (item is Dictionary<string, object> itemData)

          {
            if (itemData.ContainsKey("category_names") && itemData["category_names"] is JArray categoryNamesArray)
            {
              List<string> categoryNames = categoryNamesArray.Select(token => token.ToString()).ToList();
              if (categoryNames.Count > 0 && categoryNames[0] is string categoryName)
              {
                if (arrMeatDeli.Contains(categoryName))
                {
                  filteredItems.Add(index);
                }
              }
            }
          }
          break;
        default:
          filteredItems.Add(index);
          break;
      }
      index++;
    }
    return filteredItems;
  }
}

public class SearchModel
{
  public string zip { get; set; }
  public int radius { get; set; }
}

public class AddressInfo
{
  public static string[] CreateAddressArray(string placeName, string streetAddress, string cityStateZip)
  {
    return new string[] { placeName, streetAddress, cityStateZip };
  }
}
