using System.Threading.Tasks;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Services
{
    public interface INotificationService
    {
        Task<bool> Notify(string toAddress, string subject, string templatePath, object messageParams);
    }
}
