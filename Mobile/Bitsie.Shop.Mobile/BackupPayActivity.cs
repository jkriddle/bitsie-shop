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
using Bitsie.Shop.Services;
using ZXing;
using Android.Graphics;
using ZXing.Common;
using System.Threading;
using Bitsie.Shop.Infrastructure;
using System.Threading.Tasks;

namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Send Payment")]			
	public class BackupPayActivity : QrCodeActivity
	{
		public BackupPayActivity() {
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.BackupPay);

			string bitcoinAddress = PreferencesManager.Get<string>(this, "BackupAddress");

			TextView offlineText = (TextView)FindViewById(Resource.Id.offlineText);
			ImageView imageView = (ImageView)FindViewById(Resource.Id.qrCodeImage);

			if (String.IsNullOrEmpty(bitcoinAddress)) {
				imageView.Visibility = ViewStates.Gone;
				offlineText.Text = "You must be connected to the internet in order to accept bitcoin payments.";
			} else {
				offlineText.Text = "Scan this code with your wallet app to make a payment.";
				GenerateQrCode (imageView, "bitcoin:" + bitcoinAddress);
				imageView.Visibility = ViewStates.Visible;
			}

			Button retryButton = (Button)FindViewById(Resource.Id.retryButton);
			retryButton.Click += delegate {
				if (CheckNetworkConnection(false)) {
					StartActivity(typeof(MainActivity));
				} else {
					Toast.MakeText(this, "Network connection not found.", ToastLength.Long).Show();
				}
			};
		}

	}
}

