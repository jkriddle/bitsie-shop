using Bitsie.Shop.Web.Api.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace Bitsie.Shop.Web.Api.Models
{
    public class AlertViewModel
    {
        public AlertViewModel(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Message { get; set; }
    }
}