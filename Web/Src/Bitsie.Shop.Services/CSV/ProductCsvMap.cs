using Bitsie.Shop.Domain;
using CsvHelper.Configuration;
namespace Bitsie.Shop.Services.CSV
{
    public sealed class ProductCsvMap : CsvClassMap<Product>
    {
        public ProductCsvMap()
        {
            Map(m => m.Id).Name("ID");
            Map(m => m.Title);

        }
    }
}
