using System.Collections.Generic;
namespace Bitsie.Shop.Web.Models
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