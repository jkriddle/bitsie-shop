using System;
using System.Collections.Generic;

namespace Bitsie.Shop.Common
{
	public class GetOrdersResponse
	{
		public IList<Order> Orders { get; set; }

		public GetOrdersResponse() {
		}

		public GetOrdersResponse(IList<Order> orders) {
			Orders = orders;
		}
	}
}

