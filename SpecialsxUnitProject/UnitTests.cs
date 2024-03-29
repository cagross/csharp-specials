using Moq;
using SpecialsC_.API.Helpers;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using SpecialsCSharp.Services;

public class UnitTest
{

  [Fact]
  [Trait("TestType", "Unit")]
  public async Task DataAll_two_matching_stores()
  {
    //Case: two stores match search request.

    var sampleStoreData = new JObject
    {
        { "stores", new JArray
            {
                new JObject
                {
                    { "storeNo", "0233" },
                    { "address1", "7235 Arlington Blvd." },
                    { "city", "Falls Church" },
                    { "state", "VA" },
                    { "zip", "22042" }
                },
                new JObject
                {
                    { "storeNo", "0765" },
                    { "address1", "1230 W. Broad St." },
                    { "city", "Falls Church" },
                    { "state", "VA" },
                    { "zip", "22046" }
                }
            }
        }
    };

    // Sample item data
    var sampleItemData1 = new JObject
{
    { "category_names", new JArray("Meat") },
    { "current_price", "1.99" },
    { "price_text", "/lb" }
};
    var sampleItemData2 = new JObject
{
    { "category_names", new JArray("Meat") },
    { "current_price", "2.99" },
    { "price_text", "/lb" }
};

    var mockWrapper = new Mock<HttpHelperWrapper>();
    bool firstCall = true;
    bool secondCall = true;

    mockWrapper.Setup(x => x.SpFetchJson(It.IsAny<string>()))
        .ReturnsAsync((string param) =>
        {
          if (firstCall)
          {
            Assert.Equal("https://giantfood.com/apis/store-locator/locator/v1/stores/GNTL?storeType=GROCERY&q=22042&maxDistance=2&details=true", param);
            firstCall = false;
            return sampleStoreData; // First call returns sampleStoreData
          }
          else if (secondCall)
          {
            Assert.Equal("https://circular.giantfood.com/flyer_data/0123456?locale=en-US", param);
            secondCall = false;
            return new JObject { { "items", new JArray(sampleItemData1) } }; // Second call returns sampleItemData1
          }
          else
          {
            Assert.Equal("https://circular.giantfood.com/flyer_data/6543210?locale=en-US", param);
            return new JObject { { "items", new JArray(sampleItemData2) } }; // Third call returns sampleItemData2
          }
        });

    string str1 = "current_flyer_id--0123456";
    string str2 = "current_flyer_id--6543210";

    bool myFirstCall = true;
    mockWrapper.Setup(x => x.SpFetchText(It.IsAny<string>()))
            .ReturnsAsync((string param) =>
            {
              if (myFirstCall)
              {
                Assert.Equal("https://circular.giantfood.com/flyers/giantfood?type=2&show_shopping_list_integration=1&postal_code=22204&use_requested_domain=true&store_code=0233&is_store_selection=true&auto_flyer=&sort_by=#!/flyers/giantfood-weekly?flyer_run_id=406535", param);
                myFirstCall = false;
                return str1;
              }
              else
              {
                Assert.Equal("https://circular.giantfood.com/flyers/giantfood?type=2&show_shopping_list_integration=1&postal_code=22204&use_requested_domain=true&store_code=0765&is_store_selection=true&auto_flyer=&sort_by=#!/flyers/giantfood-weekly?flyer_run_id=406535", param);
                return str2;
              }
            });
    var itemsController = new ItemsService(mockWrapper.Object); // Instantiate your controller

    var actual = await itemsController.DataAll("22042", 2);

    var expected = new Dictionary<string, object>
          {
              {
                  "0233", new
                  {
                      storeLocation = new List<string>
                      {
                          "Giant Food",
                          "7235 Arlington Blvd.",
                          "Falls Church, VA 22042"
                      },
                      items = new List<object>
                      {
                          new
                          {
                              category_names = new List<string> { "Meat" },
                              current_price = "1.99",
                              price_text = "/lb",
                              unit_price = 1.99
                          }
                      }
                  }
              },
              {
                  "0765", new
                  {
                      storeLocation = new List<string>
                      {
                          "Giant Food",
                          "1230 W. Broad St.",
                          "Falls Church, VA 22046"
                      },
                      items = new List<object>
                      {
                          new
                          {
                              category_names = new List<string> { "Meat" },
                              current_price = "2.99",
                              price_text = "/lb",
                              unit_price = 2.99
                          }
                      }
                  }
              }
          };

    var actualJson = JsonConvert.SerializeObject(actual);
    var expectedJson = JsonConvert.SerializeObject(expected);

    Assert.Equal(expectedJson, actualJson);
  }

  [Fact]
  [Trait("TestType", "Unit")]
  public async Task DataAll_zero_matching_stores()
  {
    //Case: zero stores match search request.

    var sampleStoreData = new JObject { };

    var mockWrapper = new Mock<HttpHelperWrapper>();
    mockWrapper.Setup(x => x.SpFetchJson(It.IsAny<string>()))
        .ReturnsAsync(() =>
        {
          return sampleStoreData;
        });

    var itemsController = new ItemsService(mockWrapper.Object); // Instantiate your controller

    var actual = await itemsController.DataAll("22042", 1);
    var expected = new Dictionary<string, object> { };

    var actualJson = JsonConvert.SerializeObject(actual);
    var expectedJson = JsonConvert.SerializeObject(expected);

    Assert.Equal(expectedJson, actualJson);
  }

  [Fact]
  [Trait("TestType", "Unit")]
  public async Task DataAll_four_matching_stores()
  {
    //Case: four stores match search request.

    var emptyItems = new JObject
    {
    };
    var maxNumStores = 2;

    // Sample store data
    var sampleStoreData = new JObject
{
    { "stores", new JArray
        {
            new JObject
            {
                { "storeNo", "0233" },
                { "address1", "7235 Arlington Blvd." },
                { "city", "Falls Church" },
                { "state", "VA" },
                { "zip", "22042" }
            },
            new JObject
            {
                { "storeNo", "0765" },
                { "address1", "1230 W. Broad St." },
                { "city", "Falls Church" },
                { "state", "VA" },
                { "zip", "22046" }
            },
            new JObject
            {
                { "storeNo", "0234" },
                { "address1", "7235 Arlington Blvd." },
                { "city", "Falls Church" },
                { "state", "VA" },
                { "zip", "22042" }
            },
            new JObject
            {
                { "storeNo", "0766" },
                { "address1", "1230 W. Broad St." },
                { "city", "Falls Church" },
                { "state", "VA" },
                { "zip", "22046" }
            },
        }
    }
};

    var sampleItemData1 = new JObject
{
    { "category_names", new JArray("Meat") },
    { "current_price", "1.99" },
    { "price_text", "/lb" }
};
    var sampleItemData2 = new JObject
{
    { "category_names", new JArray("Meat") },
    { "current_price", "2.99" },
    { "price_text", "/lb" }
};
    var sampleItemData3 = new JObject
{
    { "category_names", new JArray("Meat") },
    { "current_price", "3.99" },
    { "price_text", "/lb" }
};

    var mockWrapper = new Mock<HttpHelperWrapper>();
    bool firstCall = true;
    bool secondCall = true;
    bool thirdCall = true;

    mockWrapper.Setup(x => x.SpFetchJson(It.IsAny<string>()))
          .ReturnsAsync(() =>
          {
            if (firstCall)
            {
              firstCall = false;
              return sampleStoreData;
            }
            else if (secondCall)
            {
              secondCall = false;
              return new JObject { { "items", new JArray(sampleItemData1) } };
            }
            else if (thirdCall)
            {
              thirdCall = false;
              return new JObject { { "items", new JArray(sampleItemData2) } };
            }
            else
            {
              return new JObject { { "items", new JArray(sampleItemData3) } };
            }
          })
          .Callback(() => { });

    //Strings must be over 18 chars in length, due to the implementation.
    string str1 = "sampletextsampletextsampletext";
    string str2 = "sampletextsampletextsampletext";
    string str3 = "sampletextsampletextsampletext";

    mockWrapper.SetupSequence(x => x.SpFetchText(It.IsAny<string>())).ReturnsAsync(str1).ReturnsAsync(str2).ReturnsAsync(str3);

    var itemsController = new ItemsService(mockWrapper.Object); // Instantiate your controller
    var myResult = await itemsController.DataAll("22042", 10);

    var actual = ((Dictionary<string, object>)myResult).Count;
    var expected = maxNumStores;

    Assert.Equal(expected, actual);

  }

  [Fact]
  [Trait("TestType", "Unit")]
  public void UnitPrice_ReturnsCorrectValue()
  {
    //Case: item price has an additional divisor e.g. '2 for $3.00.'
    var testItem = new Dictionary<string, object>
{
    { "description", "Selected Varieties, 6-9 oz. pkg." },
    { "current_price", "6.0"},
    { "name", "Perdue Chicken Short Cuts" },
    { "pre_price_text", "2/" },
    { "price_text", "" }
};

    var testUnitPrice = new PricingCalculator().UnitPrice(testItem);

    Assert.Equal(8.0m, testUnitPrice); // Use decimal for expected value
  }
  [Fact]
  [Trait("TestType", "Unit")]
  public void UnitPrice_ItemLb()
  {
    //Case: item price has 'lb' in price description.
    var testItem = new Dictionary<string, object>
    {
      {"description", "Frozen, 4 lb. pkg."},
      {"current_price", "12.99"},
      {"name", "Philly Gourmet Beef Patties"},
      {"pre_price_text", ""},
      {"price_text", "/ea."}
    };

    var testUnitPrice = new PricingCalculator().UnitPrice(testItem);

    Assert.Equal(3.2475m, testUnitPrice); // Use decimal for expected value
  }

  [Fact]
  [Trait("TestType", "Unit")]
  public void UnitPrice_ItemPriceTextLb()
  {
    //Case: item description does not contain 'lb' or 'oz' but price_text contains 'lb.
    var testItem = new Dictionary<string, object>
    {
      {"description", "Organic apples"},
      {"current_price", "10.00"},
      {"pre_price_text", ""},
      {"price_text", "/lb"}
    };

    var testUnitPrice = new PricingCalculator().UnitPrice(testItem);

    Assert.Equal(10.00m, testUnitPrice); // Use decimal for expected value
  }
  [Fact]
  [Trait("TestType", "Unit")]
  public async void StoreData_GiantValidData()
  {
    //Case: Giant Food store search API returns valid data.
    var sampleStoreNo1 = "0233";
    var sampleStoreNo2 = "0765";

    var sampleAddress1 = "7235 Arlington Blvd.";
    var sampleCity1 = "Falls Church";
    var sampleState1 = "VA";
    var sampleZip1 = "22042";

    var sampleAddress2 = "1230 W. Broad St.";
    var sampleCity2 = "Falls Church";
    var sampleState2 = "VA";
    var sampleZip2 = "22046";

    var sampleStoreData = new JObject
    {
      ["stores"] = new JArray
    {
        new JObject
        {
            ["storeNo"] = sampleStoreNo1,
            ["address1"] = sampleAddress1,
            ["city"] = sampleCity1,
            ["state"] = sampleState1,
            ["zip"] = sampleZip1
        },
        new JObject
        {
            ["storeNo"] = sampleStoreNo2,
            ["address1"] = sampleAddress2,
            ["city"] = sampleCity2,
            ["state"] = sampleState2,
            ["zip"] = sampleZip2
        }
    }
    };
    var mockWrapper = new Mock<HttpHelperWrapper>();
    mockWrapper.Setup(x => x.SpFetchJson(It.IsAny<string>())).ReturnsAsync(sampleStoreData);

    var storeService = new StoreService(mockWrapper.Object); // Instantiate your controller

    var expected = new ExpandoObject() as IDictionary<string, object>;
    expected[sampleStoreNo1] = AddressInfo.CreateAddressArray("Giant Food", sampleAddress1, $"{sampleCity1}, {sampleState1} {sampleZip1}");
    expected[sampleStoreNo2] = AddressInfo.CreateAddressArray("Giant Food", sampleAddress2, $"{sampleCity2}, {sampleState2} {sampleZip2}");

    var actual = await storeService.StoreData("22042", 2);

    var actualJson = JsonConvert.SerializeObject(actual);
    var expectedJson = JsonConvert.SerializeObject(expected);

    Assert.Equal(expectedJson, actualJson);
  }

}
