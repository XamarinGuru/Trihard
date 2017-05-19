
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "LoginActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class LoginActivity : BaseActivity
	{
		EditText txtEmail, txtPassword;

		ImageView invalidEmail, invalidPassword;

		LinearLayout errorEmail, errorPassword;

		string requestCode;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LoginActivity);

			requestCode = Intent.GetStringExtra("requestCode");

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionLogin).Click += ActionLogin;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;
			FindViewById<Button>(Resource.Id.ActionForgotPassword).Click += ActionForgotPassword;

			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);

			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);

			invalidEmail.Visibility = ViewStates.Invisible;
			invalidPassword.Visibility = ViewStates.Invisible;
			errorEmail.Visibility = ViewStates.Invisible;
			errorPassword.Visibility = ViewStates.Invisible;

			FindViewById<Button>(Resource.Id.ActionLogin).SetBackgroundColor(GROUP_COLOR);
		}

		private bool Validate()
		{
			invalidEmail.Visibility = ViewStates.Visible;
			invalidPassword.Visibility = ViewStates.Visible;

			bool isValid = true;

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				MarkAsInvalide(invalidEmail, errorEmail, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidEmail, errorEmail, false);
			}

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(invalidPassword, errorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidPassword, errorPassword, false);
			}

			return isValid;
		}

		void ActionLogin(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_LOGIN);

					var loginUser = LoginUser(txtEmail.Text, txtPassword.Text);

					HideLoadingView();

					if (loginUser.userId == null)
					{
                        ShowMessageBox(null, Constants.MSG_LOGIN_FAIL);
					}
					else
					{
						AppSettings.CurrentUser = loginUser;
						AppSettings.DeviceUDID = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

						Intent nextIntent;
						if (loginUser.userType == (int)Constants.USER_TYPE.ATHLETE)
						{
							nextIntent = new Intent(this, typeof(SwipeTabActivity));
						}
						else
						{
							nextIntent = new Intent(this, typeof(CoachHomeActivity));
						}

						StartActivityForResult(nextIntent, 0);
						Finish();
					}
				});
			}
		}

		void ActionForgotPassword(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(ForgotPasswordActivity));
			StartActivityForResult(activity, 1);
		}

		void ActionBack(object sender, EventArgs e)
		{
			if (requestCode == "init")
			{
				ActionBackCancel();
			}
			else
			{
				var activity = new Intent(this, typeof(InitActivity));
				StartActivityForResult(activity, 1);
			}
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				if (requestCode == "init")
				{
					ActionBackCancel();
				}
				else
				{
					var activity = new Intent(this, typeof(InitActivity));
					StartActivityForResult(activity, 1);
				}
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
