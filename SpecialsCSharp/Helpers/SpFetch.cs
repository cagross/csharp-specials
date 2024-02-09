using Newtonsoft.Json;
using System.IO.Compression;
namespace SpecialsC_.API.Helpers

{
  public class HttpHelper
  {
    /// <summary>
    /// Fetches text content from the specified URL asynchronously and returns it as a string.
    /// </summary>
    /// <param name="url">The URL from which to fetch the text content.</param>
    /// <returns>A string containing the fetched text content.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP request error occurs.</exception>
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
    /// <summary>
    /// Fetches JSON content from the specified URL asynchronously and deserializes it to an object.
    /// </summary>
    /// <param name="url">The URL from which to fetch the JSON content.</param>
    /// <returns>An object representing the deserialized JSON content, or null if an error occurs during deserialization.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP request error occurs.</exception>
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
    /// <summary>
    /// Fetches JSON content from the specified URL asynchronously by delegating the call to the actual implementation in HttpHelper.
    /// </summary>
    /// <param name="url">The URL from which to fetch the JSON content.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an object representing the fetched JSON content.</returns>
    /// <remarks>
    /// This method serves as a wrapper around the SpFetchJson method in HttpHelper, allowing for easier mocking and testing of JSON fetching operations.
    /// </remarks>
    public virtual Task<object> SpFetchJson(string url)
    {
      // Delegate the call to the actual implementation in HttpHelper
      return new HttpHelper().SpFetchJson(url);
    }
    /// <summary>
    /// Fetches text content from the specified URL asynchronously by delegating the call to the actual implementation in HttpHelper.
    /// </summary>
    /// <param name="url">The URL from which to fetch the text content.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a string representing the fetched text content.</returns>
    /// <remarks>
    /// This method serves as a wrapper around the SpFetchText method in HttpHelper, facilitating easier mocking and testing of text fetching operations.
    /// </remarks>
    public virtual Task<string> SpFetchText(string url)
    {
      // Delegate the call to the actual implementation in HttpHelper
      return new HttpHelper().SpFetchText(url);
    }
  }
}
