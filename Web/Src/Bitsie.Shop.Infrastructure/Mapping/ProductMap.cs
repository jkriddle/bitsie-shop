using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class ProductMap : IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.Table("Products");
            mapping.Id(x => x.Id, "ProductID");
            mapping.Map(x => x.Title).Not.Nullable();
            mapping.Map(x => x.Description).Nullable();
            mapping.Map(x => x.ShortDescription).Nullable();
            mapping.Map(x => x.Price).Not.Nullable();
            mapping.Map(x => x.RedirectUrl).Nullable();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<ProductStatus>();
            mapping.References(x => x.User).Not.Nullable().Column("UserID");
        }
    }
}
