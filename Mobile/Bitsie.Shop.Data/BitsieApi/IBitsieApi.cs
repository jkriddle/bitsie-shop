using Bitsie.Shop.Common;

namespace Bitsie.Shop.Data
{
	public interface IBitsieApi
	{
		AuthenticateResponse Authenticate(string token);
		SignInResponse SignIn(string email, string password);
		GetOrderResponse GetOrder(string token, string orderId);
		UpdateOrderResponse UpdateOrder(string token, string orderId);
		CreateOrderResponse CreateOrder(string token, Order order);
		GetOrdersResponse GetOrders(string token, OrderFilter filter);
	}
}

