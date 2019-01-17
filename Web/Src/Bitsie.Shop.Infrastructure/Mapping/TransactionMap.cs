using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class TransactionMap : IAutoMappingOverride<Transaction>
    {
        public void Override(AutoMapping<Transaction> mapping)
        {
            mapping.Table("Transactions");
            mapping.Id(x => x.Id, "TransactionID");
            mapping.Map(x => x.TransactionDate).Not.Nullable();
            mapping.Map(x => x.Block).Nullable();
            mapping.Map(x => x.Address).Not.Nullable();
            mapping.Map(x => x.Amount).Precision(18).Scale(9).Not.Nullable();
            mapping.References(x => x.Order).Column("OrderID");
        }
    }
}
