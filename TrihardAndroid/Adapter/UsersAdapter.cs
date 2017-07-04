using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using PortableLibrary;
using UniversalImageLoader.Core;

namespace goheja
{
	public class UsersAdapter : BaseAdapter
	{
		List<Athlete> _athletes;
		List<Athlete> _searchAthletes;
		BaseActivity mSuperActivity;

		public UsersAdapter(List<Athlete> athletes, BaseActivity superActivity)
		{
			_athletes = athletes;
			_searchAthletes = athletes;
			mSuperActivity = superActivity;
		}

		public override int Count
		{
			get
			{
				return _searchAthletes.Count;
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
				var athletes = _searchAthletes[position];

				convertView = LayoutInflater.From(mSuperActivity).Inflate(Resource.Layout.item_User, null);

				convertView.FindViewById<TextView>(Resource.Id.lblName).Text = athletes.name;

				if (!string.IsNullOrEmpty(athletes.userImagURI))
				{
					var imgIcon = convertView.FindViewById<ImageView>(Resource.Id.imgProfile);
					ImageLoader imageLoader = ImageLoader.Instance;
					imageLoader.DisplayImage(athletes.userImagURI, imgIcon);
				}

				var ActionFakeUser = convertView.FindViewById<LinearLayout>(Resource.Id.ActionFakeUser);
				ActionFakeUser.Click += ActionChangeFakeUser;
				ActionFakeUser.Tag = athletes._id;

                var scrollView = convertView.FindViewById<HorizontalScrollView>(Resource.Id.todayDoneScrollView);
                var layout = new LinearLayout(mSuperActivity.ApplicationContext);

                foreach (var eventDoneToday in athletes.eventsDoneToday)
                {
					var imgTodayDone = new ImageView(mSuperActivity.ApplicationContext);
					var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(eventDoneToday.eventType));
					switch (pType)
					{
						case Constants.EVENT_TYPE.OTHER:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_other);
							break;
						case Constants.EVENT_TYPE.BIKE:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_bike);
							break;
						case Constants.EVENT_TYPE.RUN:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_run);
							break;
						case Constants.EVENT_TYPE.SWIM:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_swim);
							break;
						case Constants.EVENT_TYPE.TRIATHLON:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_triathlon);
							break;
						case Constants.EVENT_TYPE.ANOTHER:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_other);
							break;
					}

                    imgTodayDone.SetX(0);
                    int dimensionInDp = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, mSuperActivity.Resources.DisplayMetrics);
                    imgTodayDone.LayoutParameters = new ViewGroup.LayoutParams(dimensionInDp, ViewGroup.LayoutParams.MatchParent);
                    imgTodayDone.SetPadding(5, 5, 5, 5);
                    layout.AddView(imgTodayDone);

                    imgTodayDone.Click += ActionEventInstruction;
                    imgTodayDone.Tag = athletes._id + "," + eventDoneToday.eventId;
                }
                scrollView.AddView(layout);

				switch (athletes.pmcStatus)
				{
					case 1:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_green);
						break;
					case 2:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_blue);
						break;
					case 3:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_red);
						break;
					case 4:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_empty);
						break;
				}
			}
			catch (Exception ex)
			{
				//mSuperActivity.ShowTrackMessageBox(ex.Message);
			}

			return convertView;
		}

        private void ActionEventInstruction(object sender, EventArgs e)
        {
            var tags = (sender as ImageView).Tag.ToString().Split(new char[] { ',' });

            var fakeUserId = tags[0];
            var eventId = tags[1];

            var currentUser = AppSettings.CurrentUser;

            if (currentUser.userId == fakeUserId)
            {
                currentUser.athleteId = null;
                AppSettings.isFakeUser = false;
                AppSettings.fakeUserName = string.Empty;
            }
            else
            {
                currentUser.athleteId = fakeUserId;
                AppSettings.isFakeUser = true;
                foreach (var tmpUser in _searchAthletes)
                {
                    if (tmpUser._id == fakeUserId)
                        AppSettings.fakeUserName = tmpUser.name;
                }
            }

            AppSettings.CurrentUser = currentUser;

            var nextIntent = new Intent(mSuperActivity, typeof(EventInstructionActivity));
            nextIntent.PutExtra("FromWhere", "CoachList");
            nextIntent.PutExtra("SelectedEventID", eventId);
            mSuperActivity.StartActivityForResult(nextIntent, 0);
        }

        public void PerformSearch(string strSearch)
		{
			strSearch = strSearch.ToLower();
			_searchAthletes = _athletes.Where(x => x.name.ToLower().Contains(strSearch)).ToList();
		}

		void ActionChangeFakeUser(object sender, EventArgs e)
		{
			var fakeUserId = (sender as LinearLayout).Tag.ToString();

			var currentUser = AppSettings.CurrentUser;

            if (currentUser.userId == fakeUserId)
            {
                currentUser.athleteId = null;
                AppSettings.isFakeUser = false;
                AppSettings.fakeUserName = string.Empty;
            }
            else
            {
                currentUser.athleteId = fakeUserId;
                AppSettings.isFakeUser = true;
				foreach (var tmpUser in _searchAthletes)
				{
					if (tmpUser._id == fakeUserId)
                        AppSettings.fakeUserName = tmpUser.name;
				}
            }

			AppSettings.CurrentUser = currentUser;

			var nextIntent = new Intent(mSuperActivity, typeof(SwipeTabActivity));
			mSuperActivity.StartActivityForResult(nextIntent, 0);
		}
	}

	public class UsersInSubGroupAdapter : BaseAdapter
	{
		List<AthleteInSubGroup> _athletes;
		List<AthleteInSubGroup> _searchAthletes;
		BaseActivity mSuperActivity;

		public UsersInSubGroupAdapter(List<AthleteInSubGroup> athletes, BaseActivity superActivity)
		{
			_athletes = athletes;
			_searchAthletes = athletes;
			mSuperActivity = superActivity;
		}

		public override int Count
		{
			get
			{
				return _searchAthletes.Count;
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
				var athletes = _searchAthletes[position];

				convertView = LayoutInflater.From(mSuperActivity).Inflate(Resource.Layout.item_User, null);

				convertView.FindViewById<TextView>(Resource.Id.lblName).Text = athletes.athleteName;

				if (!string.IsNullOrEmpty(athletes.userImagURI))
				{
					var imgIcon = convertView.FindViewById<ImageView>(Resource.Id.imgProfile);
					ImageLoader imageLoader = ImageLoader.Instance;
					imageLoader.DisplayImage(athletes.userImagURI, imgIcon);
				}

				var ActionFakeUser = convertView.FindViewById<LinearLayout>(Resource.Id.ActionFakeUser);
				ActionFakeUser.Click += ActionChangeFakeUser;
				ActionFakeUser.Tag = athletes.athleteId;

				var scrollView = convertView.FindViewById<HorizontalScrollView>(Resource.Id.todayDoneScrollView);
				var layout = new LinearLayout(mSuperActivity.ApplicationContext);

				foreach (var eventDoneToday in athletes.eventsDoneToday)
				{
					var imgTodayDone = new ImageView(mSuperActivity.ApplicationContext);
                    var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(eventDoneToday.eventType));
					switch (pType)
					{
						case Constants.EVENT_TYPE.OTHER:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_other);
							break;
						case Constants.EVENT_TYPE.BIKE:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_bike);
							break;
						case Constants.EVENT_TYPE.RUN:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_run);
							break;
						case Constants.EVENT_TYPE.SWIM:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_swim);
							break;
						case Constants.EVENT_TYPE.TRIATHLON:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_triathlon);
							break;
						case Constants.EVENT_TYPE.ANOTHER:
							imgTodayDone.SetImageResource(Resource.Drawable.icon_other);
							break;
					}

					imgTodayDone.SetX(0);
					int dimensionInDp = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, mSuperActivity.Resources.DisplayMetrics);
					imgTodayDone.LayoutParameters = new ViewGroup.LayoutParams(dimensionInDp, ViewGroup.LayoutParams.MatchParent);
					imgTodayDone.SetPadding(5, 5, 5, 5);
					layout.AddView(imgTodayDone);

					imgTodayDone.Click += ActionEventInstruction;
                    imgTodayDone.Tag = athletes.athleteId + "," + eventDoneToday.eventId;
				}
				scrollView.AddView(layout);

				switch (athletes.pmcStatus)
				{
					case 1:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_green);
						break;
					case 2:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_blue);
						break;
					case 3:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_red);
						break;
					case 4:
						convertView.FindViewById<ImageView>(Resource.Id.imgStatus).SetImageResource(Resource.Drawable.icon_circle_empty);
						break;
				}
			}
			catch (Exception ex)
			{
				//mSuperActivity.ShowTrackMessageBox(ex.Message);
			}

			return convertView;
		}

        private void ActionEventInstruction(object sender, EventArgs e)
        {
            var tags = (sender as ImageView).Tag.ToString().Split(new char[] { ',' });

            var fakeUserId = tags[0];
            var eventId = tags[1];

            var currentUser = AppSettings.CurrentUser;

            if (currentUser.userId == fakeUserId)
            {
                currentUser.athleteId = null;
                AppSettings.isFakeUser = false;
                AppSettings.fakeUserName = string.Empty;
            }
            else
            {
                currentUser.athleteId = fakeUserId;
                AppSettings.isFakeUser = true;
                foreach (var tmpUser in _searchAthletes)
                {
                    if (tmpUser.athleteId == fakeUserId)
                        AppSettings.fakeUserName = tmpUser.athleteName;
                }
            }

            AppSettings.CurrentUser = currentUser;

            var nextIntent = new Intent(mSuperActivity, typeof(EventInstructionActivity));
            nextIntent.PutExtra("FromWhere", "CoachList");
            nextIntent.PutExtra("SelectedEventID", eventId);
            mSuperActivity.StartActivityForResult(nextIntent, 0);
        }

		public void PerformSearch(string strSearch)
		{
			strSearch = strSearch.ToLower();
			_searchAthletes = _athletes.Where(x => x.athleteName.ToLower().Contains(strSearch)).ToList();
		}

		void ActionChangeFakeUser(object sender, EventArgs e)
		{
			var fakeUserId = (sender as LinearLayout).Tag.ToString();

			var currentUser = AppSettings.CurrentUser;

			if (currentUser.userId == fakeUserId)
			{
				currentUser.athleteId = null;
				AppSettings.isFakeUser = false;
                AppSettings.fakeUserName = string.Empty;
			}
			else
			{
				currentUser.athleteId = (sender as LinearLayout).Tag.ToString();
				AppSettings.isFakeUser = true;
				foreach (var tmpUser in _searchAthletes)
				{
                    if (tmpUser.athleteId == fakeUserId)
                        AppSettings.fakeUserName = tmpUser.athleteName;
				}
			}

			AppSettings.CurrentUser = currentUser;

			var nextIntent = new Intent(mSuperActivity, typeof(SwipeTabActivity));
			mSuperActivity.StartActivityForResult(nextIntent, 0);
		}
	}
}
