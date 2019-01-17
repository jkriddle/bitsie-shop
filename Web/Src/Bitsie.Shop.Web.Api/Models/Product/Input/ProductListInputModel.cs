using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class ProductListInputModel : PagedInputModel
    {
        public string Title { get; set; }
        public string MerchantId { get; set; }
    }
}