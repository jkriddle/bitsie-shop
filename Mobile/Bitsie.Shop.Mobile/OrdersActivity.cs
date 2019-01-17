
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
using System.Threading;
using Bitsie.Shop.Services;
using Bitsie.Shop.Infrastructure;
using Android.Media;

namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Orders")]			
	public class OrdersActivity : BaseActivity
	{
		private Timer timer;
		private List<Order> items;
		private IOrderService orderService;
		private int lastOrderId;
		private bool isProcessing;
		private ListView listView;
		private MediaPlayer mediaPlayer;
		GestureDetector gestureDetector;
		GestureListener gestureListener;

		public OrdersActivity() {
			orderService = Bootstrapper.GetInstance<IOrderService> ();
		}

		protected override void OnPause() {
			base.OnPause ();
			if (timer != null) timer.Dispose ();
			if (mediaPlayer != null) {
				mediaPlayer.Release ();
				mediaPlayer = null;
			}
		}

		protected override void OnDestroy() {
			base.OnStop ();
			if (timer != null) timer.Dispose ();
			if (mediaPlayer != null) {
				mediaPlayer.Release ();
				mediaPlayer = null;
			}
		}

		protected override void OnRestart() {
			base.OnRestart ();
			StartTimer();
			mediaPlayer = MediaPlayer.Create(this, Resource.Raw.register);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (!CheckNetworkConnection())
				return;

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Orders);
			mediaPlayer = MediaPlayer.Create(this, Resource.Raw.register);

			gestureListener = new GestureListener();
			gestureListener.LeftEvent += GoHome;
			gestureDetector = new GestureDetector (this, gestureListener);

			items = new List<Order> ();
			listView = FindViewById<ListView>(Resource.Id.orderList);
			listView.ItemClick += OnListItemClick;
			StartTimer();
		}

		private void GoHome() {
			Toast.MakeText (this, "Gesture Right", ToastLength.Short).Show ();
		}

		private void StartTimer() {
			timer = new Timer(CheckForOrders, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
		}

		private void CheckForOrders(Object state) {
			if (isProcessing)
				return;

			isProcessing = true;

			Thread thread = new Thread (() => {
				var filter = new OrderFilter {
					AfterId = lastOrderId,
					Report = "all",
					SortColumn = "OrderDate",
					SortDirection = "Descending"
				};
				var resp = orderService.GetOrders(PreferencesManager.Get<string>(this, "AuthToken"), filter);

				if (resp.Orders.Count > 0) {
					lastOrderId = resp.Orders.First().OrderId;
					items.InsertRange(0, resp.Orders);
					RunOnUiThread(() => {
						listView.Adapter = new OrderAdapter(this, items);
					});
					mediaPlayer.Start();
				}
				isProcessing = false;
			});
			thread.Start();
		}

		private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var order = items[e.Position];
			//Android.Widget.Toast.MakeText(this, order.Amount, Android.Widget.ToastLength.Short).Show();
		}
	}
}

