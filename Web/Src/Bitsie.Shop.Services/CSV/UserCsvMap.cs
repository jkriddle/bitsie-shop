using Bitsie.Shop.Domain;
using CsvHelper.Configuration;
namespace Bitsie.Shop.Services.CSV
{
    public sealed class UserCsvMap : CsvClassMap<User>
    {
        public UserCsvMap()
        {
            Map(m => m.MerchantId).Name("Merchant ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Company);
            Map(m => m.Email);
            Map(m => m.Role);
            Map(m => m.Status);

        }
    }
}
