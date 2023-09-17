using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

public class GrocerySpecialsTest : IDisposable
{
  private IWebDriver driver;

  public GrocerySpecialsTest()
  {
    // Initialize the WebDriver (ChromeDriver in this case)
    driver = new ChromeDriver();
  }

  [Fact]
  public void CheckGrocerySpecialsText()
  {
    // Navigate to your web application's URL
    driver.Navigate().GoToUrl("https://localhost:7165");

    // Check if "Grocery Specials" text is present on the page
    IWebElement element = driver.FindElement(By.XPath("//*[contains(text(),'Grocery Specials')]"));
    Assert.NotNull(element);
  }

  public void Dispose()
  {
    // Dispose of the WebDriver when the test is done
    driver.Quit();
  }
}
