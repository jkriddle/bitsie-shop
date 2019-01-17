using System;
using Bitsie.Shop.Web.UI.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Bitsie.Shop.Web.UI.Tests.User
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [TestClass]
    public class UserTests : BaseTest
    {

        [TestMethod]
        public void Reset_Password_Shows_Error_Mismatched_Password()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword?token=badtoken");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='Password']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='ConfirmPassword']")).SendKeys("bad");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Reset_Password_Shows_Error_Missing_Confirm_Password()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword?token=badtoken");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='Password']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Reset_Password_Shows_Error_Empty_Password()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword?token=member2");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Reset_Password_Shows_Error_Invalid_Token()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword?token=badtoken");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='Password']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='ConfirmPassword']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Reset_Password_Shows_Error_Missing_Token()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='Password']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='ConfirmPassword']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Reset_Password_Shows_Success_With_Valid_Info()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/ResetPassword?token=member2");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='Password']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm input[name='ConfirmPassword']")).SendKeys("test");
                driver.FindElement(By.CssSelector("#resetPasswordForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-success")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-success").Count);
            }
        }

        [TestMethod]
        public void Forgot_Password_Shows_Error_Email_Not_Found()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/SignIn");
                driver.FindElement(By.PartialLinkText("Forgot")).Click();
                driver.FindElement(By.CssSelector("#forgotForm input[name='Email']")).SendKeys("notfound@mail.com");
                driver.FindElement(By.CssSelector("#forgotForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-error").Count);
            }
        }

        [TestMethod]
        public void Forgot_Password_Shows_Success_Email_Found()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/SignIn");
                driver.FindElement(By.PartialLinkText("Forgot")).Click();
                driver.FindElement(By.CssSelector("#forgotForm input[name='Email']")).SendKeys("member2@honeypot.com");
                driver.FindElement(By.CssSelector("#forgotForm .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-success")).Count > 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".alert-success").Count);
            }
        }

        [TestMethod]
        public void User_Num_Per_Page_Shows_Records()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(u => u.Url.Contains("Manage/User"));
                driver.FindElement(By.CssSelector("a.select2-choice")).Click();
                driver.FindElement(By.CssSelector(".select2-result-selectable:nth-child(4)")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > DefaultNumPerPage);
                Assert.AreEqual(100, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Search_Users_By_Email_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".searchTerm")).SendKeys("member8@honeypot.com");
                driver.FindElement(By.CssSelector(".table-filter button")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Search_Users_By_Advanced_Email_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""Email""]').val('member8@honeypot.com')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Role_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('select[name=""Role""]').val('Administrator')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var rows = driver.FindElementsByCssSelector(".table tbody tr");
                var filtered = true;
                foreach (var el in rows)
                {
                    var td = el.FindElement(By.CssSelector("td:nth-child(3)"));
                    if (td.Text != "Administrator") filtered = false;
                }
                Assert.IsTrue(filtered);
            }
        }

        [TestMethod]
        public void Paging_Users_Shows_Next_page()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".page-2")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table .user-row-122")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".table .user-row-122")));
            }
        }

        [TestMethod]
        public void User_SignIn_Shows_Dashboard()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                Assert.IsTrue(driver.Url.Contains("Manage/Dashboard"));
            }
        }

        [TestMethod]
        public void SignOut_Displays_Message()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElementByLinkText("Sign Out").Click();
                driver.WaitFor(d => d.Url.Contains("User/SignIn"));
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-info")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".alert-info")));
            }
        }

        [TestMethod]
        public void User_SignUp_Successful()
        {
            using (var driver = BrowserFactory.Create())
            {
                driver.Navigate().GoToUrl(BaseUrl + "/User/SignUp");
                driver.FindElementByLinkText("Sign Up").Click();
                driver.WaitFor(d => d.FindElements(By.Name("Email")).Count > 0);
                driver.FindElementByName("Email").SendKeys("uitest" + DateTime.Now.Ticks + "@honeypot.com");
                driver.FindElementByName("Password").SendKeys("test");
                driver.FindElementByName("ConfirmPassword").SendKeys("test");
                driver.FindElementByCssSelector("#signUpForm .btn-primary").Click();
                driver.WaitFor(d => d.Url.Contains("Dashboard"));
                Assert.IsTrue(driver.Url.Contains("Dashboard"));
            }
        }

        [TestMethod]
        public void Admin_Can_View_User_Details()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".user-row-300 a[title='View user']")).Click();
                driver.WaitFor(u => u.FindElements(By.CssSelector("#viewUserForm .form-horizontal")).Count > 0);
                Assert.IsTrue(driver.FindElementByCssSelector("#viewUserForm").Text.Contains("300"));
            }
        }

        [TestMethod]
        public void Admin_Can_Delete_User()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Users")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".user-row-10 a[title='Delete user']")).Click();
                driver.WaitFor(u => u.FindElements(By.CssSelector(".modal-footer")).Count > 0);
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(u => u.FindElements(By.CssSelector(".alert-info")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".alert-info")));
            }
        }

        [TestMethod]
        public void Edit_Profile_Successful_No_Change()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElementByLinkText("Profile").Click();
                driver.WaitFor(d => d.FindElements(By.Name("Email")).Count > 0);
                driver.FindElementByCssSelector("#editProfileForm .btn-primary").Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-success")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".alert-success")));
            }
        }

        [TestMethod]
        public void Edit_Profile_Shows_Message_On_Error()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElementByLinkText("Profile").Click();
                driver.WaitFor(d => d.FindElements(By.Name("Email")).Count > 0);
                driver.FindElementByName("Password").SendKeys("test");
                driver.FindElementByName("ConfirmPassword").SendKeys("testBAD");
                driver.FindElementByCssSelector("#editProfileForm .btn-primary").Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".alert-error")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".alert-error")));
            }
        }
    }
}
