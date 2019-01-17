using System;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class InvoiceListInputModel : PagedInputModel
    {
        /// <summary>
        /// Filter invoices by a specific ID
        /// </summary>
        public int? InvoiceId { get; set; }

        /// <summary>
        /// Filter invoices by a specific Merchant
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Filter invoices by a specific status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Filter invoices after a specific start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter invoices before a specific end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        public string CustomerLastName { get; set; }

        public string View { get; set; }
    }
}