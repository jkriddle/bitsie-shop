using System;

namespace Bitsie.Shop.Services
{
	public static class Serializer
	{
		public static string Serialize(Object o) {
			return Data.Serializer.Serialize (o);
		}

		public static T Deserialize<T>(string s) {
			return Data.Serializer.Deserialize<T>(s);
		}
	}
}

