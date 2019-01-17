using Bitsie.Shop.Common;
using System.Net;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Newtonsoft.Json.Converters;

namespace Bitsie.Shop.Data
{
	public class BitsieApi : BaseApi, IBitsieApi
	{
		public BitsieApi(string rootUrl) : base(rootUrl) {
		}

		public AuthenticateResponse Authenticate(string token) {
			PostParameters nv = new PostParameters();
			nv.Add("token", token);
			string content = SendRequest("User/Authenticate", "POST", nv);
			AuthenticateResponse vm = JsonConvert.DeserializeObject<AuthenticateResponse>(content);
			return vm;
		}

		public SignInResponse SignIn(string email, string password) {
			PostParameters nv = new PostParameters();
			nv.Add("email", email);
			nv.Add("password", password);
			string content = SendRequest("User/SignIn", "POST", nv);
			SignInResponse vm = JsonConvert.DeserializeObject<SignInResponse>(content);
			return vm;
		}

		public GetOrderResponse GetOrder(string token, string orderId) {
			string content = SendRequest("Order/GetOne/" + orderId, "GET", null, token);
			GetOrderResponse vm = JsonConvert.DeserializeObject<GetOrderResponse>(content);
			return vm;
		}

		public UpdateOrderResponse UpdateOrder(string token, string orderId) {
			string content = SendRequest("Order/Update/" + orderId, "GET", null, token);
			UpdateOrderResponse vm = JsonConvert.DeserializeObject<UpdateOrderResponse>(content);
			return vm;
		}

		public CreateOrderResponse CreateOrder(string token, Order order) {
			PostParameters nv = new PostParameters();
			nv.Add("Gratuity", order.Gratuity.ToString("N2"));
			nv.Add("Subtotal", order.Subtotal.ToString("N2"));
			string content = SendRequest("Order/Create", "POST", nv, token);
			CreateOrderResponse vm = JsonConvert.DeserializeObject<CreateOrderResponse>(content);
			return vm;
		}

		public GetOrdersResponse GetOrders(string token, OrderFilter filter) {
			PostParameters nv = new PostParameters();
			if (filter.StartDate.HasValue) nv.Add ("StartDate", filter.StartDate.Value.ToString ("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
			if (filter.AfterId.HasValue) nv.Add ("AfterId", filter.AfterId.Value.ToString());
			if (!String.IsNullOrEmpty (filter.Report)) nv.Add("Report", filter.Report);
			if (!String.IsNullOrEmpty (filter.SortColumn)) nv.Add("SortColumn", filter.SortColumn);
			if (!String.IsNullOrEmpty (filter.SortDirection)) nv.Add("SortDirection", filter.SortDirection);
			string content = SendRequest("Order/Get", "GET", nv, token);
			GetOrdersResponse vm = JsonConvert.DeserializeObject<GetOrdersResponse>(content);
			return vm;
		}
	}
}

