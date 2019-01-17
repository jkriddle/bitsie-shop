using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UserListInputModel : PagedInputModel
    {
        public string Email { get; set; }
        public Role? Role { get; set; }
    }
}