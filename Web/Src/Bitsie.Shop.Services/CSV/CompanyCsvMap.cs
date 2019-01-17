using Bitsie.Shop.Domain;
using CsvHelper.Configuration;
namespace Bitsie.Shop.Services.CSV
{
    public sealed class CompanyCsvMap : CsvClassMap<Company>
    {
        public CompanyCsvMap()
        {
            Map(m => m.Name);
            Map(m => m.Phone);
            Map(m => m.Street);
            Map(m => m.Street2);
            Map(m => m.City);
            Map(m => m.State);
            Map(m => m.Zip);
            Map(m => m.Industry);
            Map(m => m.Website);

        }
    }
}
