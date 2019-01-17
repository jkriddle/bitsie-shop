using System.Collections.Generic;

namespace Bitsie.Shop.Common
{
	public class BaseResponse
	{
		public bool Success { get; set; }
		public IList<string> Errors { get; set; }
	}
}