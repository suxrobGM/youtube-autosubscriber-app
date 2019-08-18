using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using YouTubeSubscriber.Data;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Services
{
    public class ChannelSubscriberService : IDisposable
    {
        private readonly IChannelAccountContext _context;
        private readonly IWebDriver _driver;
        private readonly Channel _channel;

        public event EventHandler OnProcessing;

        public ChannelSubscriberService(IChannelAccountContext context, Channel channel, bool useHeadlessChrome = false)
        {
            var chromeOptions = new ChromeOptions();
            if (useHeadlessChrome)
            {                
                chromeOptions.AddArgument("headless");
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                _driver = new ChromeDriver(driverService, chromeOptions);
            }
            else
            {
                chromeOptions.AddArgument("--disable-notifications");
                _driver = new ChromeDriver(chromeOptions);
            }

            _context = context;
            _channel = channel;
        }
       
        public bool VerifyChannel()
        {
            var isYouTubeChannel = false;
            _driver.Navigate().GoToUrl(_channel.Url);

            if (IsElementPresent(By.Id("subscriber-count")))
            {
                isYouTubeChannel = true;
                _channel.SubscriberCount = Channel.ParseSubscriberCount(_driver.FindElement(By.Id("subscriber-count")).Text);
                _channel.Title = _driver.FindElement(By.Id("channel-title")).Text;
            }
            
            return isYouTubeChannel;
        }

        public void SubscribeToChannel(int count)
        {
            var signInUrl = "https://accounts.google.com/signin/v2/identifier?service=youtube&uilel=3&passive=true&continue=https://www.youtube.com/signin?action_handle_signin=true&app=desktop&hl=en&next=%2F&hl=en&flowName=GlifWebSignIn&flowEntry=ServiceLogin";
            var unsubscribedAccounts = _context.Accounts.Where(i => !i.SubscribedChannels.Where(x => x.ChannelId == _channel.Id).Any()).Take(count);

            OnProcessing?.Invoke($"Starting subscription process", new EventArgs());
            int subscribedAccountsCount = 0;
            foreach (var unsubscribedAccount in unsubscribedAccounts)
            {
                OnProcessing?.Invoke($"Trying to login account {unsubscribedAccount.Email}", new EventArgs());

                Thread.Sleep(500);
                _driver.Navigate().GoToUrl(signInUrl);
                WaitForReady(By.Id("identifierNext"));
                _driver.FindElement(By.Id("identifierId")).SendKeys(unsubscribedAccount.Email);
                _driver.FindElement(By.Id("identifierNext")).Click();
                Thread.Sleep(1500);

                if (IsElementPresent(By.XPath("//div[@aria-live='assertive' and @aria-atomic='true']/div")))
                {
                    var errorMsg = $"Unregistered email, couldn't find Google account {unsubscribedAccount.Email}";
                    OnProcessing?.Invoke(errorMsg, new EventArgs());
                    continue;
                    //throw new Exception(errorMsg);
                }

                WaitForReady(By.Id("passwordNext"));
                _driver.FindElement(By.Name("password")).SendKeys(unsubscribedAccount.Password);
                _driver.FindElement(By.Id("passwordNext")).Click();
                Thread.Sleep(1500);

                if (IsElementPresent(By.XPath("//div[@aria-live='assertive']/div[2]")))
                {
                    var errorMsg = $"Wrong password for account {unsubscribedAccount.Email}";
                    OnProcessing?.Invoke(errorMsg, new EventArgs());
                    continue;
                    //throw new Exception(errorMsg);
                }

                if (!unsubscribedAccount.IsVerified)
                {
                    unsubscribedAccount.IsVerified = true;
                }

                WaitForReady(By.Id("content"));

                if (IsElementPresent(By.Id("identity-prompt-confirm-button")))
                {
                    _driver.FindElement(By.Id("dont_ask_again")).Click();
                    Thread.Sleep(500);
                    _driver.FindElement(By.Id("identity-prompt-confirm-button")).Click();
                }

                _driver.Navigate().GoToUrl(_channel.Url);
                WaitForReady(By.Id("subscribe-button"));

                if (IsElementPresent(By.XPath("//*[@id='subscribe-button']/.//paper-button[@subscribed]")))
                {
                    OnProcessing?.Invoke($"Already subscribed account {unsubscribedAccount.Email}", new EventArgs());
                    continue;
                }
                else
                {
                    _driver.FindElement(By.Id("subscribe-button")).Click();
                    OnProcessing?.Invoke($"Successfully subscribed account {unsubscribedAccount.Email} #{++subscribedAccountsCount}", new EventArgs());
                    var channelAccount = new ChannelAccount()
                    {
                        AccountId = unsubscribedAccount.Id,
                        Account = unsubscribedAccount,
                        ChannelId = _channel.Id,
                        Channel = _channel
                    };
                    unsubscribedAccount.SubscribedChannels.Add(channelAccount);
                }

                _context.SaveChanges();
            }          

            OnProcessing?.Invoke($"Finished subscription process", new EventArgs());
        }

        public void UnsubscribeToChannel(int count)
        {
            var signInUrl = "https://accounts.google.com/signin/v2/identifier?service=youtube&uilel=3&passive=true&continue=https://www.youtube.com/signin?action_handle_signin=true&app=desktop&hl=en&next=%2F&hl=en&flowName=GlifWebSignIn&flowEntry=ServiceLogin";
            var subscribedAccounts = _context.Accounts.Where(i => i.SubscribedChannels.Where(x => x.ChannelId == _channel.Id).Any()).Take(count);
            OnProcessing?.Invoke($"Starting unsubscription process", new EventArgs());
            int unsubscribedAccountsCount = 0;


        }

        public long GetSubscribersCount()
        {
            _driver.Navigate().GoToUrl(_channel.Url);
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
