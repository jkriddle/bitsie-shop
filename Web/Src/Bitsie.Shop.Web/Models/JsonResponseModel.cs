using System.Collections.Generic;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Models
{
    public class JsonResponseModel
    {
        public IList<ValidationError> Errors { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }

        public JsonResponseModel()
        {
            Errors = new List<ValidationError>();
        }
    }
}