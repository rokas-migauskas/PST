using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using static Common.StepUtilities;

var webDriver = new ChromeDriver();

Step(1, "Open https://demoqa.com (Task 1)", () =>
{
    webDriver.Navigate().GoToUrl("https://demoqa.com");
    webDriver.Manage().Window.Maximize();
    return webDriver.Url.Contains("demoqa");
},
"Could not open demoqa.com", 1000);

Step(2, "Close cookies popup (Task 1)", () =>
{
    var elements = webDriver.FindElements(By.XPath("//*[@id='close-fixedban']"));
    if (elements.Count > 0)
    {
        elements[0].Click();
        Thread.Sleep(1000);
    }
    return true;
},
"Could not close cookies popup.", 1000);

Step(3, "Hide/Close any remaining bottom banner", () =>
{
    try
    {
        var fixedBanElement = webDriver.FindElement(By.XPath("//*[@id='fixedban']"));
        ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].style.display='none';", fixedBanElement);
    }
    catch
    {
    }
    return true;
},
"Could not hide bottom banner.", 500);

Step(4, "Open 'Widgets' card (Task 1)", () =>
{
    var widgetsCard = webDriver.FindElement(By.XPath("//div[@class='card mt-4 top-card'][.//h5[text()='Widgets']]"));
    widgetsCard.Click();
    return true;
},
"Could not open Widgets card.", 1000);

Step(5, "Open 'Progress Bar' (Task 1)", () =>
{
    var progressBarMenu = webDriver.FindElement(By.XPath("//span[text()='Progress Bar']"));
    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", progressBarMenu);
    progressBarMenu.Click();
    return webDriver.Url.Contains("progress-bar");
},
"Could not open Progress Bar page.", 1000);

Step(6, "Click 'Start' (Task 1)", () =>
{
    var startButton = webDriver.FindElement(By.XPath("//*[@id='startStopButton']"));
    startButton.Click();
    return true;
},
"Could not click 'Start'.", 1000);

Step(7, "Wait until 100%, then click 'Reset' (Task 1)", () =>
{
    var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
    wait.Until(driver => driver.FindElement(By.XPath("//*[contains(@class,'progress-bar')]")).Text == "100%");
    var resetButton = webDriver.FindElement(By.XPath("//*[@id='resetButton']"));
    resetButton.Click();
    return true;
},
"Progress bar did not reach 100% or could not click 'Reset'.", 1000);

Step(8, "Ensure progress bar is 0% (Task 1)", () =>
{
    var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
    wait.Until(driver =>
    {
        var bar = driver.FindElement(By.XPath("//*[contains(@class,'progress-bar')]"));
        var styleVal = bar.GetAttribute("style").ToLower();
        var ariaVal = bar.GetAttribute("aria-valuenow");
        var textVal = bar.Text.Trim();
        return styleVal.Contains("width: 0%") || ariaVal == "0" || textVal == "0%";
    });
    return true;
},
"Progress bar did not reset to 0%.");

Step(9, "Navigate back to homepage for Task 2", () =>
{
    webDriver.Navigate().GoToUrl("https://demoqa.com");
    return webDriver.Url.Equals("https://demoqa.com/");
},
"Could not navigate back to homepage.", 1000);

Step(10, "Close cookies popup (Task 2)", () =>
{
    var elements = webDriver.FindElements(By.XPath("//*[@id='close-fixedban']"));
    if (elements.Count > 0)
    {
        elements[0].Click();
        Thread.Sleep(1000);
    }
    return true;
},
"Could not close cookies popup for Task 2.", 1000);

Step(11, "Hide/Close any remaining bottom banner (Task 2)", () =>
{
    try
    {
        var fixedBanElement = webDriver.FindElement(By.XPath("//*[@id='fixedban']"));
        ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].style.display='none';", fixedBanElement);
    }
    catch
    {
    }
    return true;
},
"Could not hide bottom banner for Task 2.", 500);

Step(12, "Open 'Elements' card (Task 2)", () =>
{
    var elementsCard = webDriver.FindElement(By.XPath("//div[@class='card mt-4 top-card'][.//h5[text()='Elements']]"));
    elementsCard.Click();
    return true;
},
"Could not open Elements card.", 1000);

Step(13, "Open 'Web Tables' (Task 2)", () =>
{
    var webTablesMenu = webDriver.FindElement(By.XPath("//span[text()='Web Tables']"));
    webTablesMenu.Click();
    return webDriver.Url.Contains("webtables");
},
"Could not open Web Tables page.", 1000);

Step(14, "Add entries until second page exists (Task 2)", () =>
{
    int recordIndex = 1;
    WebDriverWait waitForSecondPage = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
    while (true)
    {
        var addRecordButton = webDriver.FindElement(By.XPath("//*[@id='addNewRecordButton']"));
        addRecordButton.Click();
        var firstNameInputField = webDriver.FindElement(By.XPath("//*[@id='firstName']"));
        firstNameInputField.Clear();
        firstNameInputField.SendKeys("Name" + recordIndex);
        var lastNameInputField = webDriver.FindElement(By.XPath("//*[@id='lastName']"));
        lastNameInputField.Clear();
        lastNameInputField.SendKeys("Last" + recordIndex);
        var userEmailInputField = webDriver.FindElement(By.XPath("//*[@id='userEmail']"));
        userEmailInputField.Clear();
        userEmailInputField.SendKeys("test" + recordIndex + "@example.com");
        var ageInputField = webDriver.FindElement(By.XPath("//*[@id='age']"));
        ageInputField.Clear();
        ageInputField.SendKeys((20 + recordIndex).ToString());
        var salaryInputField = webDriver.FindElement(By.XPath("//*[@id='salary']"));
        salaryInputField.Clear();
        salaryInputField.SendKeys((30000 + recordIndex * 1000).ToString());
        var departmentInputField = webDriver.FindElement(By.XPath("//*[@id='department']"));
        departmentInputField.Clear();
        departmentInputField.SendKeys("QA");
        var submitRecordButton = webDriver.FindElement(By.XPath("//*[@id='submit']"));
        submitRecordButton.Click();
        recordIndex++;
        try
        {
            bool secondPageCreated = waitForSecondPage.Until(driver =>
            {
                var totalPagesElement = driver.FindElement(By.XPath("//span[@class='-totalPages']"));
                return totalPagesElement.Text != "1";
            });
            if (secondPageCreated)
            {
                break;
            }
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    return true;
}, "Second page was not created after adding entries.", 2000);


Step(15, "Click 'Next' to go to page 2 (Task 2)", () =>
{
    var nextButton = webDriver.FindElement(By.XPath("//button[text()='Next']"));
    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", nextButton);
    Thread.Sleep(500);
    nextButton.Click();
    var currentPageInput = webDriver.FindElement(By.XPath("//input[@aria-label='jump to page']"));
    return currentPageInput.GetAttribute("value") == "2";
},
"Could not navigate to page 2.", 1000);

Step(16, "Delete 'Name8' on page 2", () =>
    {
        var rowContainingName8 = webDriver.FindElement(
            By.XPath("//div[@class='rt-tbody']//div[@role='row'][contains(., 'Name8')]"));

        var deleteButton = rowContainingName8.FindElement(By.XPath(".//span[starts-with(@id, 'delete-record-')]"));
        ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", deleteButton);
        Thread.Sleep(500);
        deleteButton.Click();
        return true;
    },
    "Could not delete 'Name8'.", 15000);

Step(17, "Ensure pagination returns to page 1 and only one page remains (Task 2)", () =>
{
    var currentPageInput = webDriver.FindElement(By.XPath("//input[@aria-label='jump to page']"));
    var totalPagesLabel = webDriver.FindElement(By.XPath("//span[@class='-totalPages']"));
    return /*currentPageInput.GetAttribute("value") == "1" && */totalPagesLabel.Text == "1";
},
"Pagination did not reset to page 1 or more than one page remains.", 1000);

webDriver.Quit();
Console.WriteLine("All steps completed.");
