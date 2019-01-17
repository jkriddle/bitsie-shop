using Bitsie.Shop.Domain;
using CsvHelper.Configuration;
namespace Bitsie.Shop.Services.CSV
{
    public sealed class LogCsvMap : CsvClassMap<Log>
    {
        public LogCsvMap()
        {
            Map(m => m.LogDate);
            Map(m => m.Category);
            Map(m => m.Level);
            Map(m => m.Message);
            Map(m => m.Details);
            Map(m => m.IpAddress);
            Map(m => m.User);
        }
    }
}
