using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class LogServiceTests
    {
        #region Helper Methods

        private Log CreateLog(int id)
        {
            var log = new Mock<Log>();
            log.SetupProperty(l => l.Id, id);
            return log.Object;
        }

        private LogService CreateLogService(ILogRepository logRepository)
        {
            return new LogService(logRepository, LogLevel.Info);
        }

        #endregion

        #region Create

        [TestMethod]
        public void CreateLog_Creates_If_Above_Threshold()
        {
            // Arrange
            var log = new Log()
            {
                Level = LogLevel.Warning
            };
            var logRepository = new Mock<ILogRepository>();
            logRepository.Setup(r => r.Save(It.IsAny<Log>())).Verifiable();
            var logService = new LogService(logRepository.Object, LogLevel.Info);

            // Act
            logService.CreateLog(log);

            // Assert
            logRepository.Verify(l => l.Save(log), Times.Once());
        }

        [TestMethod]
        public void CreateLog_Creates_If_Equal_To_Threshold()
        {
            // Arrange
            var log = new Log()
            {
                Level = LogLevel.Info
            };
            var logRepository = new Mock<ILogRepository>();
            logRepository.Setup(r => r.Save(It.IsAny<Log>())).Verifiable();
            var logService = new LogService(logRepository.Object, LogLevel.Info);

            // Act
            logService.CreateLog(log);

            // Assert
            logRepository.Verify(l => l.Save(log), Times.Once());
        }

        [TestMethod]
        public void CreateLog_Does_Not_Create_If_Below_Threshold()
        {
            // Arrange
            var log = new Log()
            {
                Level = LogLevel.Info
            };
            var logRepository = new Mock<ILogRepository>();
            logRepository.Setup(r => r.Save(It.IsAny<Log>())).Verifiable();
            var logService = new LogService(logRepository.Object, LogLevel.Warning);

            // Act
            logService.CreateLog(log);

            // Assert
            logRepository.Verify(l => l.Save(log), Times.Never());
        }
        
        #endregion

        #region Get

        [TestMethod]
        public void GetLogById_Retrieves_Log_With_Matching_Id()
        {
            // Arrange
            var log = CreateLog(1);
            var logRepository = new Mock<ILogRepository>();
            logRepository.Setup(r => r.FindOne(It.IsAny<int>())).Returns(log);
            var logService = CreateLogService(logRepository.Object);

            // Act
            var result = logService.GetLogById(1);

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        #endregion

    }
}
