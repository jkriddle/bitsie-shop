
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
using Newtonsoft.Json;

namespace Bitsie.Shop.Data
{
	public static class Serializer
	{
		public static string Serialize(Object o) {
			return JsonConvert.SerializeObject (o);
		}

		public static T Deserialize<T>(string s) {
			return JsonConvert.DeserializeObject<T>(s);
		}
	}
}

