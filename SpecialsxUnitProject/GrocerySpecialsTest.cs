using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using SeleniumExtras.WaitHelpers;


public class GrocerySpecialsTest : IDisposable
{
  private IWebDriver driver;

  public GrocerySpecialsTest()
  {
    // Initialize the WebDriver (ChromeDriver in this case)
    driver = new ChromeDriver();
  }

  // Test that 'Grocery Specials' text is present on the page.
  [Fact]
  public void CheckGrocerySpecialsText()
  {
    // Navigate to your web application's URL
    driver.Navigate().GoToUrl("https://localhost:7165");

    // Check if "Grocery Specials" text is present on the page
    IWebElement element = driver.FindElement(By.XPath("//*[contains(text(),'Grocery Specials')]"));
    Assert.NotNull(element);
  }

  // Test that radio buttons are present on the page.
  [Fact]
  public void CheckRadioButtons()
  {
    // Navigate to your web application's URL
    driver.Navigate().GoToUrl("https://localhost:7165");

    // Check if there are four radio buttons on the page
    IReadOnlyCollection<IWebElement> radioButtons = driver.FindElements(By.CssSelector("input[type='radio']"));
    Assert.Equal(4, radioButtons.Count);
  }

  // Test that correct data is displayed (case: search contains at least one store).
  [Fact]
  public void FillInputFieldsSubmitAndCheckResponse()
  {
    // Navigate to your web application's URL
    driver.Navigate().GoToUrl("https://localhost:7165");

    // Locate and fill the input fields
    IWebElement zipInput = driver.FindElement(By.Name("zip"));
    zipInput.SendKeys("22042");

    IWebElement radiusInput = driver.FindElement(By.Name("radius"));
    radiusInput.SendKeys("2");

    // Find and click the submit button
    IWebElement submitButton = driver.FindElement(By.Id("button"));
    submitButton.Click();

    // Implement a wait for the server's response
    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("row__storaddress")));

    // Check the page for expected text content
    IWebElement element1 = driver.FindElement(By.XPath("//*[text()[contains(.,'7235 Arlington Blvd.')]]"));
    Assert.NotNull(element1);
    
    IWebElement element2 = driver.FindElement(By.XPath("//*[text()[contains(.,'1230 W. Broad St.')]]"));
    Assert.NotNull(element2);
  }

  public void Dispose()
  {
    // Dispose of the WebDriver when the test is done
    driver.Quit();
  }
}
