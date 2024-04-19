using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Seleniumtests_ushakov;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;
    public WebDriverWait wait;
    
    public void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("natharos@mail.ru");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("5EZiU7*GafseMFWf");
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Feed']")));
    }

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
        Authorization();
    }

    [Test]
    public void AuthorizationTest()
    {
        driver.Url.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [Test]
    public void NavigationCommunitiesTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        communityTitle.Text.Should().Be("Сообщества");
    }
    
    [Test]
    public void SearchProfileTest()
    {
        var search = driver.FindElement(By.CssSelector("[data-tid='SearchBar']"));
        search.Click();
        var searchInput = driver.FindElement(By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"));
        var userName = "Матвей Ушаков";
        searchInput.SendKeys(userName);
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ScrollContainer__inner']")));
        var searchHint = driver.FindElement(By.CssSelector("[data-tid='ComboBoxMenu__item'] div div [title='Матвей Ушаков']"));
        searchHint.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='EmployeeName']")));
        var profileName = driver.FindElement(By.CssSelector("[data-tid='EmployeeName']"));
        var profileUrl = driver.Url;
        Assert.Multiple(() =>
        {	
            Assert.That(profileUrl == 
                        "https://staff-testing.testkontur.ru/profile/f0a643b3-d25a-45b5-8473-7ea8798dda46",
                "не открылся искомый профиль");
            Assert.That(profileName.Text == userName, "не открылся искомый профиль");
        });
    }
    
    [Test]
    public void EditProfileTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");
        var details = driver.FindElements(By.CssSelector("label[data-tid='Input'] div div textarea"))[1];
        details.SendKeys(Keys.Control+"a");
        details.SendKeys(Keys.Backspace);
        string[] detailsTexts =
        {
            "Write Autotests", 
            "Привет Александру Овчинникову и Владимиру Митягину :)", 
            "Pet the cat <3 (*&$%#*"
        };
        var randomDetailsText = detailsTexts[new Random().Next(0, detailsTexts.Length)];
        details.SendKeys(randomDetailsText);
        var saveButton = driver.FindElements(By.TagName("button"))[3];
        saveButton.Click();
        var contactCard = driver.FindElement(By.CssSelector("[data-tid='Details'] div div div"));
        contactCard.Text.Should().Be(randomDetailsText);
    }

    [Test]
    public void AppVersionLogTest()
    {
        var versionButton = driver.FindElement(By.CssSelector("[data-tid='Version']"));
        versionButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='modal-content']")));
        var titleVersionLog = driver.FindElement(By.CssSelector("[data-tid='modal-content'] div div div div div h1"));
        titleVersionLog.Text.Should().Be("Журнал изменений");
    }
    
    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
    
}