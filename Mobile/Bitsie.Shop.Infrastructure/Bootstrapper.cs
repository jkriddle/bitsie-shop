using System;
using Bitsie.Shop.Services;
using Android.Content.Res;
using Bitsie.Shop.Data;

namespace Bitsie.Shop.Infrastructure
{
	public static class Bootstrapper
	{
		static IUserService userService;
		static IOrderService orderService;

		public static void Configure(bool demoMode) {
			if (!demoMode) {
				var bitsieApi = new BitsieApi(Bitsie.Shop.Common.Configuration.BitsieApiRootUrl);
				userService = new UserService (bitsieApi);
				orderService = new OrderService (bitsieApi);
			} else {
				userService = new DemoUserService ();
				orderService = new DemoOrderService ();
			}
		}

		public static T GetInstance<T>() {
			if (typeof(T) == typeof(IUserService))
				return (T)userService;
			if (typeof(T) == typeof(IOrderService))
				return (T)orderService;
			return default(T);
		}
	}
}

