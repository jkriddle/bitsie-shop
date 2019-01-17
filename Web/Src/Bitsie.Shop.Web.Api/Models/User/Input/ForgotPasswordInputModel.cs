using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class ForgotPasswordInputModel : BaseInputModel
    {
        public string Email { get; set; }
    }
}