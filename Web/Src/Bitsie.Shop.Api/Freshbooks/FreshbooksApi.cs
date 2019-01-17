using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class FreshbooksApi : Freshbooks.Library.FreshbooksApi, IFreshbooksApi
    {
        public FreshbooksApi(string customerAccount, string developerAccount)
            : base(customerAccount, developerAccount)
        {
        }

        public FreshbooksApi(string accountName, string consumerKey, string oauthSecret)
            : base(accountName, consumerKey, oauthSecret)
        {
        }

        public FreshbooksInvoice GetInvoiceByNumber(string number)
        {
            var request = new Freshbooks.Library.Model.InvoicesRequest
            {
                Number = number
            };

            var invoiceResponse = this.Invoice.List(request);

            if (invoiceResponse.Invoices.InvoiceList.Count == 0) throw new Exception("Invoice number " + number + " not found.");

            if (invoiceResponse.Invoices.InvoiceList.Count > 1) throw new Exception("Multiple invoices with number " + number + " found.");

            var invoice = invoiceResponse.Invoices.InvoiceList.First();

            return new FreshbooksInvoice
            {
                InvoiceId = invoice.InvoiceId.HasValue ? invoice.InvoiceId.Value : (ulong?)null,
                ClientId = invoice.ClientId.HasValue ? invoice.ClientId.Value : (ulong?)null,
                Amount = invoice.Amount,
                Status = invoice.Status,
                LastName = invoice.LastName
            };
        }

        public FreshbooksInvoice GetInvoiceById(ulong id)
        {
            var invoiceIdentity = new Freshbooks.Library.Model.InvoiceIdentity();
            invoiceIdentity.InvoiceId = new Freshbooks.Library.Model.InvoiceId(id);
            var invoiceResponse = this.Invoice.Get(invoiceIdentity);
            return new FreshbooksInvoice {
                InvoiceId = invoiceResponse.Invoice.InvoiceId.HasValue ? invoiceResponse.Invoice.InvoiceId.Value : (ulong?)null,
                ClientId = invoiceResponse.Invoice.ClientId.HasValue ? invoiceResponse.Invoice.ClientId.Value : (ulong?)null,
                Amount = invoiceResponse.Invoice.Amount,
                Status = invoiceResponse.Invoice.Status,
                LastName = invoiceResponse.Invoice.LastName
            };
        }

        /// <summary>
        /// Add a payment to freshbooks
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ulong AddPayment(Order order)
        {
            var request = new Freshbooks.Library.Model.PaymentRequest
            {
                Payment = new Freshbooks.Library.Model.Payment
                {
                    InvoiceId = new Freshbooks.Library.Model.InvoiceId(Convert.ToUInt64(order.FreshbooksInvoiceId.Value)),
                    Amount = Convert.ToDouble(order.Total),
                    Notes = "Bitsie Shop order " + order.OrderNumber,
                    Type = "Cash"
                }
            };

            var response = this.Payment.Create(request);
            return response.PaymentId.Value;
        }

        /// <summary>
        /// Retrieve current OAuth token state
        /// </summary>
        /// <returns></returns>
        public string GetTokenState()
        {
            return this.TokenState;
        }

        /// <summary>
        /// Set OAuth token state
        /// </summary>
        /// <param name="tokenState"></param>
        /// <returns></returns>
        public void SetTokenState(string tokenState)
        {
            this.TokenState = tokenState;
        }

        /// <summary>
        /// Set the URL which the user will be redirected to after completing OAuth process
        /// </summary>
        /// <param name="redirectUrl">URL to redirect back to</param>
        public void SetRedirectUrl(string redirectUrl)
        {
            this.OAuthCallback = redirectUrl;
        }
    }
}
