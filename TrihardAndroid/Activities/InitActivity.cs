
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace goheja
{
	[Activity(Label = "InitActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class InitActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.InitActivity);

			InitUISettings();
		}

		void InitUISettings()
		{
			var btnSignIn = FindViewById<Button>(Resource.Id.ActionSignIn);
			var btnSignUp = FindViewById<Button>(Resource.Id.ActionSignUp);
			btnSignIn.Click += ActionSignIn;
			btnSignUp.Click += ActionSignUp;

			btnSignUp.SetBackgroundColor(GROUP_COLOR);
		}

		void ActionSignIn(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(LoginActivity));
			StartActivityForResult(activity, 1);
		}

		void ActionSignUp(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(RegisterActivity));
			StartActivityForResult(activity, 1);
		}
	}
}
