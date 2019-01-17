using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class InvoiceItemMap : IAutoMappingOverride<InvoiceItem>
    {
        public void Override(AutoMapping<InvoiceItem> mapping)
        {
            mapping.Table("InvoiceItems");
            mapping.Map(x => x.UsdAmount).Not.Nullable();
            mapping.Map(x => x.Quantity).Not.Nullable();
            mapping.Map(x => x.Description).Not.Nullable();
            mapping.Map(x => x.Position).Not.Nullable();
            mapping.References(x => x.Invoice).Column("InvoiceID").Not.Nullable();

        }
    }
}
