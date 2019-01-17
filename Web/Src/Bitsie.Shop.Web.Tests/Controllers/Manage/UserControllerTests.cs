using System.Web.Mvc;
using Bitsie.Shop.Web.Areas.Manage.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bitsie.Shop.Services;
using Moq;
using Bitsie.Shop.Web.Providers;

namespace Bitsie.Shop.Web.Tests.Controllers.Manage
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void User_Index_Returns_View()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var auth = new Mock<IAuth>();
            var controller = new UserController(userService.Object, auth.Object);

            // Act
            var result = (ViewResult)controller.Index();

            // Assert
            Assert.AreEqual(result.ViewName, "Index");
        }


        [TestMethod]
        public void User_Edit_Returns_View()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var auth = new Mock<IAuth>();
            var controller = new UserController(userService.Object, auth.Object);

            // Act
            var result = (ViewResult)controller.Edit();

            // Assert
            Assert.AreEqual(result.ViewName, "Edit");
        }


        [TestMethod]
        public void User_View_Returns_View()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var auth = new Mock<IAuth>();
            var controller = new UserController(userService.Object, auth.Object);

            // Act
            var result = (ViewResult)controller.Details();

            // Assert
            Assert.AreEqual(result.ViewName, "Details");
        }
    }
}
