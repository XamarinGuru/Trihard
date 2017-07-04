
using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "SelectPTypeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SelectPTypeActivity : BaseActivity
	{
		Color COLOR_DISABLE = Color.Rgb(67, 67, 67);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.SelectPTypeActivity);

			InitUISettings();
		}

		void InitUISettings()
		{
			FindViewById<LinearLayout>(Resource.Id.stateCycling).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateRunning).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateSwimming).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateTriathlon).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateOther).SetBackgroundColor(COLOR_DISABLE);

            var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(AppSettings.selectedEvent.type));
			switch (pType)
			{
				case Constants.EVENT_TYPE.OTHER:
					FindViewById<LinearLayout>(Resource.Id.stateOther).SetBackgroundColor(GROUP_COLOR);
					break;
				case Constants.EVENT_TYPE.BIKE:
					FindViewById<LinearLayout>(Resource.Id.stateCycling).SetBackgroundColor(GROUP_COLOR);
					break;
				case Constants.EVENT_TYPE.RUN:
					FindViewById<LinearLayout>(Resource.Id.stateRunning).SetBackgroundColor(GROUP_COLOR);
					break;
				case Constants.EVENT_TYPE.SWIM:
					FindViewById<LinearLayout>(Resource.Id.stateSwimming).SetBackgroundColor(GROUP_COLOR);
					break;
				case Constants.EVENT_TYPE.TRIATHLON:
					FindViewById<LinearLayout>(Resource.Id.stateTriathlon).SetBackgroundColor(GROUP_COLOR);
					break;
				case Constants.EVENT_TYPE.ANOTHER:
					FindViewById<LinearLayout>(Resource.Id.stateOther).SetBackgroundColor(GROUP_COLOR);
					break;
			}

			FindViewById(Resource.Id.ActionCycling).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionRunning).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionSwimming).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionTriathlon).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionOther).Click += ActionSelectedType;
		}

		void ActionSelectedType(object sender, EventArgs e)
		{
			AppSettings.selectedEvent.type = (sender as RelativeLayout).Tag.ToString();
			ActionBackCancel();
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				ActionBackCancel();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
