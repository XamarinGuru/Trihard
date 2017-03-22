
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
	[Activity(Label = "ChangePasswordActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class ChangePasswordActivity : BaseActivity
	{
		EditText txtPassword, txtPwConfirm;
		ImageView invalidPassword, invalidPwConfirm;
		LinearLayout errorPassword, errorPwConfirm;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ChangePasswordActivity);

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionResetPassword).Click += ActionResetPassword;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;

			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtPwConfirm = FindViewById<EditText>(Resource.Id.txtPwConfirm);

			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);
			invalidPwConfirm = FindViewById<ImageView>(Resource.Id.invalidPwConfirm);
			errorPwConfirm = FindViewById<LinearLayout>(Resource.Id.errorPwConfirm);

			invalidPassword.Visibility = ViewStates.Invisible;
			invalidPwConfirm.Visibility = ViewStates.Invisible;
			errorPassword.Visibility = ViewStates.Invisible;
			errorPwConfirm.Visibility = ViewStates.Invisible;

			FindViewById<Button>(Resource.Id.ActionResetPassword).SetBackgroundColor(GROUP_COLOR);
		}

		private bool Validate()
		{
			invalidPassword.Visibility = ViewStates.Visible;
			invalidPwConfirm.Visibility = ViewStates.Visible;
			
			bool isValid = true;

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(invalidPassword, errorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidPassword, errorPassword, false);
			}

			if (txtPwConfirm.Text.Length <= 0 || txtPwConfirm.Text != txtPassword.Text)
			{
				MarkAsInvalide(invalidPwConfirm, errorPwConfirm, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidPwConfirm, errorPwConfirm, false);
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
					ShowLoadingView(Constants.MSG_CHANGE_PASSWORD);

					int isSuccess = ResetPassword(AppSettings.currentEmail, txtPassword.Text);

					HideLoadingView();

					if (isSuccess == 1)
					{
						ShowMessageBox(null, Constants.MSG_CHANGE_PW_SUC, "Cancel", new[] { "OK" }, () => base.OnBackPressed());
					}
					else if (isSuccess == 2)
					{
						ShowMessageBox(null, Constants.MSG_CHANGE_PW_FAIL);
					}
					else if (isSuccess == 3)
					{
						ShowMessageBox(null, Constants.MSG_CHANGE_PW_EMAIL_FAIL);
					}
				});
			}
		}

		void ActionBack(object sender, EventArgs e)
		{
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
