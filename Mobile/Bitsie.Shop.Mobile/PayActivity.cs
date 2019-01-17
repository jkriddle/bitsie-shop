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
	public class PayActivity : QrCodeActivity
	{
		Order order;
		IOrderService orderService;
		Timer timer;
		private bool isProcessing;
		TextView priceText;
		ImageView imageview;
		OrderStatus priorStatus;

		public PayActivity() {
			orderService = Bootstrapper.GetInstance<IOrderService> ();
		}

		protected override void OnPause() {
			base.OnPause ();
			if (timer != null) timer.Dispose ();
		}

		protected override void OnDestroy() {
			base.OnStop ();
			if (timer != null) timer.Dispose ();
		}

		protected override void OnRestart() {
			base.OnRestart ();
			StartTimer ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (!CheckNetworkConnection())
				return;

			SetContentView (Resource.Layout.Pay);

			priceText = FindViewById<TextView>(Resource.Id.priceText);
			//btcPrice = FindViewById<TextView>(Resource.Id.btcPrice);
			imageview = (ImageView)FindViewById(Resource.Id.qrCodeImage);

			// start polling bitpay to see if the payment has been made. If so, go to confirmation screen;
			StartTimer ();


		}

		private void StartTimer() {
			if (order == null) order = Serializer.Deserialize<Order> (Intent.GetStringExtra ("order"));

			priceText.Text = order.Total.ToString("C") + " (" + order.BtcTotal.ToString () + " btc)";
			//btcPrice.Text = "(" + order.BtcTotal.ToString () + " btc)";

			GenerateQrCode (imageview, order.PaymentUrl);

			if (orderService is DemoOrderService) {
				// Allow a tap of the screen to move on.
				imageview.Click += delegate {
					order.Status = OrderStatus.Paid;
					var receiptActivity = new Intent (this, typeof(ReceiptActivity));
					receiptActivity.PutExtra("order", Serializer.Serialize(order));
					StartActivity(receiptActivity);  
				};
			} else {
				timer = new Timer(CheckPaymentStatus, order, TimeSpan.Zero, TimeSpan.FromSeconds(3));
			}
		}

		private void CheckPaymentStatus(Object state) {
			if (isProcessing)
				return;

			isProcessing = true;
			Thread myThread =  new Thread(() => {
				Order currentOrder = (Order)state;

				try {
					var resp = orderService.UpdateOrder(PreferencesManager.Get<string>(this, "AuthToken"), order.OrderNumber);
					currentOrder = resp.Order;
				} catch(Exception) {
					EnableOfflineMode();
					return;
				}

				if (currentOrder.Status != priorStatus) {
					priorStatus = currentOrder.Status;
					switch(currentOrder.Status) {
						case OrderStatus.Paid:
						case OrderStatus.Confirmed:
						case OrderStatus.Complete:
						case OrderStatus.Refunded:
							RunOnUiThread(() => {
								LoadingOverlay.Show(this, "Payment Received", "Please wait...", false);
							});
							Task<bool>.Run (() => {
								var receiptActivity = new Intent(this, typeof(ReceiptActivity));
								receiptActivity.PutExtra("order", Serializer.Serialize(currentOrder));
								StartActivity(receiptActivity); 
							});
							break;
					case OrderStatus.Partial:
							RunOnUiThread(() => {
								Toast.MakeText(this, "Partial payment received.", ToastLength.Long).Show();
								priceText.Text = currentOrder.UsdBalance.ToString("C");
								GenerateQrCode (imageview, currentOrder.PaymentUrl);
							});
						break;
						case OrderStatus.Expired:
							timer.Dispose();
							RunOnUiThread(() => {
								var alertDialog = new AlertDialog.Builder(this).Create();
								alertDialog.SetTitle("Invoice Expired");
								alertDialog.SetMessage("Invoice expired. Please generate a new one and try again.");

								// Setting OK Button
								alertDialog.SetButton("OK", (sender, args) => {
									var mainActivity = new Intent (this, typeof(MainActivity));
									StartActivity(mainActivity);  
								});

								alertDialog.Show();
							});
						break;
					}
				}
				isProcessing = false;
			});
			myThread.Start();
		}

	}

}

