using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class CreateInvoiceResponseModel : BaseResponseModel
    {
        public InvoiceViewModel Invoice { get; set; }
    }
}