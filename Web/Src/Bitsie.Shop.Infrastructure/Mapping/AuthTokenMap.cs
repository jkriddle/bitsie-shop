using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class AuthTokenMap : IAutoMappingOverride<AuthToken>
    {
        public void Override(AutoMapping<AuthToken> mapping)
        {
            mapping.Table("AuthTokens");
            mapping.Id(x => x.Id, "AuthTokenID");
            mapping.Map(x => x.Token).Not.Nullable().Unique().Length(150);
            mapping.Map(x => x.Expires).Not.Nullable();
        }
    }
}
