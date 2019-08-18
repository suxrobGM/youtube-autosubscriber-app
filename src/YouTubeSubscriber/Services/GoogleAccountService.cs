using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Services
{
    public class GoogleAccountService : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly Account _account;

        public event EventHandler OnProcessing;

        public GoogleAccountService(Account account)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-notifications");
            _driver = new ChromeDriver(chromeOptions);
            _account = account;
        }

        public bool VerifyAccount()
        {
            var isVerifiedAccount = false;
            _driver.Navigate().GoToUrl("https://accounts.google.com/login");

            WaitForReady(By.Id("identifierNext"));
            _driver.FindElement(By.Id("identifierId")).SendKeys(_account.Email);
            _driver.FindElement(By.Id("identifierNext")).Click();
            Thread.Sleep(1500);

            if (IsElementPresent(By.XPath("//div[@aria-live='assertive' and @aria-atomic='true']/div")))
            {
                var errorMsg = $"Unregistered email, couldn't find Google account {_account.Email}";
                OnProcessing?.Invoke(errorMsg, new EventArgs());
                return false;
            }

            WaitForReady(By.Id("passwordNext"));
            _driver.FindElement(By.Name("password")).SendKeys(_account.Password);
            _driver.FindElement(By.Id("passwordNext")).Click();
            Thread.Sleep(1500);

            if (IsElementPresent(By.XPath("//div[@aria-live='assertive']/div[2]")))
            {
                var errorMsg = $"Wrong password for account {_account.Email}";
                OnProcessing?.Invoke(errorMsg, new EventArgs());
                return false;
            }
            else
            {
                isVerifiedAccount = true;
            }

            return isVerifiedAccount;
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
