using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bitsie.Shop.Common;
using Bitsie.Shop.Services;


namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Receipt")]			
	public class ReceiptActivity : Activity
	{
		public override void OnBackPressed() {
			// Disable back button
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Receipt);

			var order = Serializer.Deserialize<Order> (Intent.GetStringExtra ("order"));

			Button homeButton = FindViewById<Button>(Resource.Id.homeButton);
			homeButton.Click += delegate {
				var mainActivity = new Intent (this, typeof(MainActivity));
				StartActivity(mainActivity);  
			};

			var receiptStr = new StringBuilder();
			receiptStr.Append ("Order Number: " + order.OrderNumber + "\n");
			receiptStr.Append ("Date: " + DateTime.Now.ToShortDateString() + "\n");
			if (PreferencesManager.Get<bool>(this, "EnableGratuity")) {
				receiptStr.Append ("Subtotal: " + order.Subtotal.ToString("C") + "\n");
				receiptStr.Append ("Gratuity: " + order.Gratuity.ToString("C") + "\n");
			}
			receiptStr.Append ("Total: " + order.Total.ToString("C") + "\n");

			TextView receiptText = FindViewById<TextView> (Resource.Id.receiptText);
			receiptText.Text = receiptStr.ToString ();


		}
	}
}

