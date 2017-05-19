
using System.Collections.Generic;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Widget;
using PortableLibrary;
using UniversalImageLoader.Core;

namespace goheja
{
	[Activity(Label = "CoachAthletesBySubGroupActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class CoachAthletesBySubGroupActivity : BaseActivity
	{
		ListView _usersList;
		List<AthleteInSubGroup> _users = new List<AthleteInSubGroup>();
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CoachAthletesBySubGroupActivity);

			var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
			ImageLoader.Instance.Init(config);

            InitUISettings();

			if (!IsNetEnable()) return;

			_users = AppSettings.selectedAthletesInSubGroup;

			var adapter = new UsersInSubGroupAdapter(_users, this);
			_usersList.Adapter = adapter;
			adapter.NotifyDataSetChanged();
		}

		void InitUISettings()
		{
			_usersList = FindViewById<ListView>(Resource.Id.usersList);

			FindViewById<EditText>(Resource.Id.txtSearch).TextChanged += ActionSearch;
		}

		void ActionSearch(object sender, TextChangedEventArgs e)
		{
			(_usersList.Adapter as UsersInSubGroupAdapter).PerformSearch((sender as EditText).Text);
			(_usersList.Adapter as UsersInSubGroupAdapter).NotifyDataSetChanged();
		}
	}
}
