using Bitsie.Shop.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class UserCompanyMapMap : IAutoMappingOverride<Company>
    {
        public void Override(AutoMapping<Company> mapping)
        {
            mapping.Table("Companies");
            mapping.Id(x => x.Id, "CompanyID");
            mapping.Map(x => x.Name).Nullable();
            mapping.Map(x => x.Street).Nullable();
            mapping.Map(x => x.Street2).Nullable();
            mapping.Map(x => x.City).Nullable();
            mapping.Map(x => x.State).Nullable();
            mapping.Map(x => x.Zip).Nullable();
            mapping.Map(x => x.Phone).Nullable();
            mapping.Map(x => x.Website).Nullable();
        }
    }
}
