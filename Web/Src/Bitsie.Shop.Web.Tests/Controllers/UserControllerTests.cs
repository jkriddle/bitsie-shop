using System;
using System.Web.Mvc;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Controllers;
using Bitsie.Shop.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bitsie.Shop.Web.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void User_SignUp_Returns_View()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (ViewResult)controller.SignUp();

            // Assert
            Assert.AreEqual(result.ViewName, "SignUp");
        }

        [TestMethod]
        public void User_SignIn_Returns_View()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (ViewResult)controller.SignIn();

            // Assert
            Assert.AreEqual(result.ViewName, "SignIn");
        }

        [TestMethod]
        public void User_SignOut_Redirects_To_Login()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = controller.SignOut();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void User_SignOut_Displays_Message()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (RedirectToRouteResult)controller.SignOut();

            // Assert
            Assert.AreNotEqual(String.Empty, result.RouteValues["message"]);
        }

    }
}
