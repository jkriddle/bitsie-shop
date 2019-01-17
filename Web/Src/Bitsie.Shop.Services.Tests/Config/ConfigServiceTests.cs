using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class ConfigServiceTests
    {
        [TestMethod]
        public void AppSettings_Returns_Stored_Value()
        {
           // Arrange
            var values = new NameValueCollection();
            values.Add("test", "penguin");
            var configService = new ConfigService(values);

            // Act
            var result = configService.AppSettings("test");

            // Assert
            Assert.AreEqual("penguin", result);
        }
    }
}
