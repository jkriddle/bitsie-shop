using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace Bitsie.Shop.Web.UI.Tests.Extensions
{
    public static class WebDriverExtensions
    {
        /// <summary>
        /// Indicates if the specified element exists in the DOM
        /// </summary>
        /// <param name="driver">Current driver</param>
        /// <param name="by">Filter properties</param>
        /// <returns>True if exists</returns>
        public static bool ElementExists(this RemoteWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Wait until the specified condition is true
        /// </summary>
        /// <param name="driver">Current driver</param>
        /// <param name="condition">Condition to wait for</param>
        public static void WaitFor(this RemoteWebDriver driver, Func<IWebDriver, bool> condition)
        {
            new WebDriverWait(driver, new TimeSpan(0, 0, 3)).Until(condition);
        }
    }
}
