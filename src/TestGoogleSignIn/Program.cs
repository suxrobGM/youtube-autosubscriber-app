using System;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace TestGoogleSignIn
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new ChromeDriver())
            {
                var signInUrl = "https://accounts.google.com/signin/v2/identifier?service=youtube&uilel=3&passive=true&continue=https://www.youtube.com/signin?action_handle_signin=true&app=desktop&hl=en&next=%2F&hl=en&flowName=GlifWebSignIn&flowEntry=ServiceLogin";
                driver.Navigate().GoToUrl(signInUrl);
                driver.FindElementByXPath("//*[@id='identifierId']").SendKeys("suxrobGM@gmail.com");
                driver.FindElementByXPath("//*[@id='identifierNext']").Click();
                Thread.Sleep(3000);
                driver.FindElementByXPath("//*[@name='password']").SendKeys("1suxrobbek1");
                driver.FindElementByXPath("//*[@id='passwordNext']").Click();

                Console.WriteLine("\nFinished!");
                Console.ReadKey();
            }
        }
    }
}
