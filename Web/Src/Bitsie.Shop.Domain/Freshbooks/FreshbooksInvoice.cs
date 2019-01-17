using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class FreshbooksInvoice
    {
        public ulong? InvoiceId { get; set; }
        public double Amount { get; set; }
        public ulong? ClientId { get; set; }
        public string Status { get; set; }
        public string LastName { get; set; }

        public bool IsAvailable
        {
            get
            {
                //‘disputed’, ‘draft’, ‘sent’, ‘viewed’, ‘paid’, ‘auto-paid’, ‘retry’, ‘failed’ or the special status ‘unpaid’ which will retrieve all invoices with a status of ‘disputed’, ‘sent’, ‘viewed’, ‘retry’ or ‘failed’.
                return Status != "paid" && Status != "auto-paid" && Status != "disputed" && Status != "retry";
            }
        }
    }
}
