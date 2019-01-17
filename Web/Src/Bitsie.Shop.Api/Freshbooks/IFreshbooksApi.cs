using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public interface IFreshbooksApi
    {
        void AuthorizeToken(Uri url);
        void AuthorizeToken(string token);
        Uri GetAuthroizationUrl();
        string GetTokenState();
        void SetTokenState(string tokenState);
        void SetRedirectUrl(string redirectUrl);
        void UseLegacyToken(string token);
        FreshbooksInvoice GetInvoiceById(ulong id);
        FreshbooksInvoice GetInvoiceByNumber(string number);
        ulong AddPayment(Order order);
    }
}
