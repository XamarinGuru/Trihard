
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "CoachSubGroupsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class CoachSubGroupsActivity : BaseActivity
	{
		SubGroups _subGroups = new SubGroups();
		List<LinearLayout> _linearSubGroups = new List<LinearLayout>();
		List<TextView> _txtSubGroups = new List<TextView>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.CoachSubGroupsActivity);

			string groupId = Intent.GetStringExtra("GROUP_ID");

        	InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				_subGroups = GetSubGroups(groupId);

				RunOnUiThread(() =>
				{
					for (int i = 0; i < _subGroups.groups.Count; i++)
					{
						_linearSubGroups[i].Tag = _subGroups.groups[i].groupId;
						_txtSubGroups[i].Text = _subGroups.groups[i].groupName;
					}

					HideLoadingView();
				});
			});
		}

		void InitUISettings()
		{
			var linearSubGroup1 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup1);
			var linearSubGroup2 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup2);
			var linearSubGroup3 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup3);
			var linearSubGroup4 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup4);
			var linearSubGroup5 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup5);
			var linearSubGroup6 = FindViewById<LinearLayout>(Resource.Id.ActionSubGroup6);

			var txtSubGroup1 = FindViewById<TextView>(Resource.Id.txtSubGroup1);
			var txtSubGroup2 = FindViewById<TextView>(Resource.Id.txtSubGroup2);
			var txtSubGroup3 = FindViewById<TextView>(Resource.Id.txtSubGroup3);
			var txtSubGroup4 = FindViewById<TextView>(Resource.Id.txtSubGroup4);
			var txtSubGroup5 = FindViewById<TextView>(Resource.Id.txtSubGroup5);
			var txtSubGroup6 = FindViewById<TextView>(Resource.Id.txtSubGroup6);

			_linearSubGroups.Add(linearSubGroup1);
			_linearSubGroups.Add(linearSubGroup2);
			_linearSubGroups.Add(linearSubGroup3);
			_linearSubGroups.Add(linearSubGroup4);
			_linearSubGroups.Add(linearSubGroup5);
			_linearSubGroups.Add(linearSubGroup6);

			_txtSubGroups.Add(txtSubGroup1);
			_txtSubGroups.Add(txtSubGroup2);
			_txtSubGroups.Add(txtSubGroup3);
			_txtSubGroups.Add(txtSubGroup4);
			_txtSubGroups.Add(txtSubGroup5);
			_txtSubGroups.Add(txtSubGroup6);


			linearSubGroup1.Click += ActionSelectedSubGroup;
			linearSubGroup2.Click += ActionSelectedSubGroup;
			linearSubGroup3.Click += ActionSelectedSubGroup;
			linearSubGroup4.Click += ActionSelectedSubGroup;
			linearSubGroup5.Click += ActionSelectedSubGroup;
			linearSubGroup6.Click += ActionSelectedSubGroup;

            FindViewById(Resource.Id.ActionBack).Click += (sender, e) => OnBack();
		}

		void ActionSelectedSubGroup(object sender, EventArgs e)
		{
			var selectedGroupId = (sender as LinearLayout).Tag.ToString();

			foreach (var subGroup in _subGroups.groups)
			{
				if (subGroup.groupId == selectedGroupId)
				{
					if (subGroup.athletes.Count == 0)
					{
						ShowMessageBox(null, Constants.MSG_NO_ATHLETES);
						return;
					}

					AppSettings.selectedAthletesInSubGroup = SortSubUsers(subGroup.athletes);

					var nextIntent = new Intent(this, typeof(CoachAthletesBySubGroupActivity));
					StartActivityForResult(nextIntent, 0);
				}
			}
		}
	}
}
