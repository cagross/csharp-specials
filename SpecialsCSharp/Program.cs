// using System.Text;
// using System.Text.Json;

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseFileServer();

// Add a new route to return JSON data from sample.json
app.Map("/items", app =>
{
  app.Run(async context =>
  {
    // Read the JSON data from sample.json
    string json = File.ReadAllText("sample.json");

    // Return the JSON object as the response
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(json);
  });
});

app.MapFallbackToFile("index.html");

// var requestUrl = "https://api.openai.com/v1/chat/completions";
// var requestData = new
// {
//   model = "gpt-3.5-turbo",
//   messages = new[]
//     {
//         new
//         {
//             role = "system",
//             content = "You are an expert at cooking, especially food native to the United States and each of its 50 states."
//         },
//         new
//         {
//             role = "user",
//             content = "First choose a random US state.  Then tell me the most popular food dish in that state. Please return your answer formatted as JSON containing two different properties: stateName and dishName.  Do not include any other text in your answer."
//         }
//     }
// };
// // Serialize the object to a JSON string.
// var jsonPayload = JsonSerializer.Serialize(requestData);
// // Create the content for the POST request.
// var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
// // Add the Authorization header with your key.
// string apiKey = Environment.GetEnvironmentVariable("OAI_API_KEY");
// httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

// Ensure default route (i.e. homepage) returns static text.
// app.MapGet("/", async context =>
// {
//   // Make the POST request and wait for the response.
//   var response = await httpClient.PostAsync(requestUrl, content);
//   if (response.IsSuccessStatusCode)
//   {
//     System.Diagnostics.Debug.WriteLine("CAG: Success status code.");
//     // Read the content as a string.
//     var json = await response.Content.ReadAsStringAsync();
//     System.Diagnostics.Debug.WriteLine("CAG: json obtained.");
//     System.Diagnostics.Debug.WriteLine(json);

//     // Deserialize the response JSON if needed.
//     var responseObject = JsonSerializer.Deserialize<MyResponseModel>(json);

//     // Access the content property from choices in the JSON response
//     string myResponse = responseObject.choices[0].message.content;
//     System.Diagnostics.Debug.WriteLine($"myResponse value: {myResponse}");

//     ContentModel contentModel = JsonSerializer.Deserialize<ContentModel>(myResponse);
//     // Now you can access the individual properties.
//     string stateName = contentModel.stateName;
//     string dishName = contentModel.dishName;

//     System.Diagnostics.Debug.WriteLine($"stateName value: {stateName}");
//     System.Diagnostics.Debug.WriteLine($"dishName value: {dishName}");

//     await context.Response.WriteAsync($"Future port of Grocery Specials web app to .NET based tech stack. In addition, it will contain an AI component. In fact, here is some content fetched from ChatGPT: The most popular food dish in {stateName} is {dishName}.  See https://github.com/cagross/csharp-specials. See working Swagger page at https://specialscapi20230823213803.azurewebsites.net/swagger/index.html");
//   }
//   else
//   {
//     System.Diagnostics.Debug.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
//   }
// });

app.Run();
// public class MyResponseModel
// {
//   public string id { get; set; }
//   public string @object { get; set; }
//   public long created { get; set; }
//   public string model { get; set; }
//   public List<Choice> choices { get; set; } // Define 'choices' property directly
//   public Usage usage { get; set; }
// }

// public class Choice
// {
//   public int index { get; set; }
//   public Message message { get; set; }
//   public string finish_reason { get; set; }
// }

// public class Message
// {
//   public string role { get; set; }
//   public string content { get; set; }
// }

// public class Usage
// {
//   public int prompt_tokens { get; set; }
//   public int completion_tokens { get; set; }
//   public int total_tokens { get; set; }
// }

// public class ContentModel
// {
//   public string stateName { get; set; }
//   public string dishName { get; set; }
// }