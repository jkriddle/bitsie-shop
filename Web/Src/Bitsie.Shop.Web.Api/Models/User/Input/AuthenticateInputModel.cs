using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class AuthenticateInputModel : BaseInputModel
    {
        public string Token { get; set; }
    }
}