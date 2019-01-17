using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using System.Collections.Generic;

namespace Bitsie.Shop.Services
{
    public interface IInvoiceService
    {
        void CreateInvoice(Invoice invoice);
        Invoice GetInvoiceById(int id);
        IPagedList<Invoice> GetInvoices(InvoiceFilter filter, int currentPage, int numPerPage);
        Invoice GetInvoiceByNumber(string id, int merchantId);
        void UpdateInvoice(Invoice invoice);
        IList<Invoice> GetOpenInvoices();
        Invoice GetInvoiceByGuid(string guid);
        void Save(Invoice invoice);
        void SaveInvoiceItems(List<InvoiceItem> invoiceItems);
        void RemoveInvoiceItems(Invoice invoice);         
        void SendInvoice(int id);        
        bool ValidateInvoice(Invoice invoice, IValidationDictionary validationDictionary);
    }
}
