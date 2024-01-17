// using System.Text.Json;
using Newtonsoft.Json;
using System.IO.Compression;
namespace SpecialsC_.API.Helpers

{
  public class HttpHelper
  {
    public async Task<string> SpFetchText(string url)
    {
      using HttpClient client = new();
      client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
      client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
      HttpResponseMessage response = await client.GetAsync(url);

      if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
      {
        return "403"; // Return a string indicating a 403 error.
      }

      using Stream stream = await response.Content.ReadAsStreamAsync();
      Stream contentStream;
      string content;

      // Determine if the response is GZip compressed and set the appropriate stream
      if (response.Content.Headers.ContentEncoding.Any(encoding => encoding.ToLowerInvariant().Contains("gzip")))
      {
        // If the content is GZip compressed, use a GZipStream to decompress
        contentStream = new GZipStream(stream, CompressionMode.Decompress);
      }
      else
      {
        // If the content is not compressed, use the original stream
        contentStream = stream;
      }
      // Read the content from the stream
      using (var reader = new StreamReader(contentStream))
      {
        content = await reader.ReadToEndAsync();
      }
      return content;

    }
    public async Task<object> SpFetchJson(string url)
    {
      using HttpClient client = new();
      client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
      HttpResponseMessage response = await client.GetAsync(url);
      string json = await response.Content.ReadAsStringAsync();
      try
      {
        return JsonConvert.DeserializeObject<object>(json);
      }
      catch (JsonException ex)
      {
        Console.WriteLine("Error deserializing JSON: " + ex.Message);
      }
      return null;
    }

  }
  public class HttpHelperWrapper
  {
    public virtual Task<object> SpFetchJson(string url)
    {
      // Delegate the call to the actual implementation in HttpHelper
      return new HttpHelper().SpFetchJson(url);
    }
    public virtual Task<string> SpFetchText(string url)
    {
      // Delegate the call to the actual implementation in HttpHelper
      return new HttpHelper().SpFetchText(url);
    }
  }
}
