
using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace goheja
{
	[Activity(Label = "CoachGroupsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class CoachGroupsActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CoachGroupsActivity);

			InitUISettings();
		}

		void InitUISettings()
		{
			FindViewById(Resource.Id.ActionGroupAthletes).Click += (sender, e) => OnBack();
			FindViewById(Resource.Id.ActionGroupCycling).Click += ActionSelectedGroup;
			FindViewById(Resource.Id.ActionGroupRun).Click += ActionSelectedGroup;
			FindViewById(Resource.Id.ActionGroupSwim).Click += ActionSelectedGroup;
			FindViewById(Resource.Id.ActionGroupTriathlon).Click += ActionSelectedGroup;
			FindViewById(Resource.Id.ActionGroupOther).Click += ActionSelectedGroup;

			FindViewById(Resource.Id.ActionBack).Click += (sender, e) => OnBack();
		}

		void ActionSelectedGroup(object sender, EventArgs e)
		{
			var nextIntent = new Intent(this, typeof(CoachSubGroupsActivity));
			nextIntent.PutExtra("GROUP_ID", (sender as LinearLayout).Tag.ToString());
			StartActivityForResult(nextIntent, 1);
		}
	}
}
