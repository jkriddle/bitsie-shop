
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Bitsie.Shop.Web.UI.Tests
{
    /// <summary>
    /// Helper for the generation of various browsers.
    /// </summary>
    public static class BrowserFactory
    {
        public static RemoteWebDriver Create()
        {
            // @todo - make this dynamic to test in multiple browseres
            return new ChromeDriver();
        }
    }
}
