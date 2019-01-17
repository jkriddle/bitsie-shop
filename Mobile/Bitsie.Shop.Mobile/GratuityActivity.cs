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
using Bitsie.Shop.Services;
using Bitsie.Shop.Common;
using Bitsie.Shop.Infrastructure;
using System.Threading.Tasks;
using Android.Util;
using Android.Graphics;
using Android.Text;
using Android.Views.InputMethods;

namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Gratuity")]			
	public class GratuityActivity : BaseActivity
	{
		Order order;
		decimal subtotal = 0;
		TextView totalText;
		IOrderService orderService;
		EditText gratuityAmount;

		public GratuityActivity() {
			orderService = Bootstrapper.GetInstance<IOrderService>();
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			if (!CheckNetworkConnection())
				return;

			SetContentView (Resource.Layout.Gratuity);

			order = Serializer.Deserialize<Order> (Intent.GetStringExtra ("order"));
			totalText = FindViewById<TextView> (Resource.Id.totalText);
			TextView subtotalText = FindViewById<TextView> (Resource.Id.subtotal_label);
			subtotal = order.Subtotal;
			subtotalText.Text = "subtotal: " + Math.Round(Convert.ToDecimal(subtotal), 2).ToString();
			totalText.Text = "$" + order.Subtotal.ToString("N2");

			gratuityAmount = FindViewById<EditText> (Resource.Id.gratuityAmountText);
			Button gratuityNone = FindViewById<Button> (Resource.Id.gratuityNone);
			gratuityNone.Click += delegate {
				SetGratuity (0m);
				gratuityAmount.Text = Math.Round(Convert.ToDecimal(order.Gratuity), 2).ToString();
			};

			Button gratuity10 = FindViewById<Button> (Resource.Id.gratuity10);
			gratuity10.Click += delegate {
				SetGratuity (subtotal * .10m);
				gratuityAmount.Text = Math.Round(Convert.ToDecimal(order.Gratuity), 2).ToString();
			};

			Button gratuity15 = FindViewById<Button> (Resource.Id.gratuity15);
			gratuity15.Click += delegate {
				SetGratuity (subtotal * .15m);
				gratuityAmount.Text = Math.Round(Convert.ToDecimal(order.Gratuity), 2).ToString();
			};

			Button gratuity20 = FindViewById<Button> (Resource.Id.gratuity20);
			gratuity20.Click += delegate {
				decimal gratAmount = (subtotal * .20m);
				gratuityAmount.Text = Math.Round(Convert.ToDecimal(gratAmount), 2).ToString();
				SetGratuity (gratAmount);
			};

			gratuityAmount.KeyPress += (object sender, View.KeyEventArgs e) => {
				if (e.KeyCode == Keycode.Enter) {
					InputMethodManager manager = (InputMethodManager) GetSystemService(InputMethodService);
					manager.HideSoftInputFromWindow(gratuityAmount.WindowToken, 0);
					e.Handled = true;
				}
				e.Handled = false;
			};

			//update total on gratuity amount change
			string oldAmountText = "";
			gratuityAmount.BeforeTextChanged += (object sender, TextChangedEventArgs e) =>
			{
				oldAmountText = gratuityAmount.Text;
			};

			gratuityAmount.AfterTextChanged += (object sender, AfterTextChangedEventArgs e) =>
			{
				// Ensure value is a valid decimal amount
				// A valid amount must be parseable as a decimal and not be empty,
				// and not have more than 2 decimal places
				string newAmountText = gratuityAmount.Text;
				decimal value = 0;
				string[] split = newAmountText.Split('.');
				int count = 0;
				if (split.Length > 1) count = split[split.Length - 1].Length;
				if (newAmountText.Length > 0 
					&& (!decimal.TryParse(newAmountText, out value)
						|| count > 2)) {
					// Amount being entered is not okay
					gratuityAmount.Text = oldAmountText;
					gratuityAmount.SetSelection(gratuityAmount.Text.Length);
				} else {
					// amount is okay
					SetGratuity(value);
				}
			};

			Button continueButton = FindViewById<Button> (Resource.Id.continueButton);
			continueButton.Click += delegate {

				LoadingOverlay.Show(this, "Please Wait", "Generating invoice...", false);
				Task<bool>.Run (() => {
					CreateOrderResponse resp = null;
					try {
						resp = orderService.CreateOrder(PreferencesManager.Get<string>(this, "AuthToken"), order);
					} catch(Exception) {
						RunOnUiThread(() => {
							EnableOfflineMode();
						});
						return;
					}

					LoadingOverlay.Dismiss();

					if (resp.Success) {
						var payActivity = new Intent(this, typeof(PayActivity));
						payActivity.PutExtra ("order", Serializer.Serialize(resp.Order));
						StartActivity (payActivity);  
						return;
					}

					// Response failed
					ShowDialog("Invalid Invoice", String.Join("\n", resp.Errors), delegate {
						// Do nothing after OK
					});
				});
			};

		}
			

		private void SetGratuity(decimal value) {
			order.Gratuity = value;
			order.Total = order.Subtotal + value;
			totalText.Text = order.Total.ToString("C");
		}
	}
}

