
using System;
using Bitsie.Shop.Web.UI.Tests.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace Bitsie.Shop.Web.UI.Tests
{
    public class BaseTest
    {
        protected string BaseUrl = "http://localhost:8291";
        protected const int DefaultNumPerPage = 20;
        private const string _adminEmail = "admin@honeypot.com";
        private const string _adminPass = "test";

        /// <summary>
        /// Log a user into the website as an administrator
        /// </summary>
        /// <param name="driver"></param>
        protected void SignInAsAdmin(RemoteWebDriver driver)
        {
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl(BaseUrl + "/User/SignIn");

            // Find the text input element by its name
            driver.FindElement(By.Name("Email")).SendKeys(_adminEmail);
            driver.FindElement(By.Name("Password")).SendKeys(_adminPass);
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();
            driver.WaitFor(u => u.Url.Contains("Manage"));
        }
    }
}
