using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Models
{
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public IList<string> Errors { get; set; }

        public BaseResponseModel()
        {
            Errors = new List<string>();
        }
    }
}