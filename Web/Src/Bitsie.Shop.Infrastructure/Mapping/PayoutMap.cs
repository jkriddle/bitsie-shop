using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class PayoutMap : IAutoMappingOverride<Payout>
    {
        public void Override(AutoMapping<Payout> mapping)
        {
            mapping.Table("Payouts");
            mapping.Id(x => x.Id, "PayoutID");
            mapping.Map(x => x.DateProcessed).Not.Nullable();
            mapping.Map(x => x.ConfirmationNumber).Nullable();
            mapping.Map(x => x.TransactionId).Nullable();
            mapping.Map(x => x.PayoutAmount).Precision(9).Scale(2).Not.Nullable();
            mapping.Map(x => x.PayoutFee).Precision(9).Scale(2).Not.Nullable();
            mapping.HasMany(x => x.Order);
        }
    }
}
