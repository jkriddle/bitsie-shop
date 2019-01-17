using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Api.Models
{
    public class InvoiceListViewModel : PagedViewModel<Invoice>
    {
        #region Constructor

        public InvoiceListViewModel(IPagedList<Invoice> invoices)
            : base(invoices)
        {
            Invoices = new List<InvoiceViewModel>();
            foreach (var invoice in invoices.Items)
            {
                Invoices.Add(new InvoiceViewModel(invoice));
            }
        }

        #endregion

        #region Public Properties

        public IList<InvoiceViewModel> Invoices { get; set; }

        #endregion

    }
}