using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Services
{
    public class Automatization
    {
        private IWebDriver _driver;

        public Automatization()
        {

        }

        public void OpenChrome()
        {
            _driver = new ChromeDriver();
        }

        public void HeadlessChrome()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            _driver = new ChromeDriver(driverService, chromeOptions);
        }

        public bool VerifyChannel(ref Channel channel)
        {
            var isYouTubeChannel = false;
            _driver.Navigate().GoToUrl(channel.Url);

            if (IsElementPresent(By.Id("subscriber-count")))
            {
                isYouTubeChannel = true;
                channel.SubscriberCount = Channel.ParseSubscriberCount(_driver.FindElement(By.Id("subscriber-count")).Text);
                channel.Title = _driver.FindElement(By.Id("channel-title")).Text;
            }
            _driver.Dispose();
            return isYouTubeChannel;
        }

        
        private bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        private void WaitForReady(By by)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromHours(2));
            wait.Until(driver =>
            {
                //bool isAjaxFinished = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return jQuery.active == 0");
                return IsElementPresent(by);
            });
        }
    }
}
