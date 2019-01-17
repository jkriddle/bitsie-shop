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
    public class LogControllerTests
    {
        #region

        private Log CreateLog(int id)
        {
            var log = new Mock<Log>();
            log.SetupProperty(l => l.Id, id);
            return log.Object;
        }

        private LogController CreateLogController(ILogService logService)
        {
            var userService = new Mock<IUserService>();
            var mapper = new Mock<IMapperService>();
            var controller = new LogController(userService.Object, logService, mapper.Object);
            return controller;
        }

        #endregion

        #region Get

        [TestMethod]
        public void GetOne_Returns_Log()
        {
            // Arrange
            var log = CreateLog(1);
            var logService = new Mock<ILogService>();
            logService.Setup(l => l.GetLogById(It.IsAny<int>())).Returns(log);
            var controller = CreateLogController(logService.Object);

            // Act
            var result = controller.GetOne(1);

            // Assert
            Assert.AreEqual(log.Id, result.LogId);
        }

        #endregion

    }
}

