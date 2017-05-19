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
    [Activity(Label = "listing", ScreenOrientation = ScreenOrientation.Portrait)]
	public class RegisterActivity : BaseActivity
    {
		EditText txtFirstname, txtLastname, txtUsername, txtEmail, txtPassword, txtAge;
		TextView txtErrorUsername;
		ImageView invalidFirstname, invalidLastname, invalidUsername, invalidEmail, invalidPassword, invalidAge, invalidTerms;
		LinearLayout errorFirstname, errorLastname, errorUsername, errorEmail, errorPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegisterActivity);

			InitUI();
        }

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionSignUp).Click += ActionSignUp;
			FindViewById<Button>(Resource.Id.ActionTerms).Click += ActionTerms;

			txtFirstname = FindViewById<EditText>(Resource.Id.txtFirstname);
			txtLastname = FindViewById<EditText>(Resource.Id.txtLastname);
			txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtAge = FindViewById<EditText>(Resource.Id.txtAge);
			txtErrorUsername = FindViewById<TextView>(Resource.Id.txtErrorUsername);

			invalidFirstname = FindViewById<ImageView>(Resource.Id.invalidFirstname);
			invalidLastname = FindViewById<ImageView>(Resource.Id.invalidLastname);
			invalidUsername = FindViewById<ImageView>(Resource.Id.invalidUsername);
			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			invalidAge = FindViewById<ImageView>(Resource.Id.invalidAge);
			invalidTerms = FindViewById<ImageView>(Resource.Id.invalidTerms);
			errorFirstname = FindViewById<LinearLayout>(Resource.Id.errorFirstname);
			errorLastname = FindViewById<LinearLayout>(Resource.Id.errorLastname);
			errorUsername = FindViewById<LinearLayout>(Resource.Id.errorUsername);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);

			invalidFirstname.Visibility = ViewStates.Invisible;
			invalidLastname.Visibility = ViewStates.Invisible;
			invalidUsername.Visibility = ViewStates.Invisible;
			invalidEmail.Visibility = ViewStates.Invisible;
			invalidPassword.Visibility = ViewStates.Invisible;
			invalidAge.Visibility = ViewStates.Invisible;
			invalidTerms.Visibility = ViewStates.Invisible;
			errorFirstname.Visibility = ViewStates.Invisible;
			errorLastname.Visibility = ViewStates.Invisible;
			errorUsername.Visibility = ViewStates.Invisible;
			errorEmail.Visibility = ViewStates.Invisible;
			errorPassword.Visibility = ViewStates.Invisible;

			FindViewById<Button>(Resource.Id.ActionSignUp).SetBackgroundColor(GROUP_COLOR);
		}

		private bool Validate()
		{
			var checkTerms = FindViewById<CheckBox>(Resource.Id.checkTerms);

			invalidFirstname.Visibility = ViewStates.Visible;
			invalidLastname.Visibility = ViewStates.Visible;
			invalidUsername.Visibility = ViewStates.Visible;
			invalidEmail.Visibility = ViewStates.Visible;
			invalidPassword.Visibility = ViewStates.Visible;
			invalidAge.Visibility = ViewStates.Visible;
			invalidTerms.Visibility = ViewStates.Visible;

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

			if (txtFirstname.Text.Length <= 0)
			{
				MarkAsInvalide(invalidFirstname, errorFirstname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidFirstname, errorFirstname, false);
			}

			if (txtLastname.Text.Length <= 0)
			{
				MarkAsInvalide(invalidLastname, errorLastname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidLastname, errorLastname, false);
			}

			if (txtUsername.Text.Length <= 0)
			{
				txtErrorUsername.Text = "You must enter a valid user name.";
				MarkAsInvalide(invalidUsername, errorUsername, true);
				isValid = false;
			}
			else if (txtUsername.Text.Length >= 8)
			{
				txtErrorUsername.Text = "User name length should be shorter then 8 characters";
				MarkAsInvalide(invalidUsername, errorUsername, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidUsername, errorUsername, false);
			}

			int testage = 0;
			int.TryParse(txtAge.Text, out testage);
			if (txtAge.Text.Length < 1 || txtAge.Text.Length > 2 || testage < 8 || testage > 90)
			{
				MarkAsInvalide(invalidAge, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidAge, null, false);
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

			if (!checkTerms.Checked)
			{
				MarkAsInvalide(invalidTerms, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidTerms, null, false);
			}

			return isValid;
		}

        private void ActionSignUp(object sender, EventArgs eventArgs)
        {
			if (!IsNetEnable()) return;

            try
            {
				if (Validate())
				{
					string deviceUDID = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

					System.Threading.ThreadPool.QueueUserWorkItem(delegate
					{
						ShowLoadingView(Constants.MSG_SIGNUP);

						var result = RegisterUser(txtFirstname.Text, txtLastname.Text, deviceUDID, txtUsername.Text, txtPassword.Text, txtEmail.Text, int.Parse(txtAge.Text));

						if (result == "user added")
						{
							var loginUser = LoginUser(txtEmail.Text, txtPassword.Text);

							HideLoadingView();

							if (loginUser.userId == null)
							{
								ShowMessageBox(null, Constants.MSG_SIGNUP_FAIL);
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
						}
						else
						{
                            HideLoadingView();
							ShowMessageBox(null, result);
						}
					});
				}
            }
            catch (Exception ex)
            {
                HideLoadingView();
				ShowMessageBox(null, ex.Message.ToString());
            }
        }

		private void ActionTerms(object sender, EventArgs eventArgs)
		{
			if (!IsNetEnable()) return;

			var uri = Android.Net.Uri.Parse(Constants.URL_TERMS);
			var intent = new Intent(Intent.ActionView, uri);
			StartActivity(intent);
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }
    }
}
