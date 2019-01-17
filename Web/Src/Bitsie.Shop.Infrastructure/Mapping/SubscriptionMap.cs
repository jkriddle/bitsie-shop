using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class SubscriptionMap : IAutoMappingOverride<Subscription>
    {
        public void Override(AutoMapping<Subscription> mapping)
        {
            mapping.Table("Subscriptions");
            mapping.Id(x => x.Id, "SubscriptionID");
            mapping.Map(x => x.DateExpires).Not.Nullable();
            mapping.Map(x => x.DateRenewed).Not.Nullable();
            mapping.Map(x => x.DateSubscribed).Not.Nullable();
            mapping.Map(x => x.Type).Not.Nullable().CustomType<SubscriptionType>();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<SubscriptionStatus>();
            mapping.Map(x => x.Price).Not.Nullable();
            mapping.Map(x => x.Term).Not.Nullable().CustomType<SubscriptionTerm>();
            mapping.HasOne(x => x.User);
            mapping.HasMany(x => x.Orders);
        }
    }
}
