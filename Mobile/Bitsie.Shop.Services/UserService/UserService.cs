
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bitsie.Shop.Common;
using Bitsie.Shop.Data;

namespace Bitsie.Shop.Services
{		
	public class UserService : IUserService
	{
		private readonly IBitsieApi bitsieApi;

		public UserService(IBitsieApi bitsieApi) {
			this.bitsieApi = bitsieApi;
		}

		public AuthenticateResponse Authenticate(string token) {
			return bitsieApi.Authenticate(token);
		}

		public SignInResponse SignIn(string email, string password) {
			return bitsieApi.SignIn(email, password);
		}
	}
}

