using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Task4;

public static class TestUser
{
    public static string Email { get; set; }
    public static string Password { get; set; }
}

public class RegistrationFixture : IDisposable
{
    public RegistrationFixture()
    {
        IWebDriver registrationDriver = new ChromeDriver();
        registrationDriver.Manage().Window.Maximize();
        registrationDriver.Navigate().GoToUrl("https://demowebshop.tricentis.com/");
        WebDriverWait registrationWait = new WebDriverWait(registrationDriver, TimeSpan.FromSeconds(20));
        registrationWait.Until(
            ExpectedConditions.ElementIsVisible(By.XPath("//a[contains(@class, 'ico-register')]")));
        registrationDriver.FindElement(By.XPath("//a[contains(@class, 'ico-register')]")).Click();
        registrationWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='register-button']")));
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string newUserEmail = "testuser_" + timeStamp + "@example.com";
        string newUserPassword = "Test123!";
        registrationDriver.FindElement(By.XPath("//input[@id='gender-male']")).Click();
        registrationDriver.FindElement(By.XPath("//input[@id='FirstName']")).SendKeys("Test");
        registrationDriver.FindElement(By.XPath("//input[@id='LastName']")).SendKeys("User");
        registrationDriver.FindElement(By.XPath("//input[@id='Email']")).SendKeys(newUserEmail);
        registrationDriver.FindElement(By.XPath("//input[@id='Password']")).SendKeys(newUserPassword);
        registrationDriver.FindElement(By.XPath("//input[@id='ConfirmPassword']")).SendKeys(newUserPassword);
        registrationDriver.FindElement(By.XPath("//input[@id='register-button']")).Click();
        registrationWait.Until(driver => driver.PageSource.Contains("Your registration completed"));
        registrationDriver.FindElement(By.XPath("//input[@value='Continue']")).Click();
        TestUser.Email = newUserEmail;
        TestUser.Password = newUserPassword;
        registrationDriver.Quit();
    }

    public void Dispose()
    {
    }
}

[CollectionDefinition("Registration Collection")]
public class RegistrationCollection : ICollectionFixture<RegistrationFixture>
{
}

public class DigitalDownloadsTestBase : IDisposable
{
    protected IWebDriver driver;
    protected WebDriverWait wait;
    protected string baseUrl = "https://demowebshop.tricentis.com/";

    public DigitalDownloadsTestBase()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public void Dispose()
    {
        driver.Quit();
    }

    protected void Login()
    {
        driver.Navigate().GoToUrl(baseUrl);
        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[contains(@class, 'ico-login')]")));
        driver.FindElement(By.XPath("//a[contains(@class, 'ico-login')]")).Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='Email']")));
        driver.FindElement(By.XPath("//input[@id='Email']")).SendKeys(TestUser.Email);
        driver.FindElement(By.XPath("//input[@id='Password']")).SendKeys(TestUser.Password);
        driver.FindElement(By.XPath("//input[@value='Log in']")).Click();
        wait.Until(driver => driver.PageSource.Contains("Log out"));
    }

    protected void AddProductsToCartFromFile(string dataFilePath)
    {
        string[] productNames = File.ReadAllLines(dataFilePath);
        foreach (string productNameRaw in productNames)
        {
            string productName = productNameRaw.Trim();
            if (!string.IsNullOrEmpty(productName))
            {
                driver.Navigate().GoToUrl(baseUrl);
                var digitalDownloadsLink = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath(
                        "//div[@class='block block-category-navigation']//a[contains(., 'Digital downloads')]")));
                digitalDownloadsLink.Click();
                wait.Until(driver => driver.PageSource.Contains(productName));
                IWebElement productLink = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath(
                        $"//h2[@class='product-title']/a[contains(normalize-space(text()), '{productName}')]")));
                productLink.Click();
                wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[starts-with(@id, 'add-to-cart-button-')]")));
                IWebElement addToCartButton =
                    driver.FindElement(By.XPath("//input[starts-with(@id, 'add-to-cart-button-')]"));
                addToCartButton.Click();
                wait.Until(driver => driver.PageSource.Contains("The product has been added to your"));
            }
        }
    }

    protected void CheckoutOrder()
    {
        driver.FindElement(By.XPath("//a[contains(@class, 'ico-cart')]")).Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='termsofservice']")));
        IWebElement termsCheckbox = driver.FindElement(By.XPath("//input[@id='termsofservice']"));
        if (!termsCheckbox.Selected)
        {
            termsCheckbox.Click();
        }

        IWebElement checkoutButton = driver.FindElement(By.XPath("//button[@id='checkout']"));
        checkoutButton.Click();
        Thread.Sleep(2000); // Wait for the billing section to load

        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("co-billing-form")));

        // Check for an existing billing address using the billing-address-select dropdown
        var billingAddressSelectElements = driver.FindElements(By.XPath("//select[@id='billing-address-select']"));
        if (billingAddressSelectElements.Count > 0)
        {
            var billingSelect = new OpenQA.Selenium.Support.UI.SelectElement(billingAddressSelectElements[0]);
            string selectedValue = billingSelect.SelectedOption.GetAttribute("value");
            if (!string.IsNullOrEmpty(selectedValue))
            {
                // An existing address is selected; simply click the Continue button
                IWebElement billingContinueButton = driver.FindElement(
                    By.XPath("//input[contains(@class, 'new-address-next-step-button') and @value='Continue']"));
                billingContinueButton.Click();
            }
            else
            {
                // No existing address selected; fill in the new billing address fields
                FillBillingFields();
            }
        }
        else
        {
            // Billing address select not present; assume new address form is already visible and fill it
            FillBillingFields();
        }

        // Payment Method Step
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//input[contains(@class, 'payment-method-next-step-button') and @value='Continue']")));
        IWebElement paymentMethodContinueButton = driver.FindElement(
            By.XPath("//input[contains(@class, 'payment-method-next-step-button') and @value='Continue']"));
        paymentMethodContinueButton.Click();

        // Payment Information Step
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//input[contains(@class, 'payment-info-next-step-button') and @value='Continue']")));
        IWebElement paymentInfoContinueButton =
            driver.FindElement(
                By.XPath("//input[contains(@class, 'payment-info-next-step-button') and @value='Continue']"));
        paymentInfoContinueButton.Click();

        // Confirm Order Step
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//input[contains(@class, 'confirm-order-next-step-button') and @value='Confirm']")));
        IWebElement confirmOrderButton =
            driver.FindElement(
                By.XPath("//input[contains(@class, 'confirm-order-next-step-button') and @value='Confirm']"));
        confirmOrderButton.Click();

        wait.Until(driver => driver.PageSource.Contains("Your order has been successfully processed!"));
    }

    private void FillBillingFields()
    {
        IWebElement billingFirstName = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_FirstName']"));
        billingFirstName.Clear();
        billingFirstName.SendKeys("Test");

        IWebElement billingLastName = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_LastName']"));
        billingLastName.Clear();
        billingLastName.SendKeys("User");

        IWebElement billingEmail = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_Email']"));
        billingEmail.Clear();
        billingEmail.SendKeys(TestUser.Email);

        IWebElement billingCountry = driver.FindElement(By.XPath("//select[@id='BillingNewAddress_CountryId']"));
        new OpenQA.Selenium.Support.UI.SelectElement(billingCountry).SelectByValue("1"); // United States

        IWebElement billingCity = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_City']"));
        billingCity.Clear();
        billingCity.SendKeys("TestCity");

        IWebElement billingAddress1 = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_Address1']"));
        billingAddress1.Clear();
        billingAddress1.SendKeys("123 Test St.");

        IWebElement billingZip = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_ZipPostalCode']"));
        billingZip.Clear();
        billingZip.SendKeys("12345");

        IWebElement billingPhone = driver.FindElement(By.XPath("//input[@id='BillingNewAddress_PhoneNumber']"));
        billingPhone.Clear();
        billingPhone.SendKeys("5551234567");

        IWebElement billingContinueButton =
            driver.FindElement(
                By.XPath("//input[contains(@class, 'new-address-next-step-button') and @value='Continue']"));
        billingContinueButton.Click();
    }
}

[Collection("Registration Collection")]
public class DigitalDownloadsTestData1 : DigitalDownloadsTestBase
{
    [Fact]
    public void TestOrderDigitalDownloadUsingData1()
    {
        Login();
        string dataFilePath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, "data1.txt");
        AddProductsToCartFromFile(dataFilePath);
        CheckoutOrder();
    }
}

[Collection("Registration Collection")]
public class DigitalDownloadsTestData2 : DigitalDownloadsTestBase
{
    [Fact]
    public void TestOrderDigitalDownloadUsingData2()
    {
        Login();
        string dataFilePath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, "data2.txt");
        AddProductsToCartFromFile(dataFilePath);
        CheckoutOrder();
    }
}