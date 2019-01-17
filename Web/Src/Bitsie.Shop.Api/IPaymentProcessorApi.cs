using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public interface IPaymentProcessorApi
    {
        Order GetOrder(string orderNumber);
        void CreateOrder(Order order, string callbackUrl);
    }
}
