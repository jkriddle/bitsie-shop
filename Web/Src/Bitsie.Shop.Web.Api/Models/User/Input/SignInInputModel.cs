using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class SignInInputModel : BaseInputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string hashcashid { get; set; }
        public bool RememberMe { get; set; }
    }
}