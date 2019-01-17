using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    /// <summary>
    /// Convert a date/time sent as a long number (i.e. ticks since 1/1/1970) into a .NET Date/Time
    /// </summary>
    public class EpochDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new DateTime(1970, 1, 1) + new TimeSpan(long.Parse(reader.Value.ToString()) * 10000);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).Ticks.ToString());
        }
    }
}
