// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;
using SpecialsC_.API.Helpers;
using System.Dynamic;
using Newtonsoft.Json.Linq;
namespace SpecialsCSharp.Services
{
  public class ItemsService
  {
    private readonly HttpHelperWrapper _httpHelperWrapper;
    /// <summary>
    /// Initializes a new instance of the ItemsService class.
    /// </summary>
    /// <param name="httpHelperWrapper">An instance of HttpHelperWrapper used for HTTP requests.</param>
    [ActivatorUtilitiesConstructor]
    public ItemsService(HttpHelperWrapper httpHelperWrapper)
    {
      _httpHelperWrapper = httpHelperWrapper;
    }

    /// <summary>
    /// Uses zip/radius search parameters to find circular items from all stores found within the zip/radius search. 
    /// </summary>
    /// <param name="zip">The zip code to search for stores.</param>
    /// <param name="radius">The search radius in miles.</param>
    /// <returns>Returns circular data for all stores as an object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when zip is null.</exception>
    /// <exception cref="ArgumentException">Thrown when radius is le
    public virtual async Task<object> DataAll(string zip, int radius)
    {
      // var storeCodeLoc = await storeService.StoreData(zip, radius);
      // var _httpHelperWrapper = new HttpHelperWrapper();
      var storeService = new StoreService(_httpHelperWrapper);
      var storeCodeLoc = await storeService.StoreData(zip, radius);

      // var storeCodeLoc = await StoreData(zip, radius);

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

    /// <summary>
    /// Fetch all weekly circular data from one Giant Food store and return only those from the deli department.
    /// </summary>
    /// <param name="storeCode">The code representing the store for which data is to be retrieved.</param>
    /// <returns>Returns an array of objects containing data retrieved from the API.</returns>
    public async Task<object[]> ApiData(string storeCode)
    {
      var itemsService = new ItemsService(_httpHelperWrapper);
      // var storeCodeLoc = await storeService.StoreData(zip, radius);
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
        var filteredItems = itemsService.ProductFilter(itemsArray.ToObject<List<Dictionary<string, object>>>(), 1);

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
              itemsArray[item]["unit_price"] = new PricingCalculator().UnitPrice(itemObject.ToObject<Dictionary<string, object>>());
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

    /// <summary>
    /// Accept all circular items and filter by specific departments, e.g. meat, deli, etc.  For now, it filters by only the meat/deli department.
    /// </summary>
    /// <param name="dataItems">A list of dictionaries containing data items to be filtered.</param>
    /// <param name="filterCode">An integer representing the filter code.</param>
    /// <returns>Returns a list of integers representing filtered item indices.</returns>
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
}