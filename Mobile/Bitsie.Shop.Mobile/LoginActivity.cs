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
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;

namespace Bitsie.Shop.Mobile
{
	[Activity (Label = "Login")]			
	public class LoginActivity : BaseActivity
	{
		private readonly IUserService userService;

		public LoginActivity() {
			userService = Bootstrapper.GetInstance<IUserService> ();
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);

			Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
			loginButton.Click += delegate {

				TextView emailText = FindViewById<TextView>(Resource.Id.emailText);
				TextView passwordText = FindViewById<TextView>(Resource.Id.passwordText);

				if (String.IsNullOrEmpty(emailText.Text) || String.IsNullOrEmpty(passwordText.Text)) {
					Toast.MakeText(this, "Email and password are required.", ToastLength.Long).Show();
					return;
				}
			
				if (emailText.Text == "demo") {
					// Demo mode
					StartDemoMode();
					return;
				} else if (emailText.Text == "clear") {
					// Clear settings
					PreferencesManager.Clear(this);
					Toast.MakeText(this, "Preferences cleared successfully.", ToastLength.Long).Show();
				} else {
					AttemptSignIn(emailText.Text, passwordText.Text);
				}
			};

			Typeface tf = Typeface.CreateFromAsset (Application.Context.Assets, "fonts/HelveticaNeueLTStd-Bd.otf");
			//loginButton.Typeface = tf;

		}

		private void StartDemoMode() {

			// Check if client has been set up previously
			PreferencesManager.Set(this, "AuthToken", "demo");
			PreferencesManager.Set(this, "DemoMode", true);

			// Restart app
			var alertDialog = new AlertDialog.Builder(this).Create();
			alertDialog.SetTitle("Demo Mode Enabled");
			alertDialog.SetMessage("The app will restart in demo mode.");

			// Setting OK Button
			alertDialog.SetButton("OK", (sender, args) => {
				Intent startActivity = new Intent(this, typeof(SplashActivity));
				int pendingIntentId = 707070;
				PendingIntent mPendingIntent = PendingIntent.GetActivity(this, pendingIntentId, startActivity, PendingIntentFlags.CancelCurrent);
				AlarmManager mgr = (AlarmManager)this.GetSystemService(Context.AlarmService);
				mgr.Set(AlarmType.Rtc, DateTime.Now.Millisecond + 1000, mPendingIntent);
				this.Finish();
			});

			alertDialog.Show();
			return;
		}


		private void AttemptSignIn(string email, string password) {
			LoadingOverlay.Show(this, "Please Wait", "Signing in", false);

			Task<bool>.Run (() => {
				SignInResponse resp;
				try {
					resp = userService.SignIn(email, password);
				} catch(Exception) {
					RunOnUiThread(() => {
						LoadingOverlay.Dismiss();
						Toast.MakeText(this, "Unable to connect to server.", ToastLength.Long).Show();
					});
					return;
				}
				LoadingOverlay.Dismiss();
				RunOnUiThread(() => {
					if (!resp.Success) {

						string errString = "";
						switch(resp.Errors[0]){
							case "AuthFail1":
							case "AuthFail3":
							case "SignedOut":
								errString = "Authorization failed, check email/password and try again";
							break;
							case "AuthFail2":
								errString = "Submit failed, try again or contact support";
							break;
							case "AuthFail4":
								errString = "Account is pending approval";
							break;
							default:
								errString = "Authorization failed, check email/password and try again";
							break;
						}
						Toast.MakeText(this, String.Join("\n", errString), ToastLength.Long).Show();
						return;
					} else if (ValidateAccount(resp.User, false)) {
						// Store login token
						PreferencesManager.Set(this, "AuthToken", resp.Token);
						PreferencesManager.Init(this, resp.User);
						StartActivity(typeof(MainActivity));
					}
				});

			}); 
		} 
	}
}

