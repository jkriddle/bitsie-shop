using System.Threading;
using Android.App;
using Android.OS;
using Android.Content;
using Bitsie.Shop.Infrastructure;
using System;
using Bitsie.Shop.Common;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Mobile
{

	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : BaseActivity
	{
		public SplashActivity() {
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Check for DEMO mode before setting up dependencies
			if (PreferencesManager.Get<Boolean>(this, "DemoMode") == true) {
				Configuration.DemoMode = true;
			}

			// Setup dependency injection
			Bootstrapper.Configure(Configuration.DemoMode);


			if (String.IsNullOrEmpty(PreferencesManager.Get<string>(this, "AuthToken"))) {
				// preferences not set
				// ensure logo is displayed for a bit
				Thread.Sleep(1000); 
				StartActivity (typeof(LoginActivity));
			} else {
				// Re-authenticate or show login again.
				var userService = Bootstrapper.GetInstance<IUserService> ();
				try {
					AuthenticateResponse resp = userService.Authenticate(PreferencesManager.Get<string> (this, "AuthToken"));
					if (resp.Success && ValidateAccount(resp.User)) {
						PreferencesManager.Init(this, resp.User);
						StartActivity (typeof(MainActivity));
					}
				} catch(Exception) {
					// no connection and we can't authorize the user.
					// if backup address is set, just use that
					string backupAddress = PreferencesManager.Get<string>(this, "BackupAddress");
					if (!String.IsNullOrEmpty(backupAddress)) {
						EnableOfflineMode();
						return;
					} else {
						var alertDialog = new AlertDialog.Builder(this).Create();
						alertDialog.SetTitle("Network Not Found");
						alertDialog.SetMessage("Could not connect to internet in order to validate credentials. Please try again.");

						// Setting OK Button
						alertDialog.SetButton("OK", (sender, args) => {
							this.Finish();
						});

						alertDialog.Show();
						return;
					}
				}
			}
		}
	}
}

