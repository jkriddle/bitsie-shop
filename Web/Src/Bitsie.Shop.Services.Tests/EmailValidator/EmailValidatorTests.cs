using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class EmailValidatorTests
    {
        [TestMethod]
        public void Return_True_For_Valid_Email()
        {
           // Arrange
            string email = "valid@email.com";

            // Act
            var result = EmailValidator.IsValid(email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Return_False_For_Invalid_Email()
        {
            // Arrange
            string email = "invalid";

            // Act
            var result = EmailValidator.IsValid(email);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
