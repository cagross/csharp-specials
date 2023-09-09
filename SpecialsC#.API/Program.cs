using System.Text.Json; // Add this line

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient as a service
builder.Services.AddHttpClient();

var app = builder.Build();

var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

// Make the GET request and waitfor the response.
var response = await httpClient.GetAsync("https://api.wordpress.org/plugins/info/1.0/woocommerce.json");
if (response.IsSuccessStatusCode)
{

  System.Diagnostics.Debug.WriteLine("CAG: Success status code.");
  // Read the content as a string
  var json = await response.Content.ReadAsStringAsync();
  System.Diagnostics.Debug.WriteLine("CAG: json obtained.");
  System.Diagnostics.Debug.WriteLine(json);
  var myResponse = JsonSerializer.Deserialize<MyResponseModel>(json);
  string myValue = myResponse.name;
  System.Diagnostics.Debug.WriteLine($"myProp value: {myValue}");
}
else
{
  System.Diagnostics.Debug.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure default route (i.e. homepage) returns static text.
app.MapGet("/", async context =>
{
  await context.Response.WriteAsync("Future port of Grocery Specials web app to .NET based tech stack. See https://github.com/cagross/csharp-specials. See working Swagger page at https://specialscapi20230823213803.azurewebsites.net/swagger/index.html");
});

app.Run();
public class MyResponseModel
{
  public string name { get; set; } // Replace 'string' with the actual data type of myProp in your JSON
                                   // Add other properties matching your JSON structure
}