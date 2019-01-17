using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class InvoiceMap : IAutoMappingOverride<Invoice>
    {
        public void Override(AutoMapping<Invoice> mapping)
        {
            mapping.Table("Invoices");
            mapping.Id(x => x.Id, "InvoiceID");
            mapping.References(x => x.Customer).Column("CustomerID").Cascade.All();
            mapping.References(x => x.Merchant).Column("MerchantID").Cascade.All();
            mapping.Map(x => x.USDAmount).Not.Nullable();
            mapping.Map(x => x.InvoiceGuid).Not.Nullable();
            mapping.Map(x => x.SendDate).Nullable();
            mapping.Map(x => x.DueDate).Not.Nullable();
            mapping.Map(x => x.InvoiceDescription).Nullable();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<InvoiceStatus>();
            mapping.HasMany(x => x.InvoiceItem);
        }
    }
}
