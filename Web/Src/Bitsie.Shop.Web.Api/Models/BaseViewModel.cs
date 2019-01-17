using System.Collections.Generic;
namespace Bitsie.Shop.Web.Api.Models
{
    public class BaseViewModel
    {
        public bool Success { get; set; }
        public IList<string> Errors { get; set; }

        public BaseViewModel()
        {
            Errors = new List<string>();
        }
    }
}