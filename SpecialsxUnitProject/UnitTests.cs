using Moq;
using SpecialsC_.API.Helpers;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;

public class UnitTest
{
  [Fact]
  public async Task TestItemsPost()
  {
    var mockController = new Mock<ItemsController>();
    mockController
        .Setup(x => x.DataAll(It.IsAny<string>(), It.IsAny<int>()))
        .ReturnsAsync(new { myProp = 555 });

    var testObj = new SearchModel { zip = "22042", radius = 2 };

    await mockController.Object.ItemsPost(testObj);

    mockController.Verify(x => x.DataAll(testObj.zip, testObj.radius), Times.Once);
  }
  [Fact]
  public async Task TestDataAll()
  {

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
        .ReturnsAsync(() =>
        {
          if (firstCall)
          {
            firstCall = false;
            return sampleStoreData; // First call returns sampleStoreData
          }
          else if (secondCall)
          {
            secondCall = false;
            return new JObject { { "items", new JArray(sampleItemData1) } }; // Second call returns sampleItemData1
          }
          else
          {
            return new JObject { { "items", new JArray(sampleItemData2) } }; // Third call returns sampleItemData2
          }
        });

    string str1 = "current_flyer_id--0123456";
    string str2 = "current_flyer_id--6543210";

    mockWrapper.SetupSequence(x => x.SpFetchText(It.IsAny<string>()))
        .ReturnsAsync(str1)
        .ReturnsAsync(str2);

    var itemsController = new ItemsController(mockWrapper.Object); // Instantiate your controller

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
  public void UnitPrice_ReturnsCorrectValue()
  {
    var itemsController = new ItemsController(); // Instantiate your controller
    var testItem = new Dictionary<string, object>
{
    { "description", "Selected Varieties, 6-9 oz. pkg." },
    { "current_price", "6.0"},
    { "name", "Perdue Chicken Short Cuts" },
    { "pre_price_text", "2/" },
    { "price_text", "" }
};

    var testUnitPrice = itemsController.UnitPrice(testItem);

    Assert.Equal(8.0m, testUnitPrice); // Use decimal for expected value
  }
  [Fact]
  public void UnitPrice_ItemLb()
  {
    var itemsController = new ItemsController(); // Instantiate your controller
    var testItem = new Dictionary<string, object>
    {
      {"description", "Frozen, 4 lb. pkg."},
      {"current_price", "12.99"},
      {"name", "Philly Gourmet Beef Patties"},
      {"pre_price_text", ""},
      {"price_text", "/ea."}
    };

    var testUnitPrice = itemsController.UnitPrice(testItem);

    Assert.Equal(3.2475m, testUnitPrice); // Use decimal for expected value
  }
  [Fact]
  public void UnitPrice_ItemPriceTextLb()
  {
    var itemsController = new ItemsController(); // Instantiate your controller
    var testItem = new Dictionary<string, object>
    {
      {"description", "Organic apples"},
      {"current_price", "10.00"},
      {"pre_price_text", ""},
      {"price_text", "/lb"}
    };

    var testUnitPrice = itemsController.UnitPrice(testItem);

    Assert.Equal(10.00m, testUnitPrice); // Use decimal for expected value
  }
  [Fact]
  public async void StoreData_GiantValidData()
  {
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

    var itemsController = new ItemsController(mockWrapper.Object); // Instantiate your controller

    var expected = new ExpandoObject() as IDictionary<string, object>;
    expected[sampleStoreNo1] = AddressInfo.CreateAddressArray("Giant Food", sampleAddress1, $"{sampleCity1}, {sampleState1} {sampleZip1}");
    expected[sampleStoreNo2] = AddressInfo.CreateAddressArray("Giant Food", sampleAddress2, $"{sampleCity2}, {sampleState2} {sampleZip2}");

    var actual = await itemsController.StoreData("22042", 2);

    var actualJson = JsonConvert.SerializeObject(actual);
    var expectedJson = JsonConvert.SerializeObject(expected);

    Assert.Equal(expectedJson, actualJson);
  }

  //Case: zero stores match search request.
  [Fact]
  public async Task DataAll_zero_matching_stores()
  {
    var sampleStoreData = new JObject { };

    var mockWrapper = new Mock<HttpHelperWrapper>();
    mockWrapper.Setup(x => x.SpFetchJson(It.IsAny<string>()))
        .ReturnsAsync(() =>
        {
          return sampleStoreData;
        });

    var itemsController = new ItemsController(mockWrapper.Object); // Instantiate your controller

    var actual = await itemsController.DataAll("22042", 1);
    var expected = new Dictionary<string, object> { };

    var actualJson = JsonConvert.SerializeObject(actual);
    var expectedJson = JsonConvert.SerializeObject(expected);

    Assert.Equal(expectedJson, actualJson);
  }

}
