using System.Collections.Generic;
using Bitsie.Shop.Services;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class CreateProductResponseModel : BaseResponseModel
    {
        public int ProductId { get; set; }
    }
}