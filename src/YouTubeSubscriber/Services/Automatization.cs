using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Services
{
    public class Automatization : IDisposable
    {
        private readonly IWebDriver _driver;
        public event EventHandler OnSubscribing;

        public Automatization(bool headlessChrome = false)
        {
            if (headlessChrome)
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                _driver = new ChromeDriver(driverService, chromeOptions);
            }
            else
            {
                _driver = new ChromeDriver();
            }
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
            
            return isYouTubeChannel;
        }

        public void SubscribeToChannel(Channel channel, ref Account[] accounts)
        {
            var signInUrl = "https://accounts.google.com/signin/v2/identifier?service=youtube&uilel=3&passive=true&continue=https://www.youtube.com/signin?action_handle_signin=true&app=desktop&hl=en&next=%2F&hl=en&flowName=GlifWebSignIn&flowEntry=ServiceLogin";

            OnSubscribing?.Invoke($"Starting subscription process", new EventArgs());

            for (int i = 0; i < accounts.Length; i++)                     
            {
                OnSubscribing?.Invoke($"Trying to login account {accounts[i].Email}", new EventArgs());

                _driver.Navigate().GoToUrl(signInUrl);
                WaitForReady(By.Id("identifierNext"));
                _driver.FindElement(By.Id("identifierId")).SendKeys(accounts[i].Email);
                _driver.FindElement(By.Id("identifierNext")).Click();
                Thread.Sleep(1500);

                if (IsElementPresent(By.XPath("//div[@aria-live='assertive' and @aria-atomic='true']/div")))
                {
                    var errorMsg = $"Unregistered email, couldn't find Google account {accounts[i].Email}";
                    OnSubscribing?.Invoke(errorMsg, new EventArgs());
                    continue;
                    //throw new Exception(errorMsg);
                }

                WaitForReady(By.Id("passwordNext"));
                _driver.FindElement(By.Name("password")).SendKeys(accounts[i].Password);
                _driver.FindElement(By.Id("passwordNext")).Click();
                Thread.Sleep(1500);

                if (IsElementPresent(By.XPath("//div[@aria-live='assertive']/div[2]")))
                {
                    var errorMsg = $"Wrong password for account {accounts[i].Email}";
                    OnSubscribing?.Invoke(errorMsg, new EventArgs());
                    continue;
                    //throw new Exception(errorMsg);
                }

                if (!accounts[i].IsVerified)
                {
                    accounts[i].IsVerified = true;
                }

                WaitForReady(By.Id("footer-container"));

                if (IsElementPresent(By.Id("identity-prompt-confirm-button")))
                {
                    _driver.FindElement(By.Id("identity-prompt-confirm-button")).Click();
                }

                _driver.Navigate().GoToUrl(channel.Url);
                WaitForReady(By.Id("subscribe-button"));

                if (IsElementPresent(By.Id("notification-preference-toggle-button")))
                {
                    OnSubscribing?.Invoke($"Already subscribed account {accounts[i].Email}", new EventArgs());
                    continue;
                }
                else
                {
                    _driver.FindElement(By.Id("subscribe-button")).Click();
                    int count = i + 1;
                    OnSubscribing?.Invoke($"Successfully subscribed account {accounts[i].Email} #{count}", new EventArgs());
                }             
            }

            OnSubscribing?.Invoke($"Finished subscription process", new EventArgs());
        }

        public long GetSubscribersCount(Channel channel)
        {
            _driver.Navigate().GoToUrl(channel.Url);
            WaitForReady(By.Id("subscriber-count"));
            return Channel.ParseSubscriberCount(_driver.FindElement(By.Id("subscriber-count")).Text);
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

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
