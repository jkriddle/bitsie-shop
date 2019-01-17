using System.Collections.Generic;
using System.Net.Mail;
using System.Xml.Linq;
using Bitsie.Shop.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class EmailNotificationServiceTests
    {

        #region Notify

        [TestMethod]
        public void Notify_Creates_Log_On_Success()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var emailService = new Mock<IEmailService>();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Email, "recipient@honeypot.com");
           
            var service = new EmailNotificationService(logService.Object, emailService.Object,
                "",
                "test@honeypot.com", "Support");

            // Act
            service.Notify(user.Object.Email, "", "Template", null);

            // Assert
            logService.Verify(l => l.CreateLog(It.IsAny<Domain.Log>()), Times.Once());
        }

        [TestMethod]
        public void Notify_Creates_Log_On_Failure()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var emailService = new Mock<IEmailService>();
            emailService.Setup(e => e.Send(It.IsAny<MailMessage>())).Throws(new SmtpException());
            var user = new Mock<User>();
            user.SetupProperty(u => u.Email, "recipient@honeypot.com");

            var service = new EmailNotificationService(logService.Object, emailService.Object,
                "",
                "test@honeypot.com", "Support");

            // Act
            service.Notify(user.Object.Email, "", "", null);

            // Assert
            logService.Verify(l => l.CreateLog(It.IsAny<Domain.Log>()), Times.Once());
        }

        #endregion


    }
}
