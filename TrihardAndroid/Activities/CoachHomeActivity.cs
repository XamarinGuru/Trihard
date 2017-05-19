
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Widget;
using PortableLibrary;
using UniversalImageLoader.Core;

namespace goheja
{
	[Activity(Label = "CoachHomeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class CoachHomeActivity : BaseActivity
	{
		ListView _usersList;
		List<Athlete> _users = new List<Athlete>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CoachHomeActivity);

			var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
			ImageLoader.Instance.Init(config);

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				_users = GetAllUsers();

				RunOnUiThread(() =>
				{
					var adapter = new UsersAdapter(_users, this);
					_usersList.Adapter = adapter;
					adapter.NotifyDataSetChanged();

					HideLoadingView();
				});
			});
		}

		void InitUISettings()
		{
			_usersList = FindViewById<ListView>(Resource.Id.usersList);

			FindViewById<EditText>(Resource.Id.txtSearch).TextChanged += ActionSearch;
			FindViewById(Resource.Id.ActionShowByGroup).Click += ActionShowByGroup;
		}

		void ActionSearch(object sender, TextChangedEventArgs e)
		{
			(_usersList.Adapter as UsersAdapter).PerformSearch((sender as EditText).Text);
			(_usersList.Adapter as UsersAdapter).NotifyDataSetChanged();
		}

		void ActionShowByGroup(object sender, EventArgs e)
		{
			var nextIntent = new Intent(this, typeof(CoachGroupsActivity));
			StartActivityForResult(nextIntent, 0);
		}
	}
}
