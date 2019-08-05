using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
