using System;

namespace Bitsie.Shop.Common
{
	public class BaseFilter
	{

		/// <summary>
		/// Column to sort by
		/// </summary>
		/// <value>The sort column.</value>
		public string SortColumn { get; set; }

		/// <summary>
		/// Ascending or Descending
		/// </summary>
		/// <value>The sort direction.</value>
		public string SortDirection { get; set; }
	}
}

