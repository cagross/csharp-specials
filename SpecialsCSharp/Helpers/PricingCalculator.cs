using System.Text.RegularExpressions;

namespace SpecialsC_.API.Helpers
{
  public class PricingCalculator
  {
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
  }
}