﻿using System;
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
            if (useHeadlessChrome)
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

            _context = context;
            _channel = channel;
        }

        public Account[] GetSubcribedAccounts(int count)
        {
            var accounts = _context.Accounts.Where(i => i.SubscribedChannels.Where(x => x.ChannelId == _channel.Id).Any()).Take(count);

            return accounts.ToArray();
        }

        public Account[] GetUnsubcribedAccounts(int count)
        {
            var accounts = _context.Accounts.Where(i => !i.SubscribedChannels.Where(x => x.ChannelId == _channel.Id).Any()).Take(count);

            return accounts.ToArray();
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

        public void SubscribeToChannel(Account[] accounts)
        {
            var signInUrl = "https://accounts.google.com/signin/v2/identifier?service=youtube&uilel=3&passive=true&continue=https://www.youtube.com/signin?action_handle_signin=true&app=desktop&hl=en&next=%2F&hl=en&flowName=GlifWebSignIn&flowEntry=ServiceLogin";

            OnProcessing?.Invoke($"Starting subscription process", new EventArgs());

            for (int i = 0; i < accounts.Length; i++)                     
            {
                OnProcessing?.Invoke($"Trying to login account {accounts[i].Email}", new EventArgs());

                _driver.Navigate().GoToUrl(signInUrl);
                WaitForReady(By.Id("identifierNext"));
                _driver.FindElement(By.Id("identifierId")).SendKeys(accounts[i].Email);
                _driver.FindElement(By.Id("identifierNext")).Click();
                Thread.Sleep(1500);

                if (IsElementPresent(By.XPath("//div[@aria-live='assertive' and @aria-atomic='true']/div")))
                {
                    var errorMsg = $"Unregistered email, couldn't find Google account {accounts[i].Email}";
                    OnProcessing?.Invoke(errorMsg, new EventArgs());
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
                    OnProcessing?.Invoke(errorMsg, new EventArgs());
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

                _driver.Navigate().GoToUrl(_channel.Url);
                WaitForReady(By.Id("subscribe-button"));

                if (IsElementPresent(By.Id("notification-preference-toggle-button")))
                {
                    OnProcessing?.Invoke($"Already subscribed account {accounts[i].Email}", new EventArgs());
                    continue;
                }
                else
                {
                    _driver.FindElement(By.Id("subscribe-button")).Click();
                    int count = i + 1;
                    OnProcessing?.Invoke($"Successfully subscribed account {accounts[i].Email} #{count}", new EventArgs());
                    var channelAccount = new ChannelAccount()
                    {
                        AccountId = accounts[i].Id,
                        Account = accounts[i],
                        ChannelId = _channel.Id,
                        Channel = _channel
                    };  
                    accounts[i].SubscribedChannels.Add(channelAccount);
                }             
            }

            OnProcessing?.Invoke($"Finished subscription process", new EventArgs());
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
