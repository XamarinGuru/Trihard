using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using System.Threading.Tasks;

namespace location2
{
	partial class SignUpViewController : BaseViewController
	{
		bool termsAccepted = false;

		public SignUpViewController() : base()
		{
		}
		public SignUpViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			InitUISettings();
		}

		void InitUISettings()
		{
			btnSignUP.BackgroundColor = GROUP_COLOR;
		}

		partial void ActionBack(UIButton sender)
		{
			InitViewController mainVC = Storyboard.InstantiateViewController("InitViewController") as InitViewController;
			this.PresentViewController(mainVC, false, null);
		}

		private bool Validate()
		{
			btnValidFirstname.Hidden = false;
			btnValidLastname.Hidden = false;
			btnValidUsername.Hidden = false;
			btnValidPassword.Hidden = false;
			btnValidEmail.Hidden = false;
			btnValidAge.Hidden = false;
			btnValidTerms.Hidden = false;

			bool isValid = true;

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, true);
				isValid = false;
			} else
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, false);
			}

			if (txtFirstName.Text.Length <= 0)
			{
				MarkAsInvalide(btnValidFirstname, viewErrorFirstname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidFirstname, viewErrorFirstname, false);
			}

			if (txtLastName.Text.Length <= 0)
			{
				MarkAsInvalide(btnValidLastname, viewErrorLastname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidLastname, viewErrorLastname, false);
			}

			if (txtNickName.Text.Length <= 0)
			{
				txtUsername.Text = "You must enter a valid user name.";
				MarkAsInvalide(btnValidUsername, viewErrorUsername, true);
				isValid = false;
			}
			else if (txtNickName.Text.Length >= 8)
			{
				txtUsername.Text = "User name length should be shorter then 8 characters";
				MarkAsInvalide(btnValidUsername, viewErrorUsername, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidUsername, viewErrorUsername, false);
			}

			int testage = 0;
			int.TryParse(txtAge.Text, out testage);
			if (txtAge.Text.Length < 1 || txtAge.Text.Length > 2 || testage < 8 || testage > 90)
			{
				MarkAsInvalide(btnValidAge, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidAge, null, false);
			}

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, false);
			}

			if (termsAccepted == false)
			{
				MarkAsInvalide(btnValidTerms, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidTerms, null, false);
			}

			return isValid;
		}

		#region keyboard process
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtEmail.IsEditing && !txtPassword.IsEditing)
				return;

			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 2;

			if (scroll_amount > 0)
			{
				moveViewUp = true;
				ScrollTheView(moveViewUp);
			}
			else {
				moveViewUp = false;
			}
		}
		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}
		private void ScrollTheView(bool move)
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = this.View.Frame;

			if (move)
			{
				frame.Y = -(scroll_amount);
			}
			else {
				frame.Y = 0;
			}

			this.View.Frame = frame;
			UIView.CommitAnimations();
		}
		#endregion

		#region event handler
		async partial void ActionSignUp(UIButton sender)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
				try
				{
					string deviceUDID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

					var result = "";

					ShowLoadingView(Constants.MSG_SIGNUP);
					await Task.Run(() =>
					{
						InvokeOnMainThread(() => { result = RegisterUser(txtFirstName.Text, txtLastName.Text, deviceUDID, txtNickName.Text, txtPassword.Text, txtEmail.Text, int.Parse(txtAge.Text)); });
					});

					if (result == "user added")
						GoToMainPage(deviceUDID);
					else
					{
						await Task.Run(() =>
						{
							HideLoadingView();
						});

						ShowMessageBox(null, result);
					}
				}
				catch (Exception err)
				{
					await Task.Run(() =>
					{
						HideLoadingView();
					});
					ShowMessageBox(null, err.Message.ToString());
				}
			}
		}

		partial void ActionAcceptTerms(UIButton sender)
		{
			if (!IsNetEnable()) return;

			sender.Selected = !sender.Selected;

			if (sender.Selected && !ValidateUserNickName(txtNickName.Text))
			{
				ShowMessageBox(null, "Nick name is taken, try another.");
				sender.Selected = false;
				return;
			}
			termsAccepted = true;
		}

		partial void ActionTerms(UIButton sender)
		{
			if (!IsNetEnable()) return;

			UIApplication.SharedApplication.OpenUrl(new NSUrl(Constants.URL_TERMS));
		}

		#endregion

		async private void GoToMainPage(string deviceUDID)
		{
			AppSettings.Email = txtEmail.Text;
			AppSettings.Password = txtPassword.Text;
			AppSettings.Username = txtNickName.Text;
			AppSettings.DeviceUDID = deviceUDID;

			string userID = "0";
			await Task.Run(() =>
			{
				InvokeOnMainThread(() => { userID = GetUserID(); });
				HideLoadingView();
			});

			if (userID == "0")//if the user not registered yet, go to register screen
			{
				ShowMessageBox(null, Constants.MSG_SIGNUP_FAIL);
			}
			else//if the user already registered, go to main screen
			{
				MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				this.PresentViewController(mainVC, false, null);
			}
			//ThreadPool.QueueUserWorkItem(delegate
			//{
			//	BeginInvokeOnMainThread(delegate
			//	{
			//		//register device id to keychain
			//		var s = new SecRecord(SecKind.GenericPassword)
			//		{
			//			Label = "Item Label",
			//			Description = "Item description",
			//			Account = "Account",
			//			Service = "Service",
			//			Comment = "Your comment here",
			//			ValueData = deviceID,
			//			Generic = NSData.FromString("foo")
			//		};


			//		var err = SecKeyChain.Add(s);

			//		if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
			//			ShowMessageBox(null, "Can't save device id to keychain");

			//		//NSUserDefaults.StandardUserDefaults.SetString(deviceID, "deviceId");
			//		NSUserDefaults.StandardUserDefaults.SetString(txtEmail.Text, "email");
			//		NSUserDefaults.StandardUserDefaults.SetString(txtPassword.Text, "password");

			//		MainPageViewController mainVC = this.Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
			//		this.PresentViewController(mainVC, false, null);
			//	});
			//});
		}
	}
}
