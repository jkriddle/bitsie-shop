using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class OfflineAddressMap : IAutoMappingOverride<OfflineAddress>
    {
        public void Override(AutoMapping<OfflineAddress> mapping)
        {
            mapping.Table("OfflineAddresses");
            mapping.Id(x => x.Id, "OfflineAddressId");
            mapping.Map(x => x.Address).Not.Nullable().Unique().Length(40);
            mapping.Map(x => x.EncryptedPrivateKey).Nullable();
            mapping.Map(x => x.EmailNotifications).Nullable();
            mapping.Map(x => x.TextNotifications).Nullable();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<OfflineAddressStatus>();
            mapping.References(x => x.User).Column("UserID").Not.Nullable();
        }
    }
}
