using System.Collections.Specialized;

namespace Bitsie.Shop.Data
{
	public class PostParameters : NameValueCollection
	{
		public override void Add(string name, string value) {
			base.Add(name, System.Net.WebUtility.UrlEncode(value));
		}
	}
}

