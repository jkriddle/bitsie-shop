using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class Bip39WalletMap : IAutoMappingOverride<Bip39Wallet>
    {
        public void Override(AutoMapping<Bip39Wallet> mapping)
        {
            mapping.Table("Wallets");
            mapping.Id(x => x.Id, "WalletID");
            mapping.Map(x => x.PublicMasterKey).Not.Nullable();
            mapping.Map(x => x.EncryptedPrivateMasterKey).Nullable();
            mapping.Map(x => x.LastDerivation).Nullable();
            mapping.HasMany(x => x.Addresses).KeyColumn("WalletID");
            mapping.References(x => x.User).Column("UserID").Not.Nullable().Cascade.All();
        }
    }
}
