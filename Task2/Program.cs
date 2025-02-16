using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using static Common.StepUtilities;

var webDriver = new ChromeDriver();

Step(1, "Open demowebshop.tricentis.com", () =>
{
    webDriver.Navigate().GoToUrl("https://demowebshop.tricentis.com/");
    webDriver.Manage().Window.Maximize();
    return webDriver.Url.Contains("demowebshop.tricentis.com");
},
"Could not open demowebshop.tricentis.com", 1000);

Step(2, "Click 'Gift Cards' in left menu", () =>
{
    var giftCardsLink = webDriver.FindElement(By.XPath("//div[@class='block block-category-navigation']//a[contains(text(),'Gift Cards')]"));
    giftCardsLink.Click();
    return webDriver.Url.Contains("gift-cards");
},
"Could not open Gift Cards page after click", 1000);

Step(3, "Select item with price > 99", () =>
    {
        var productOverNinetyNine = webDriver.FindElement(By.XPath("//div[@class='item-box'][.//span[@class='price actual-price'][number(translate(., '$', '')) > 99]]//h2[@class='product-title']/a"));
    
        if (productOverNinetyNine == null) return false;
    
        productOverNinetyNine.Click();
        return true;
    },
    "No product found with price over 99.", 1000);


Step(4, "Fill 'Recipient's Name', 'Your Name', set Qty=5000", () =>
{
    var recipientsNameField = webDriver.FindElement(By.XPath("//div[@class='giftcard']//input[contains(@id,'_RecipientName')]"));
    recipientsNameField.Clear();
    recipientsNameField.SendKeys("John");

    var yourNameField = webDriver.FindElement(By.XPath("//div[@class='giftcard']//input[contains(@id,'_SenderName')]"));
    yourNameField.Clear();
    yourNameField.SendKeys("Jane");

    var giftCardQuantityField = webDriver.FindElement(By.XPath("//div[@class='add-to-cart']//input[contains(@id,'EnteredQuantity')]"));
    giftCardQuantityField.Clear();
    giftCardQuantityField.SendKeys("5000");
    return true;
},
"Could not fill gift card fields.", 1000);

Step(6, "Click 'Add to cart'", () =>
{
    var addToCartButtonGift = webDriver.FindElement(By.XPath("//div[@class='add-to-cart-panel']//input[contains(@id,'add-to-cart-button-')]"));
    addToCartButtonGift.Click();
    return true;
},
"Could not click 'Add to cart' for gift card.", 1000);

Step(7, "Click 'Add to wish list' for gift card", () =>
{
    var addToWishlistGift = webDriver.FindElement(By.XPath("//input[@class='button-2 add-to-wishlist-button']"));
    addToWishlistGift.Click();
    return true;
},
"Could not click 'Add to wish list' for gift card.", 1000);

Step(8, "Click 'Jewelry' in left menu", () =>
{
    var jewelryLink = webDriver.FindElement(By.XPath("//div[@class='block block-category-navigation']//a[contains(text(),'Jewelry')]"));
    jewelryLink.Click();
    return webDriver.Url.Contains("jewelry");
},
"Could not open Jewelry page.", 1000);

Step(9, "Click 'Create Your Own Jewelry'", () =>
{
    var createYourOwnJewelryLink = webDriver.FindElement(By.XPath("//h2[@class='product-title']//a[contains(text(),'Create Your Own Jewelry')]"));
    createYourOwnJewelryLink.Click();
    return true;
},
"Could not click 'Create Your Own Jewelry'.", 1000);

Step(10, "Select Material=Silver (1 mm), Length=80, Pendant=Star, Qty=26", () =>
{
    var materialDropdown = new SelectElement(webDriver.FindElement(By.XPath("//*[@id='product_attribute_71_9_15']")));
    materialDropdown.SelectByText("Silver (1 mm)");

    var lengthInput = webDriver.FindElement(By.XPath("//*[@id='product_attribute_71_10_16']"));
    lengthInput.Clear();
    lengthInput.SendKeys("80");

    var starRadioButton = webDriver.FindElement(By.XPath("//*[@id='product_attribute_71_11_17_50']"));
    starRadioButton.Click();

    var jewelryQuantityField = webDriver.FindElement(By.XPath("//*[@id='addtocart_71_EnteredQuantity']"));
    jewelryQuantityField.Clear();
    jewelryQuantityField.SendKeys("26");
    return true;
},
"Could not select or enter jewelry options.", 1000);

Step(12, "Click 'Add to cart' for jewelry", () =>
{
    var addToCartButtonJewelry = webDriver.FindElement(By.XPath("//*[@id='add-to-cart-button-71']"));
    addToCartButtonJewelry.Click();
    return true;
},
"Could not click 'Add to cart' for jewelry.", 1000);

Step(13, "Click 'Add to wish list' for jewelry", () =>
{
    var addToWishlistJewelry = webDriver.FindElement(By.XPath("//*[@id='add-to-wishlist-button-71']"));
    addToWishlistJewelry.Click();
    return true;
},
"Could not click 'Add to wish list' for jewelry.", 1000);

Step(14, "Click 'Wishlist' at top of page", () =>
{
    var wishlistLink = webDriver.FindElement(By.XPath("//a[@class='ico-wishlist']"));
    wishlistLink.Click();
    return webDriver.Url.Contains("wishlist");
},
"Could not open Wishlist page.", 1000);

Step(15, "Check 'Add to cart' boxes for both wishlist items", () =>
{
    var wishlistRows = webDriver.FindElements(By.XPath("//table[@class='cart']/tbody/tr"));
    foreach (var wishlistRow in wishlistRows)
    {
        var addToCartCheckbox = wishlistRow.FindElement(By.XPath(".//input[@name='addtocart']"));
        addToCartCheckbox.Click();
    }
    return wishlistRows.Count > 0;
},
"No wishlist items to check for 'Add to cart'.", 1000);

Step(16, "Click 'Add to cart' button in wishlist", () =>
{
    var wishlistAddToCartButton = webDriver.FindElement(By.XPath("//input[@name='addtocartbutton']"));
    wishlistAddToCartButton.Click();
    return true;
},
"Could not click 'Add to cart' in wishlist.", 2000);

Step(17, "Verify 'Sub-Total' = '1002600.00'", () =>
{
    Thread.Sleep(3000);
    var totalElement = webDriver.FindElement(By.XPath("//span[@class='product-price order-total']/strong"));
    var totalValue = totalElement.Text;
    return totalValue == "1002600.00";
},
"Sub-Total is not '1002600.00'");

webDriver.Quit();
Console.WriteLine("All steps completed successfully!");
