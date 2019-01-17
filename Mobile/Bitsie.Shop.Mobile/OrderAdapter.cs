using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.App;
using Bitsie.Shop.Common;

namespace Bitsie.Shop.Mobile
{
	public class OrderAdapter : BaseAdapter<Order> {
		List<Order> items;
		Activity context;
		public OrderAdapter(Activity context, List<Order> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Order this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.OrderRow, null);
			view.FindViewById<TextView> (Resource.Id.amount).Text = item.Total.ToString("C");
			string name = item.FirstName + " " + item.LastName;
			if (String.IsNullOrEmpty (name)) name = "Customer Payment";
			view.FindViewById<TextView> (Resource.Id.name).Text = name;
			view.FindViewById<TextView> (Resource.Id.address).Text = item.PaymentAddress;
			return view;
		}
	}
}

