using Bitsie.Shop.Web.UI.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Bitsie.Shop.Web.UI.Tests.User
{
    /// <summary>
    /// Log UI Tests
    /// </summary>
    [TestClass]
    public class LogTests : BaseTest
    {
        [TestMethod]
        public void Log_Num_Per_Page_Shows_Records()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(u => u.Url.Contains("Manage/Log"));
                driver.FindElement(By.CssSelector("a.select2-choice")).Click();
                driver.FindElement(By.CssSelector(".select2-result-selectable:nth-child(4)")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > DefaultNumPerPage);
                Assert.AreEqual(100, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Search_Logs_By_Message_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".searchTerm")).SendKeys("Sample log 30");
                driver.FindElement(By.CssSelector(".table-filter button")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Date_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript("$('.date-range').trigger('click')");
                (driver as IJavaScriptExecutor).ExecuteScript("$('.ranges ul li:nth-child(2)').trigger('click')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Category_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('select[name=""LogCategory""]').val('Application')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var rows = driver.FindElementsByCssSelector(".table tbody tr");
                var filtered = true;
                foreach (var el in rows)
                {
                    var td = el.FindElement(By.CssSelector("td:nth-child(5)"));
                    if (td.Text != "Application") filtered = false;
                }
                Assert.IsTrue(filtered);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Level_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('select[name=""LogLevel""]').val('Warning')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var rows = driver.FindElementsByCssSelector(".table tbody tr");
                var filtered = true;
                foreach (var el in rows)
                {
                    var td = el.FindElement(By.CssSelector("td:nth-child(4)"));
                    if (td.Text != "Warning") filtered = false;
                }
                Assert.IsTrue(filtered);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_User_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""UserId""]').val('1')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var td = driver.FindElementByCssSelector(".table tbody tr td:first-child");
                Assert.AreEqual("99", td.Text);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Message_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""Message""]').val('Sample log 86')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Details_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""Details""]').val('Log details 86')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Paging_Logs_Shows_Next_page()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".page-2")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table .log-row-80")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".table .log-row-80")));
            }
        }
        
        [TestMethod]
        public void View_Log_Details_Shows_Log_Info()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Logs")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".log-row-100 a")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector("#viewLogForm .form-horizontal")).Count > 0);
                Assert.IsTrue(driver.FindElementByCssSelector("#viewLogForm").Text.Contains("100"));
            }
        }
    }
}
