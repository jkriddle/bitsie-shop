using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Bitsie.Shop.Web.UI.Tests
{
    public class ModalHelper
    {
        public static void ClickPrimary(RemoteWebDriver driver)
        {

            // Unable to click the Okay button due to chrome driver issue.
            // @see http://code.google.com/p/selenium/issues/detail?id=2766
            // Just run direct JS
            (driver as IJavaScriptExecutor).ExecuteScript("$('.modal-footer .btn-primary').trigger('click')");
        }
    }
}
