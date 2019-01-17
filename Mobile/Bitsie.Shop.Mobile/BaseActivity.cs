using Android.App;
using Android.OS;
using Android.Net;
using Android.Widget;
using Android.Content;
using Java.Net;
using System;
using System.Threading.Tasks;
using Bitsie.Shop.Common;
using System.Net;
using System.IO;

namespace Bitsie.Shop.Mobile
{
	[Activity(Label = "BaseActivity")]			
	public class BaseActivity : Activity
	{

		private void TryConnection(int timeout) {
			URL testUrl = new URL (Configuration.BitsieApiRootUrl);
			using (URLConnection connection = testUrl.OpenConnection ()) {
				connection.ConnectTimeout = timeout;
				connection.Connect();
			}
		}

		public bool CheckNetworkConnection(bool enableOfflineMode = true) {
			if (Configuration.DemoMode)
				return true;

			try {
				var task = Task<bool>.Run(() => {
					TryConnection(10000);
				});
				task.Wait();
				return true;
			} catch(Exception) {
				if (enableOfflineMode) EnableOfflineMode();
				return false;
			}

		}
			
		protected void EnableOfflineMode() {
			RunOnUiThread(() => {
				LoadingOverlay.Dismiss();
				Toast.MakeText(this, "Network connection not found. Using backup address.", ToastLength.Long).Show();
				StartActivity (typeof(BackupPayActivity)); 
			});
		}

		/// <summary>
		/// Ensure user has set up their account properly in order to use a mobile device
		/// </summary>
		/// <returns><c>true</c>, if account was validated, <c>false</c> otherwise.</returns>
		/// <param name="user">User.</param>
		protected bool ValidateAccount(UserViewModel user, bool redirectOnInvalid = true) {
			if (!user.PaymentMethod.HasValue) {
				var alertDialog = new AlertDialog.Builder(this).Create();
				alertDialog.SetTitle("Account Setup Required");
				alertDialog.SetMessage(Resources.GetString(Resource.String.noProvider));

				// Setting OK Button
				alertDialog.SetButton("OK", (sender, args) => {
					if (redirectOnInvalid) StartActivity (typeof(LoginActivity));
				});

				alertDialog.Show();
			}
			return user.PaymentMethod.HasValue;
		}

		protected void ShowDialog(string title, string message, Action okCallback) {
			RunOnUiThread (() => {
				var alertDialog = new AlertDialog.Builder(this).Create();
				alertDialog.SetTitle(title);
				alertDialog.SetMessage(message);

				// Setting OK Button
				alertDialog.SetButton("OK", (sender, args) => {
					okCallback();
				});

				alertDialog.Show();
			});
		}
	}
}

