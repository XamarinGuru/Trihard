using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	public class GoHejaEventAdapter : BaseAdapter
	{
		List<GoHejaEvent> _events;
		EventCalendarActivity mSuperActivity;

		public GoHejaEventAdapter(List<GoHejaEvent> events, EventCalendarActivity superActivity)
		{
			_events = events;
			mSuperActivity = superActivity;
		}

		public override int Count
		{
			get
			{
				return _events.Count;
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		override public long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			try
			{
				var goHejaEvent = _events[position];

				if (convertView == null)
				{
					convertView = LayoutInflater.From(mSuperActivity).Inflate(Resource.Layout.item_NitroEvent, null);
				}
				convertView.FindViewById(Resource.Id.ActionEventDetail).Click += ActionEventDetail;
				convertView.FindViewById(Resource.Id.ActionEventDetail).Tag = position;

				var txtTitle = convertView.FindViewById(Resource.Id.txtTitle) as TextView;
				txtTitle.Text = goHejaEvent.title;

				var eventDate = goHejaEvent.StartDateTime();

				var txtTime = convertView.FindViewById(Resource.Id.txtTime) as TextView;
				txtTime.Text = String.Format("{0:t}", eventDate);

				var imgType = convertView.FindViewById<ImageView>(Resource.Id.imgType);
                var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(goHejaEvent.type));
				switch (pType)
				{
					case Constants.EVENT_TYPE.OTHER:
						imgType.SetImageResource(Resource.Drawable.icon_other);
						break;
					case Constants.EVENT_TYPE.BIKE:
						imgType.SetImageResource(Resource.Drawable.icon_bike);
						break;
					case Constants.EVENT_TYPE.RUN:
						imgType.SetImageResource(Resource.Drawable.icon_run);
						break;
					case Constants.EVENT_TYPE.SWIM:
						imgType.SetImageResource(Resource.Drawable.icon_swim);
						break;
					case Constants.EVENT_TYPE.TRIATHLON:
						imgType.SetImageResource(Resource.Drawable.icon_triathlon);
						break;
					case Constants.EVENT_TYPE.ANOTHER:
						imgType.SetImageResource(Resource.Drawable.icon_other);
						break;
				}

				var eventStart = goHejaEvent.StartDateTime();
				var dateNow = DateTime.Now;
				var durMin = goHejaEvent.durMin == "" ? 0 : int.Parse(goHejaEvent.durMin);
				var durHrs = goHejaEvent.durHrs == "" ? 0 : int.Parse(goHejaEvent.durHrs);
				var durSec = durHrs * 3600 + durMin * 60;

				if (goHejaEvent.attended == "0" && DateTime.Compare(eventStart, dateNow.AddSeconds(durSec)) < 0)
				{
					txtTitle.PaintFlags = txtTitle.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;
					txtTitle.SetTextColor(Android.Graphics.Color.Rgb(112, 112, 112));

					txtTime.PaintFlags = txtTime.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;
					txtTime.SetTextColor(Android.Graphics.Color.Rgb(112, 112, 112));
				}
			}
			catch (Exception ex)
			{
				mSuperActivity.ShowTrackMessageBox(ex.Message);
			}

			return convertView;
		}

		void ActionEventDetail(object sender, EventArgs e)
		{
			var index = ((LinearLayout)sender).Tag;
			var selectedEvent = _events[(int)index];

			var nextIntent = new Intent(mSuperActivity, typeof(EventInstructionActivity));
			nextIntent.PutExtra("FromWhere", "CalendarList");
            nextIntent.PutExtra("SelectedEventID", selectedEvent._id);
			mSuperActivity.StartActivityForResult(nextIntent, 1);
			mSuperActivity.OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
		}
	}
}
