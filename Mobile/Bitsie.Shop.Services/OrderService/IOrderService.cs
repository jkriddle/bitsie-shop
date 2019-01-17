using Bitsie.Shop.Common;

namespace Bitsie.Shop.Services
{
	public interface IOrderService
	{
		GetOrderResponse GetOrder(string token, string orderId);
		UpdateOrderResponse UpdateOrder(string token, string orderId);
		CreateOrderResponse CreateOrder(string token, Order order);
		GetOrdersResponse GetOrders(string token, OrderFilter filter);
	}
}

