using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using System.Text;
using Bitsie.Shop.Services;
using Android.Graphics;
using Bitsie.Shop.Common;
using Bitsie.Shop.Infrastructure;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using Android.Text;

namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Bitsie Shop")]
	public class MainActivity : BaseActivity
	{
		private IOrderService orderService;

		public MainActivity() {

			orderService = Bootstrapper.GetInstance<IOrderService>();
		}

		public override void OnBackPressed() {
			// Disable back button
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (!CheckNetworkConnection())
				return;

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button goButton = FindViewById<Button> (Resource.Id.goButton);

			EditText amountField = FindViewById<EditText> (Resource.Id.amountText);
			string oldAmountText = "";
			amountField.BeforeTextChanged += (object sender, TextChangedEventArgs e) =>
			{
				oldAmountText = amountField.Text;
			};
			amountField.AfterTextChanged += (object sender, AfterTextChangedEventArgs e) =>
			{
				// Ensure value is a valid decimal amount
				string newAmountText = amountField.Text;
				decimal value = 0;
				string[] split = newAmountText.Split('.');
				int count = 0;
				if (split.Length > 1) count = split[split.Length - 1].Length;
				if (newAmountText.Length > 0 
					&& (!decimal.TryParse(newAmountText, out value)
						|| count > 2)) {
					amountField.Text = oldAmountText;
					amountField.SetSelection(amountField.Text.Length);
				}
			};

			TextView dollarSign = FindViewById<TextView> (Resource.Id.dollarSign);
			TextView amountLabel = FindViewById<TextView> (Resource.Id.amountLabel);

			InputMethodManager imm = (InputMethodManager) this.GetSystemService (Context.InputMethodService);
			imm.ToggleSoftInput (ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
			amountField.SetSelection (0);

			goButton.Click += delegate {

				string atext = amountField.Text.ToString();
				decimal amount = Decimal.Parse(amountField.Text);

				Order order = new Order {
					Subtotal = amount,
					Gratuity = 0
				};

				if (PreferencesManager.Get<bool>(this, "EnableGratuity")) {
					var gratuityActivity = new Intent (this, typeof(GratuityActivity));
					gratuityActivity.PutExtra("order", Serializer.Serialize(order));
					StartActivity(gratuityActivity);  
				} else {
					LoadingOverlay.Show(this, "Please Wait", "Generating order...", false);
					Task<bool>.Run (() => {
						try {
							CreateOrderResponse resp = orderService.CreateOrder(PreferencesManager.Get<string>(this, "AuthToken"), order);
							if (!resp.Success) {
								var alertDialog = new AlertDialog.Builder(this).Create();
								alertDialog.SetTitle("Error");
								alertDialog.SetMessage(String.Join("\n", resp.Errors));
								alertDialog.Show();
								return;
							}

							order = resp.Order;
						} catch(Exception) {
							RunOnUiThread(() => {
								EnableOfflineMode();
							});
							return;
						}

						LoadingOverlay.Dismiss();
						var payActivity = new Intent(this, typeof(PayActivity));
						payActivity.PutExtra ("order", Serializer.Serialize(order));
						StartActivity (payActivity);  
					});
				}
			};
				
		}
			
	}
}


