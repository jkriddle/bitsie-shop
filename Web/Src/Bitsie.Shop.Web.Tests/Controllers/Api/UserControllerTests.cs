using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http.Controllers;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Areas.Api.Controllers;
using Bitsie.Shop.Web.Areas.Api.Models;
using Bitsie.Shop.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bitsie.Shop.Web.Tests.Controllers.Api
{
    [TestClass]
    public class UserControllerTests
    {

        private UserController CreateController(IUserService userService)
        {
            var logService = new Mock<ILogService>();
            var mapper = new Mock<IMapperService>();
            var users = new List<User>()
                {
                    new User
                        {
                            Email = "test@honeypot.com"
                        }
                };
            var auth = new Mock<IAuth>();
            var configService = new Mock<IConfigService>();
            var subscriptionService = new Mock<ISubscriptionService>();
            var controller = new UserController(auth.Object, userService, logService.Object, configService.Object, subscriptionService.Object, mapper.Object);
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://localhost", ""),
                new HttpResponse(new StringWriter())
            );
            controller.ControllerContext = new HttpControllerContext();
            return controller;
        }

        #region Get

        [TestMethod]
        public void GetOne_Returns_Populated_User_View_Model()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(user.Object);
            var controller = CreateController(userService.Object);
            var currentUser = new Mock<User>();
            currentUser.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(true);
            controller.CurrentUser = currentUser.Object;

            // Act
            var result = controller.GetOne(1);

            // Assert
            Assert.AreEqual(user.Object.Id, result.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void GetOne_Throws_Exception_When_Not_Found()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object);

            // Act
            var result = controller.GetOne(1);

            // Asserted by attribute
        }

        [TestMethod]
        public void Get_Users_Returns_User_List_View_Model()
        {
            // Arrange
            var users = new List<User>()
                {
                    new User()
                };
            var userList = new PagedList<User>(users, 1, 20);
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUsers(It.IsAny<UserFilter>(), It.IsAny<int>(), It.IsAny<int>())).Returns(userList);
            var controller = CreateController(userService.Object);

            // Act
            var result = controller.Get(new UserListInputModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UserListViewModel));
        }

        #endregion

        #region Sign Up

        [TestMethod]
        public void SignUp_Successful_For_Valid_User()
        {
            // Arrange
            var inputModel = new SignUpInputModel
            {
                Email = "testuser@honeypot.com",
                Password = "test",
                ConfirmPassword = "test"
            };
            var user = new User()
                {
                    Email = inputModel.Email,
                    AuthToken = new AuthToken()
                };
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(user);
            userService.Setup(u => u.ValidateUser(It.IsAny<User>(), It.IsAny<IValidationDictionary>())).Returns(true);
            var controller = CreateController(userService.Object);

            // Act
            var result = controller.SignUpStart(inputModel);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void SignUp_Failed_For_Invalid_User()
        {

            // Arrange
            var inputModel = new SignUpInputModel
            {
            };
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object);

            // Act
            var result = controller.SignUpStart(inputModel);

            // Assert
            Assert.IsFalse(result.Success);
        }

        #endregion

        #region Sign In

        [TestMethod]
        public void SignIn_Success_For_Valid_User()
        {
            // Arrange
            const string email = "valid@email.com";
            const string password = "test";
            var user = new User
                {
                    Email = email,
                    AuthToken = new AuthToken
                        {
                            Token = "12345",
                            Expires = DateTime.UtcNow
                        }
                };
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(user);
            var controller = CreateController(userService.Object);
            var currentUser = new Mock<User>();
            currentUser.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(true);
            controller.CurrentUser = currentUser.Object;

            // Act
            BaseResponseModel result = controller.SignIn(new SignInInputModel
            {
                Email = email,
                Password = password
            });

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void SignIn_Failure_For_Invalid_User()
        {
            // Arrange
            const string email = "valid@email.com";
            const string password = "test";
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns((User) null);
            var controller = CreateController(userService.Object);

            // Act
            BaseResponseModel result = controller.SignIn(new SignInInputModel
                {
                    Email = email,
                    Password = password
                });

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void SignOut_Successful()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object); 
            
            // Act
            var result = controller.SignOut();

            // Assert
            Assert.IsTrue(result.Success);
        }

        #endregion

        #region Update User

        [TestMethod]
        public void Update_Successful_Valid_Fields()
        {
            // Arrange
            var existing = new Mock<User>();
            existing.SetupProperty(u => u.Id, 1);
            existing.SetupProperty(u => u.Email, "testupdate@honeypot.com");
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserById(1)).Returns(existing.Object);
            userService.Setup(u => u.ValidateUser(It.IsAny<User>(), It.IsAny<IValidationDictionary>())).Returns(true);
            var currentUser = new Mock<User>();
            currentUser.SetupProperty(u => u.Id, 1);
            currentUser.SetupProperty(u => u.Email, "testupdate@honeypot.com");
            var controller = CreateController(userService.Object); 
            controller.CurrentUser = currentUser.Object;

            // Act
            var result = controller.Update(new UpdateUserInputModel
                {
                    UserId = 1,
                    Email = "testupdate@honeypot.com"
                });

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Update_Failed_Missing_Id()
        {
            // Arrange
            var mapper = new Mock<IMapperService>();
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object); 

            // Act
            controller.Update(new UpdateUserInputModel
            {
                UserId = 1,
                Email = "testupdate@honeypot.com",
                Password = "goodpass",
                ConfirmPassword = "badpass"
            });

            // Asserted by attribute
        }

        [TestMethod]
        public void Update_Failed_NonMatching_Passwords()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(new User());
            var controller = CreateController(userService.Object); 
            controller.CurrentUser = new User();

            // Act
            var result = controller.Update(new UpdateUserInputModel
            {
                UserId = 1,
                Email = "testupdate@honeypot.com",
                Password = "goodpass",
                ConfirmPassword = "badpass"
            });

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Update_Failed_When_User_Not_Found()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object); 

            // Act
            var result = controller.Update(new UpdateUserInputModel
            {
                UserId = 0,
                Email = "testupdate@honeypot.com"
            });

            // Asserted by attribute
        }


        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Update_Failed_No_Permissions()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object); 

            // Act
            var result = controller.Update(new UpdateUserInputModel
            {
                UserId = 0,
                Email = "testupdate@honeypot.com"
            });

            // Asserted by attribute
        }

        #endregion

        #region "Delete"

        [TestMethod]
        public void Delete_User_Successful_When_User_Exists()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Role, Role.Administrator);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(user.Object);
            var controller = CreateController(userService.Object);
            var currentUser = new Mock<User>();
            currentUser.SetupProperty(u => u.Id, 1);
            currentUser.SetupProperty(u => u.Email, "test@honeypot.com");
            currentUser.SetupProperty(u => u.Role, Role.Administrator);
            currentUser.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(true);
            controller.CurrentUser = currentUser.Object;

            // Act
            var result = controller.Delete(user.Object.Id);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Delete_User_Fails_When_User_Does_Not_Exist()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Role, Role.Administrator);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object); 

            // Act
            controller.Delete(2);

            // Asserted by attribute
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Delete_User_Fails_No_Permission()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            var controller = CreateController(userService.Object);

            // Act
            controller.Delete(1);

            // Asserted by attribute
        }

        [TestMethod]
        public void Forgot_Password_Successful_For_Matching_User()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserByEmail("test@honeypot.com")).Returns(user.Object);
            var controller = CreateController(userService.Object);

            // Act
            var vm = controller.ForgotPassword(new ForgotPasswordInputModel
                {
                    Email = "test@honeypot.com"
                });

            // Assert
            Assert.IsTrue(vm.Success);
        }

        [TestMethod]
        public void Forgot_Password_Generates_Reset_Link()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserByEmail("test@honeypot.com")).Returns(user.Object);
            var controller = CreateController(userService.Object);

            // Act
            var vm = controller.ForgotPassword(new ForgotPasswordInputModel
            {
                Email = "test@honeypot.com"
            });

            // Assert
            userService.Verify(u => u.GenerateResetRequest(It.IsAny<User>()), Times.Once());
        }

        [TestMethod]
        public void Forgot_Password_Missing_User_Not_Successul()
        {
            // Arrange
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            var userService = new Mock<IUserService>();
            userService.Setup(u => u.GetUserByEmail("test@honeypot.com")).Returns(user.Object);
            var controller = CreateController(userService.Object);

            // Act
            var vm = controller.ForgotPassword(new ForgotPasswordInputModel
            {
                Email = "missing@honeypot.com"
            });

            // Assert
            Assert.IsFalse(vm.Success);
        }

        #endregion

    }
}

