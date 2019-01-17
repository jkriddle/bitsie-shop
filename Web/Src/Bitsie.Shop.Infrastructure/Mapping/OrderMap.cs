using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class OrderMap : IAutoMappingOverride<Order>
    {
        public void Override(AutoMapping<Order> mapping)
        {
            mapping.Table("Orders");
            mapping.Id(x => x.Id, "OrderID");
            mapping.Map(x => x.OrderNumber);
            mapping.Map(x => x.Description).Nullable();
            mapping.Map(x => x.PaymentAddress).Not.Nullable();
            mapping.Map(x => x.OrderDate).Not.Nullable();
            mapping.Map(x => x.Subtotal).Precision(9).Scale(2).Not.Nullable();
            mapping.Map(x => x.Gratuity).Precision(9).Scale(2).Not.Nullable();
            mapping.Map(x => x.Total).Precision(9).Scale(2).Not.Nullable();
            mapping.Map(x => x.OrderType).Not.Nullable().CustomType<OrderType>();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<OrderStatus>();
            mapping.Map(x => x.BtcTotal).Precision(18).Scale(9).Not.Nullable();
            mapping.Map(x => x.BtcPaid).Precision(18).Scale(9).Not.Nullable();
            mapping.Map(x => x.ExchangeRate).Precision(9).Scale(2).Not.Nullable();
            mapping.Map(x => x.Status).Not.Nullable();
            mapping.Map(x => x.FreshbooksInvoiceId).Nullable();
            mapping.Map(x => x.FreshbooksPaymentId).Nullable();
            mapping.HasMany(x => x.Transactions);
            mapping.References(x => x.Invoice).Column("InvoiceId").Nullable();
            mapping.References(x => x.User).Column("UserID");
            mapping.References(x => x.Payout).Column("PayoutID");
            mapping.References(x => x.Product).Column("ProductID");
            mapping.References(x => x.Subscription).Column("SubscriptionID");
            mapping.References(x => x.Customer).Column("CustomerID");
        }
    }
}
