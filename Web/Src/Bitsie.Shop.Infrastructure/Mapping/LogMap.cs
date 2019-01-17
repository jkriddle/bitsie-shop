using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class LogMap : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            mapping.Table("Logs");
            mapping.Id(x => x.Id, "LogID");
            mapping.Map(x => x.LogDate).Not.Nullable();
            mapping.Map(x => x.Category).Not.Nullable().CustomType<LogCategory>();
            mapping.Map(x => x.Level).Not.Nullable().CustomType<LogLevel>();
            mapping.Map(x => x.Message).Not.Nullable();
            mapping.Map(x => x.IpAddress).Nullable();
            mapping.References(x => x.User).Column("UserID");
        }
    }
}
