using Bitsie.Shop.Domain;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services.CSV
{
    public class CsvHelper
    {
        public static void Write<T>(Stream stream, IList<T> records)
        {
            using (TextWriter tw = new StreamWriter(stream))
            {
                var csv = new CsvWriter(tw);
                csv.Configuration.RegisterClassMap<CompanyCsvMap>();
                csv.Configuration.RegisterClassMap<UserCsvMap>();
                csv.Configuration.RegisterClassMap<OrderCsvMap>();
                csv.Configuration.RegisterClassMap<LogCsvMap>();
                csv.Configuration.RegisterClassMap<ProductCsvMap>();
                csv.WriteRecords(records);
                csv.NextRecord();
            }
        }
    }
}
