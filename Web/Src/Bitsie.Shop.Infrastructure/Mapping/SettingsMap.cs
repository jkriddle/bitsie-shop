using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class SettingsMap : IAutoMappingOverride<Settings>
    {
        public void Override(AutoMapping<Settings> mapping)
        {
            mapping.Table("Settings");
            mapping.Id(x => x.Id, "SettingsID");
            mapping.Map(x => x.PaymentMethod).Nullable().CustomType<PaymentMethod>();
            mapping.Map(x => x.BitpayApiKey).Nullable();
            mapping.Map(x => x.CoinbaseApiKey).Nullable();
            mapping.Map(x => x.CoinbaseApiSecret).Nullable();
            mapping.Map(x => x.GoCoinAccessToken).Nullable();
            mapping.Map(x => x.EnableGratuity).Not.Nullable();
            mapping.Map(x => x.DailyMaximum).Not.Nullable().Default("1000");
            mapping.Map(x => x.PaymentAddress).Nullable();
            mapping.Map(x => x.StoreTitle).Nullable();
            mapping.Map(x => x.LogoUrl).Nullable();
            mapping.Map(x => x.BackgroundColor).Nullable();
            mapping.Map(x => x.HtmlTemplate).Nullable();
            mapping.Map(x => x.FreshbooksAuthToken).Nullable();
        }
    }
}
