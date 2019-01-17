using System.Net.Mail;

namespace Bitsie.Shop.Services
{
    public class EmailService : IEmailService
    {
        public void Send(MailMessage message)
        {
            var smtp = new SmtpClient();
            smtp.Send(message);
        }
    }
}
