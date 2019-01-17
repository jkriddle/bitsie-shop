using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Bitsie.Shop.Domain;
using Mustache;
using System.IO;
using System.Security.Permissions;

namespace Bitsie.Shop.Services
{
    /// <summary>
    /// Notification service that sends emails to users.
    /// </summary>
    public class EmailNotificationService : INotificationService
    {
        #region Fields

        /// <summary>
        /// Address which is used to send all notifications. Also used
        /// as the reply-to address.
        /// </summary>
        private readonly string _fromEmail;

        /// <summary>
        /// Name which is used for the email that sends all notifications.
        /// </summary>
        private readonly string _fromName;

        private readonly IEmailService _emailService;
        private readonly ILogService _logService;
        private readonly string _templateDirectory;

        #endregion

        public EmailNotificationService(
            ILogService logService,
            IEmailService emailService,
            string templateDirectory,
            string fromEmail,
            string fromName)
        {
            _fromEmail = fromEmail;
            _fromName = fromName;
            _emailService = emailService;
            _logService = logService;
            _templateDirectory = templateDirectory;
        }
        
        /// <summary>
        /// Send an email to the specified user
        /// </summary>
        /// <param name="toAddress">Recipient of the email</param>
        /// <param name="template">Template used for email</param>
        /// <param name="messageParams">Values for placeholders used in template to replace with.</param>
        public async Task<bool> Notify(string toAddress, string subject, string template, object messageParams)
        {
            var message = new MailMessage(new MailAddress(_fromEmail, _fromName),
                new MailAddress(toAddress));

            message.Subject = subject;

            try
            {
                string fullPath = System.IO.Path.Combine(_templateDirectory, template + ".html");
                FormatCompiler compiler = new FormatCompiler();
                string content = System.IO.File.ReadAllText(fullPath);
                Generator generator = compiler.Compile(content);    
                string body = generator.Render(messageParams);
                generator = compiler.Compile(System.IO.File.ReadAllText(System.IO.Path.Combine(_templateDirectory, "Email.html")));
                message.Body = generator.Render(body); 
                message.IsBodyHtml = true;

                _emailService.Send(message);

                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Level = LogLevel.Info,
                    Message = string.Format("Email notification to {0} successful: {1}", message.To, message.Subject),
                    Details = message.Body
                });
            }
            catch (SmtpException ex)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Level = LogLevel.Error,
                    Message = string.Format("Email notification to {0} failed: {1}", message.To, message.Subject),
                    Details = ex.Message + "\r\n" + ex.StackTrace + " \r\n Original Email: " + message.Body
                });
            }

            await Task.Yield();
            return true;
        }

    }
}
