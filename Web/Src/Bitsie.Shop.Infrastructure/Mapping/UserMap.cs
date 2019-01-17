using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Table("Users");
            mapping.Id(x => x.Id, "UserID");
            mapping.Map(x => x.MerchantId).Nullable().Unique().Length(50);
            mapping.Map(x => x.FirstName).Nullable();
            mapping.Map(x => x.DateRegistered).Not.Nullable();
            mapping.Map(x => x.LastName).Nullable();
            mapping.Map(x => x.Email).Not.Nullable().Length(150);
            mapping.Map(x => x.HashedPassword).Not.Nullable();
            mapping.Map(x => x.Salt).Not.Nullable();
            mapping.Map(x => x.Role).Not.Nullable().CustomType<Role>();
            mapping.References(x => x.AuthToken).Column("AuthTokenID").Cascade.All();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<UserStatus>();
            mapping.HasMany(x => x.Permissions);
            mapping.HasMany(x => x.OfflineAddresses);
            mapping.HasMany(x => x.Logs);
            //mapping.HasOne(x => x.Wallet);
            mapping.References(x => x.Merchant).Column("MerchantUserID").Nullable().Cascade.All();
            mapping.References(x => x.Company).Column("CompanyID").Cascade.All();
            mapping.References(x => x.Settings).Column("SettingsID").Cascade.All();
            mapping.References(x => x.Subscription).Column("SubscriptionID").Cascade.All();
        }
    }
}
