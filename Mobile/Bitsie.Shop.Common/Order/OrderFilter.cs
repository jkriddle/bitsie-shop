using System;

namespace Bitsie.Shop.Common
{
	public class OrderFilter : BaseFilter
	{
		public int? AfterId { get; set; }
		public DateTime? StartDate { get; set; }
		public string Report { get; set; }
	}
}

