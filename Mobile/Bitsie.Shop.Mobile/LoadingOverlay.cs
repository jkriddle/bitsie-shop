using Android.Content;
using System;
using Android.App;
using System.Threading;

namespace Bitsie.Shop.Mobile
{
	public static class LoadingOverlay  {

		private static ProgressDialog dialog;
		private static Activity context;

		public static void Show(Activity activity, string title, string message, bool indeterminate) {
			if (dialog != null) {
				if (dialog.IsShowing)
					Dismiss();
			}
			context = activity;
			dialog = ProgressDialog.Show(context, title, message, indeterminate);
		}

		public static void Dismiss() {
			if (context != null && dialog != null)
				context.RunOnUiThread(() => dialog.Dismiss());
		}
	}
}

