using System;
using System.Collections.Generic;
using System.Linq;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Bitsie.Shop.Api;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class UserServiceTests
    {

        private IUserService CreateUserService(IUserRepository userRepository)
        {
            var configService = new Mock<IConfigService>();
            configService.Setup(c => c.AppSettings("NotificationTemplatePath")).Returns("/fake/path");
            var permissionRepository = new Mock<IPermissionRepository>();
            var authTokenRepository = new Mock<IAuthTokenRepository>();
            var notificationService = new Mock<INotificationService>();
            var offlineAddressRepository = new Mock<IOfflineAddressRepository>();
            var bitcoinService = new Mock<IBitcoinService>();
            var walletApi = new Mock<IWalletApi>();
            var logService = new Mock<ILogService>();
            var subscriptionRepository = new Mock<ISubscriptionRepository>();
            return new UserService(configService.Object, 
                userRepository, 
                permissionRepository.Object, 
                authTokenRepository.Object,
                walletApi.Object,
                logService.Object,
                bitcoinService.Object,
                offlineAddressRepository.Object,
                notificationService.Object, 
                subscriptionRepository.Object);
        }

        #region Authenticate

        [TestMethod]
        public void Authenticate_Successful_With_Valid_User()
        {
            // Arrange
            var user = new User
                {
                    Email = "test@honeypot.com",
                    Status = UserStatus.Active
                };
            var users = new List<User>()
                {
                    user
                };
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(users.AsQueryable());
            var userService = CreateUserService(userRepository.Object);
            userService.GenerateUserPassword(user, "test");

            // Act
            var result = userService.Authenticate(user.Email, "test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Authenticate_Fails_With_Deleted_User()
        {
            // Arrange
            var user = new User
            {
                Email = "test@honeypot.com",
                Status = UserStatus.Suspended
            };
            var users = new List<User>()
                {
                    user
                };
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(users.AsQueryable());
            var userService = CreateUserService(userRepository.Object);
            userService.GenerateUserPassword(user, "test");

            // Act
            var result = userService.Authenticate(user.Email, "test");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GenerateResetToken_Creates_Token()
        {
            // Arrange
            var user = new User
            {
                Email = "test@honeypot.com",
                ResetPasswordToken = "1234"
            };
            var users = new List<User>()
                {
                    user
                };
            var originalToken = user.ResetPasswordToken;
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(users.AsQueryable());
            var userService = CreateUserService(userRepository.Object);

            // Act
            userService.GenerateResetRequest(user);

            // Assert
            Assert.AreNotEqual(originalToken, user.ResetPasswordToken);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ResetPassword_Fails_With_Empty_Pass()
        {
            // Arrange
            var user = new User
            {
                Email = "test@honeypot.com",
            };
            var users = new List<User>()
                {
                    user
                };
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(users.AsQueryable());
            var userService = CreateUserService(userRepository.Object);

            // Act
            userService.ResetPassword(user, "");

            // Asserted by attribute
        }

        [TestMethod]
        public void ResetPassword_Changes_Password()
        {
            // Arrange
            var user = new User
            {
                Email = "test@honeypot.com",
            };
            var users = new List<User>()
                {
                    user
                };

            var originalPass = user.HashedPassword;
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(users.AsQueryable());
            var userService = CreateUserService(userRepository.Object);

            // Act
            userService.ResetPassword(user, "newpass");

            // Assert
            Assert.AreNotEqual(originalPass, user.HashedPassword);
        }

        #endregion

        #region ValidateUser

        [TestMethod]
        public void Validation_Success_With_Valid_Info()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var userService = CreateUserService(userRepository.Object);
            var validationDictionary = new ValidationDictionary();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");
            user.SetupProperty(u => u.HashedPassword, new byte[] { 0, 1, 2 });

            // Act
            var result = userService.ValidateUser(user.Object, validationDictionary);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void User_Validation_Fails_Missing_Email()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var userService = CreateUserService(userRepository.Object);
            var validationDictionary = new ValidationDictionary();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.HashedPassword, new byte[] { 0, 1, 2 });

            // Act
            var result = userService.ValidateUser(user.Object, validationDictionary);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void User_Validation_Fails_Invalid_Email()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var userService = CreateUserService(userRepository.Object);
            var validationDictionary = new ValidationDictionary();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "bademail");
            user.SetupProperty(u => u.HashedPassword, new byte[] { 0, 1, 2 });

            // Act
            var result = userService.ValidateUser(user.Object, validationDictionary);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void User_Validation_Fails_Missing_Password()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var userService = CreateUserService(userRepository.Object);
            var validationDictionary = new ValidationDictionary();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");

            // Act
            var result = userService.ValidateUser(user.Object, validationDictionary);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region UpdateUser

        [TestMethod]
        public void Update_Failed_Email_In_Use()
        {
            // Arrange
            var existing = new Mock<User>();
            existing.SetupProperty(u => u.Id, 2);
            existing.SetupProperty(u => u.Email, "test@honeypot.com");
            existing.SetupProperty(u => u.HashedPassword, new byte[] { 0, 1, 2 });
            var userList = new List<User>()
                {
                    existing.Object
                };

            var userRepository = new Mock<IUserRepository>();
            var permissionRepository = new Mock<IPermissionRepository>();
            userRepository.Setup(u => u.FindAll()).Returns(userList.AsQueryable());
            var userService = CreateUserService(userRepository.Object);
            var validationDictionary = new ValidationDictionary();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Id, 1);
            user.SetupProperty(u => u.Email, "test@honeypot.com");
            user.SetupProperty(u => u.HashedPassword, new byte[] { 0, 1, 2 });

            // Act
            var result = userService.ValidateUser(user.Object, validationDictionary);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

    }
}
