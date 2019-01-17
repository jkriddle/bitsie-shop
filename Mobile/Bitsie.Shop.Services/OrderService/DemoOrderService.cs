using Bitsie.Shop.Common;
using System;
using System.Collections.Generic;

namespace Bitsie.Shop.Services
{
	public class DemoOrderService : IOrderService
	{
		public CreateOrderResponse CreateOrder(string token, Order order) {
			return new CreateOrderResponse {
				Success = true,
				Order = new Order {
					PaymentAddress = "1BitcoinAddress",
					BtcPaid = 0m,
					BtcTotal = .001m,
					OrderDate = DateTime.UtcNow,
					Gratuity = 5.0m,
					OrderId =  1,
					OrderNumber = "R23432",
					PaymentUrl = "bitcoin:1BitcoinAddress",
					Rate = 630m,
					Status = OrderStatus.Pending,
					Subtotal = 10m,
					Total = 15m
				}
			};
		}

		public GetOrderResponse GetOrder(string token, string orderId) {
			return new GetOrderResponse {
				Success = true,
				Order = new Order {
					PaymentAddress = "1BitcoinAddress",
					BtcPaid = 0m,
					BtcTotal = .001m,
					OrderDate = DateTime.UtcNow,
					Gratuity = 5.0m,
					OrderId = 1,
					OrderNumber = "R23432",
					PaymentUrl = "bitcoin:1BitcoinAddress",
					Rate = 630m,
					Status = OrderStatus.Pending,
					Subtotal = 10m,
					Total = 15m
				}
			};
		}

		public UpdateOrderResponse UpdateOrder(string token, string orderId) {
			return new UpdateOrderResponse {
				Success = true,
				Order = new Order {
					PaymentAddress = "1BitcoinAddress",
					BtcPaid = 0m,
					BtcTotal = .001m,
					OrderDate = DateTime.UtcNow,
					Gratuity = 5.0m,
					OrderId = 1,
					OrderNumber = "R23432",
					PaymentUrl = "bitcoin:1BitcoinAddress",
					Rate = 630m,
					Status = OrderStatus.Pending,
					Subtotal = 10m,
					Total = 15m
				}
			};
		}

		public GetOrdersResponse GetOrders(string token, OrderFilter filter) {
			return new GetOrdersResponse() {
				Orders = new List<Order> {
					new Order {
						PaymentAddress = "1BitcoinAddress",
						BtcPaid = 0m,
						BtcTotal = .001m,
						OrderDate = DateTime.UtcNow,
						Gratuity = 5.0m,
						OrderId = 1,
						OrderNumber = "R23432",
						PaymentUrl = "bitcoin:1BitcoinAddress",
						Rate = 630m,
						Status = OrderStatus.Pending,
						Subtotal = 10m,
						Total = 15m
					},
					new Order {
						PaymentAddress = "1BitcoinAddress2",
						BtcPaid = 0m,
						BtcTotal = .002m,
						OrderDate = DateTime.UtcNow,
						Gratuity = 10.0m,
						OrderId = 2,
						OrderNumber = "E39E",
						PaymentUrl = "bitcoin:1BitcoinAddress2",
						Rate = 620m,
						Status = OrderStatus.Pending,
						Subtotal = 20m,
						Total = 25m
					}
				}
			};
		}
	}
}

