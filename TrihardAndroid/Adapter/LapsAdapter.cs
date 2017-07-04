using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using PortableLibrary;
using PortableLibrary.Model;

namespace goheja.Adapter
{
    public class LapsAdapter : BaseAdapter
    {
		List<Lap> _laps;
        int _type;
		BaseActivity mSuperActivity;

		public LapsAdapter(List<Lap> laps, int type, BaseActivity superActivity)
		{
			_laps = laps;
            _type = type;
			mSuperActivity = superActivity;
		}

		public override int Count
		{
			get
			{
				return _laps.Count;
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
			convertView = LayoutInflater.From(mSuperActivity).Inflate(Resource.Layout.item_Lap, null);

            Lap lap = lap = _laps[position];
			LinearLayout lapContent = null;
			
            try
            {
				var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), _type);
				switch (pType)
				{
					case Constants.EVENT_TYPE.BIKE:
					case Constants.EVENT_TYPE.RUN:
                        lapContent = GetBikeOrRunLapsView(convertView, lap, position);
                        break;
					case Constants.EVENT_TYPE.SWIM:
						lapContent = GetSwimLapsView(convertView, lap, position);
						break;
					case Constants.EVENT_TYPE.TRIATHLON:
						lapContent = GetTriathlonLapsView(convertView, lap, position);
						break;
					case Constants.EVENT_TYPE.ANOTHER:
                    case Constants.EVENT_TYPE.OTHER:
						lapContent = GetOtherLapsView(convertView, lap, position);
						break;
				}
			}
			catch (Exception ex)
			{
				//mSuperActivity.ShowTrackMessageBox(ex.Message);
			}

			return lapContent;
		}

        private LinearLayout GetBikeOrRunLapsView(View convertView, Lap lap, int position)
        {
			var lapContent = convertView.FindViewById<LinearLayout>(Resource.Id.lapForBikeOrRun);

			lapContent.FindViewById<TextView>(Resource.Id.lap).Text = lap.lap;
			lapContent.FindViewById<TextView>(Resource.Id.lapKm).Text = lap.lapKm;
			lapContent.FindViewById<TextView>(Resource.Id.elapsedTime).Text = lap.elapsedTime;
			lapContent.FindViewById<TextView>(Resource.Id.avgPace).Text = lap.avgPace;
			lapContent.FindViewById<TextView>(Resource.Id.avgHr).Text = lap.avgHr;
			lapContent.FindViewById<TextView>(Resource.Id.avgCadance).Text = lap.avgCadance;
			lapContent.FindViewById<TextView>(Resource.Id.avgPower).Text = lap.avgPower;

            return lapContent;
        }

		private LinearLayout GetSwimLapsView(View convertView, Lap lap, int position)
		{
			var lapContent = convertView.FindViewById<LinearLayout>(Resource.Id.lapForSwim);

			lapContent.FindViewById<TextView>(Resource.Id.lap).Text = lap.lap;
			lapContent.FindViewById<TextView>(Resource.Id.lapKm).Text = lap.lapKm;
			lapContent.FindViewById<TextView>(Resource.Id.elapsedTime).Text = lap.elapsedTime;
			lapContent.FindViewById<TextView>(Resource.Id.avgPace).Text = lap.avgPace;
			lapContent.FindViewById<TextView>(Resource.Id.avgCadance).Text = lap.avgCadance;
			lapContent.FindViewById<TextView>(Resource.Id.swim_Strock).Text = lap.swim_Strock;

			return lapContent;
		}

		private LinearLayout GetTriathlonLapsView(View convertView, Lap lap, int position)
		{
			var lapContent = convertView.FindViewById<LinearLayout>(Resource.Id.lapForTriathlon);

			lapContent.FindViewById<TextView>(Resource.Id.lap).Text = lap.lap;
			lapContent.FindViewById<TextView>(Resource.Id.lapKm).Text = lap.lapKm;
			lapContent.FindViewById<TextView>(Resource.Id.elapsedTime).Text = lap.elapsedTime;
			lapContent.FindViewById<TextView>(Resource.Id.avgPace).Text = lap.avgPace;
			lapContent.FindViewById<TextView>(Resource.Id.avgHr).Text = lap.avgHr;
			lapContent.FindViewById<TextView>(Resource.Id.avgCadance).Text = lap.avgCadance;
			lapContent.FindViewById<TextView>(Resource.Id.avgPower).Text = lap.avgPower;
            lapContent.FindViewById<TextView>(Resource.Id.swim_Strock).Text = lap.swim_Strock;

			return lapContent;
		}

		private LinearLayout GetOtherLapsView(View convertView, Lap lap, int position)
		{
            var lapContent = convertView.FindViewById<LinearLayout>(Resource.Id.lapForOther);

			lapContent.FindViewById<TextView>(Resource.Id.lap).Text = lap.lap;
			lapContent.FindViewById<TextView>(Resource.Id.elapsedTime).Text = lap.elapsedTime;
			lapContent.FindViewById<TextView>(Resource.Id.avgHr).Text = lap.avgHr;
			lapContent.FindViewById<TextView>(Resource.Id.avgPower).Text = lap.avgPower;
            lapContent.FindViewById<TextView>(Resource.Id.time).Text = lap.time;

			return lapContent;
		}
    }
}
