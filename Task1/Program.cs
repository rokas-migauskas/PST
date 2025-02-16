using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

IWebDriver chromeDriver = new ChromeDriver();
chromeDriver.Navigate().GoToUrl("https://google.com");
chromeDriver.Quit();