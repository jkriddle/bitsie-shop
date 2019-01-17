using System.Net.Mail;

namespace Bitsie.Shop.Services
{
    public interface IEmailService
    {
        void Send(MailMessage message);
    }
}
