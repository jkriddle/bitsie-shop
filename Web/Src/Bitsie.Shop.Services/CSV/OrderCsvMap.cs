using Bitsie.Shop.Domain;
using CsvHelper.Configuration;
namespace Bitsie.Shop.Services.CSV
{
    public sealed class OrderCsvMap : CsvClassMap<Order>
    {
        public OrderCsvMap()
        {
            Map(m => m.OrderNumber).Name("Order Number");
            Map(m => m.OrderDate).Name("Order Date");
            Map(m => m.Description).Name("Reference/Description");
            Map(m => m.Subtotal);
            Map(m => m.Gratuity);
            Map(m => m.Total);
            Map(m => m.BtcTotal).Name("BTC Amount");
            Map(m => m.BtcPaid).Name("BTC Paid");
            Map(m => m.Status).Name("Status");
            Map(m => m.Rate).Name("Exchange Rate");
            Map(m => m.OrderType).Name("Order Type");
        }
    }
}
