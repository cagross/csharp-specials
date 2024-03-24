// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;
using SpecialsC_.API.Helpers;
using System.Dynamic;
using Newtonsoft.Json.Linq;
namespace SpecialsCSharp.Services
{
  public class StoreService
  {
    private readonly HttpHelperWrapper _httpHelperWrapper;
    /// <summary>
    /// Initializes a new instance of the StoreService class.
    /// </summary>
    /// <param name="httpHelperWrapper">An instance of HttpHelperWrapper used for HTTP requests.</param>
    [ActivatorUtilitiesConstructor]
    public StoreService(HttpHelperWrapper httpHelperWrapper)
    {
      _httpHelperWrapper = httpHelperWrapper;
    }

    /// <summary>
    /// Fetch store data and return store code and location for all Giant Food stores within given zip/radius search parameters.
    /// </summary>
    /// <param name="zip">The zip code to search for stores.</param>
    /// <param name="radius">The search radius in miles.</param>
    /// <returns>Returns store data as an object. Contains store code and location of each store found in search.</returns>
    /// <exception cref="ArgumentNullException">Thrown when zip is null.</exception>
    /// <exception cref="ArgumentException">Thrown when radius is less than or equal to zero.</exception>
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

  }
}