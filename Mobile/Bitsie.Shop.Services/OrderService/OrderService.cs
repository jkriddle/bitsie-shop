using Bitsie.Shop.Data;
using Bitsie.Shop.Common;

namespace Bitsie.Shop.Services
{
	public class OrderService : IOrderService
	{
		private readonly IBitsieApi bitsieApi;

		public OrderService(IBitsieApi bitsieApi) {
			this.bitsieApi = bitsieApi;
		}

		public GetOrderResponse GetOrder(string token, string orderId) {
			return bitsieApi.GetOrder(token, orderId);
		}

		public UpdateOrderResponse UpdateOrder(string token, string orderId) {
			return bitsieApi.UpdateOrder(token, orderId);
		}

		public CreateOrderResponse CreateOrder(string token, Order order) {
			return bitsieApi.CreateOrder(token, order);
		}

		public GetOrdersResponse GetOrders(string token, OrderFilter filter) {
			return bitsieApi.GetOrders(token, filter);
		}
	}
}

