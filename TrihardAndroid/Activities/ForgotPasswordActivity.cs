
using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ForgotPasswordActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class ForgotPasswordActivity : BaseActivity
	{
		EditText txtEmail;
		ImageView invalidEmail;
		LinearLayout errorEmail;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ForgotPasswordActivity);

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionResetPassword).Click += ActionResetPassword;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;

			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);

			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);

			invalidEmail.Visibility = ViewStates.Invisible;
			errorEmail.Visibility = ViewStates.Invisible;

			FindViewById<Button>(Resource.Id.ActionResetPassword).SetBackgroundColor(GROUP_COLOR);
		}

		private bool Validate()
		{
			invalidEmail.Visibility = ViewStates.Visible;
			
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

			return isValid;
		}

		void ActionResetPassword(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_FORGOT_PASSWORD);

					string isSuccess = GetCode(txtEmail.Text);

					HideLoadingView();

					if (isSuccess == "1")
					{
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("");
						alert.SetMessage(Constants.MSG_FORGOT_PW_SUC);
						alert.SetPositiveButton("OK", (senderAlert, args) =>
						{
							RunOnUiThread(() => { base.OnBackPressed(); });
						});
						RunOnUiThread(() =>
						{
							alert.Show();
						});
					}
					else if (isSuccess == "0")
					{
						ShowMessageBox(null, Constants.MSG_FORGOT_PW_EMAIL_FAIL);
					}
				});

			}
		}

		void ActionBack(object sender, EventArgs e)
		{
			base.OnBackPressed();
		}
	}
}
