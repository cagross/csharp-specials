/// <summary>
/// Contains methods for handling address information.
/// </summary>
public class AddressInfo
{
  /// <summary>
  /// Creates an array containing place name, street address, and city/state/ZIP.
  /// </summary>
  /// <param name="placeName">The name of the place.</param>
  /// <param name="streetAddress">The street address.</param>
  /// <param name="cityStateZip">The city, state, and ZIP code.</param>
  /// <returns>An array containing the address information.</returns>
  public static string[] CreateAddressArray(string placeName, string streetAddress, string cityStateZip)
  {
    return new string[] { placeName, streetAddress, cityStateZip };
  }
}