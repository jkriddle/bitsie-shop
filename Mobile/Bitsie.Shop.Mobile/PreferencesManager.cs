using System;
using Android.Content;
using System.Linq;
using Bitsie.Shop.Common;


namespace Bitsie.Shop.Mobile
{
	public static class PreferencesManager
	{
		/**
		 * Initialize a user's preferences
		 */
		public static void Init(Context con, UserViewModel user) {
			Set(con, "EnableGratuity", user.EnableGratuity);
			Set(con, "BackupAddress", user.BackupAddress);
		}

		/**
		 * Clear a user's preferences
		 */
		public static void Clear(Context con) {
			var prefs = con.GetSharedPreferences("bitsie.shop", FileCreationMode.Private);
			var ed = prefs.Edit ();
			ed.Clear ();
			ed.Commit ();
		}

		/**
		 * Get the specified preference
		 */
		public static T Get<T>(Context con, string key) {
			var prefs = con.GetSharedPreferences("bitsie.shop", FileCreationMode.Private);
			if (!prefs.All.ContainsKey (key))
				return default(T);
			return (T)prefs.All.Where (x => x.Key == key).FirstOrDefault ().Value;
		}

		/**
		 * Set the specified preference
		 */
		public static void Set(Context con, string key, object value) {
			if (value == null)
				return;

			var refType = value.GetType();
			var prefs = con.GetSharedPreferences("bitsie.shop", FileCreationMode.Private);
			var ed = prefs.Edit();

			if (refType == typeof(string))
			{
				ed.PutString(key,(String)value);
				ed.Commit();
				return;
			}
			if (refType == typeof(bool))
			{
				ed.PutBoolean(key, (bool)value);
				ed.Commit();
				return;
			}
			if (refType == typeof(int))
			{
				ed.PutInt(key, (int)value);
				ed.Commit();
				return;
			}
			if (refType == typeof(float))
			{
				ed.PutFloat(key, (float)value);
				ed.Commit();
				return;
			}
			if (refType == typeof(long))
			{
				ed.PutLong(key, (long)value);
				ed.Commit();
				return;
			}
			throw new InvalidOperationException("Type not supported, only use String, Bool, Int, Float, Long");
		}
	}
}

