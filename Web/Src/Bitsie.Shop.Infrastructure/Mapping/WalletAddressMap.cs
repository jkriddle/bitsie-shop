using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class WalletAddressMap : IAutoMappingOverride<WalletAddress>
    {
        public void Override(AutoMapping<WalletAddress> mapping)
        {
            mapping.Table("WalletAddresses");
            mapping.Id(x => x.Id, "WalletAddressID");
            mapping.Map(x => x.Address).Not.Nullable();
            mapping.Map(x => x.DateExpires).Not.Nullable();
            mapping.Map(x => x.Derivation).Not.Nullable();
            mapping.Map(x => x.IsUsed).Not.Nullable();
            mapping.References(x => x.Wallet).Column("WalletID").Not.Nullable().Cascade.All();
        }
    }
}
