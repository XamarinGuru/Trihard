
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

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LoginActivity);

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

					bool isSuccess = LoginUser(txtEmail.Text, txtPassword.Text);

					HideLoadingView();

					if (isSuccess)
					{
						var activity = new Intent(this, typeof(SwipeTabActivity));
						StartActivityForResult(activity, 1);
						Finish();
					}
					else
					{
						ShowMessageBox(null, Constants.MSG_LOGIN_FAIL);
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
			ActionBackCancel();
		}
	}
}
