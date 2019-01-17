using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class SystemSettingMap : IAutoMappingOverride<SystemSetting>
    {
        public void Override(AutoMapping<SystemSetting> mapping)
        {
            mapping.Table("SystemSettings");
            mapping.Id(x => x.Id, "SystemSettingID");
            mapping.Map(x => x.Name).Not.Nullable();
            mapping.Map(x => x.Value).Nullable();
        }
    }
}
