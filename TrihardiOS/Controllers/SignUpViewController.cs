﻿using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using System.Threading;

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
		void KeyBoardUpNotification(NSNotification notification)
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
		void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}
		void ScrollTheView(bool move)
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = View.Frame;

			if (move)
			{
				frame.Y = -(scroll_amount);
			}
			else {
				frame.Y = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}
        #endregion

        #region event handler
        partial void ActionSignUp(UIButton sender)
        {
            if (!IsNetEnable()) return;

            if (Validate())
            {
                try
                {
					var strFName = txtFirstName.Text;
					var strLName = txtLastName.Text;
					var strNName = txtNickName.Text;
					var strPW = txtPassword.Text;
					var strEmail = txtEmail.Text;
					var nAge = int.Parse(txtAge.Text);

					ThreadPool.QueueUserWorkItem(delegate
					{
						ShowLoadingView(Constants.MSG_SIGNUP);
                        var result = RegisterUser(strFName, strLName, strNName, strPW, strEmail, nAge);

                        if (result == "user added")
                        {
                            var loginUser = LoginUser(strEmail, strPW);

                            HideLoadingView();

                            InvokeOnMainThread(() =>
	                        {
	                            if (loginUser == null)
	                            {
	                                ShowMessageBox(null, Constants.MSG_LOGIN_FAIL);
	                            }
	                            else
	                            {
	                                UIViewController nextVC;
	                                if (loginUser.userType == Constants.USER_TYPE.ATHLETE)
	                                {
	                                    nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
	                                }
	                                else
	                                {
	                                    var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
	                                    nextVC = new UINavigationController(tabVC);

	                                    AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
	                                    myDelegate.navVC = nextVC as UINavigationController;
	                                }
	                                PresentViewController(nextVC, true, null);
	                            }
	                        });
                        }
                        else
                        {
                            HideLoadingView();
                            ShowMessageBox(null, result);
                        }
                    });
                }
                catch (Exception ex)
                {
                    HideLoadingView();
                    ShowMessageBox(null, ex.Message.ToString());
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
	}
}
