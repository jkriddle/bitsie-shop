using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class QueueMap : IAutoMappingOverride<Queue>
    {
        public void Override(AutoMapping<Queue> mapping)
        {
            mapping.Table("Queue");
            mapping.Id(x => x.Id, "QueueID");
            mapping.Map(x => x.Guid).Not.Nullable().Length(100).Unique();
            mapping.Map(x => x.QueueDate).Not.Nullable();
            mapping.Map(x => x.Action).Not.Nullable().CustomType<QueueAction>();
            mapping.Map(x => x.Input).Nullable();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<QueueStatus>();
            mapping.Map(x => x.Url).Nullable();
        }
    }
}
