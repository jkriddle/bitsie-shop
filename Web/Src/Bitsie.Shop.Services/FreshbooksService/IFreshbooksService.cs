using Bitsie.Shop.Domain;
using System;
namespace Bitsie.Shop.Services
{
    public interface IFreshbooksService
    {
        void SetAccount(User user);
        void AuthorizeToken(Uri url);
        void AuthorizeToken(string token);
        Uri GetAuthorizationUrl(string redirectUrl);
        string GetTokenState();
        FreshbooksInvoice GetInvoiceById(ulong id);
        FreshbooksInvoice GetInvoiceByNumber(string number);
    }
}
