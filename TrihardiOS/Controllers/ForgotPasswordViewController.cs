using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class ForgotPasswordViewController : BaseViewController
    {
        public ForgotPasswordViewController() : base()
		{
		}
		public ForgotPasswordViewController(IntPtr handle) : base(handle)
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
			btnForgotPW.BackgroundColor = GROUP_COLOR;
		}


		private bool Validate()
		{
			btnValidEmail.Hidden = false;

			bool isValid = true;

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, false);
			}

			return isValid;
		}

		partial void ActionResetPassword(UIButton sender)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_FORGOT_PASSWORD);

					string isSuccess = "0";

					InvokeOnMainThread(() => { isSuccess = GetCode(txtEmail.Text); });

					HideLoadingView();

					if (isSuccess == "1")
					{
						ShowMessageBox1(null, Constants.MSG_FORGOT_PW_SUC, "OK", null, ReturnToLogin);
					}
					else if (isSuccess == "0")
					{
						ShowMessageBox(null, Constants.MSG_FORGOT_PW_EMAIL_FAIL);
					}
				});
			}
		}
		void ReturnToLogin()
		{
			InvokeOnMainThread(() =>
			{
				LoginViewController mainVC = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
				this.PresentViewController(mainVC, false, null);
			});
		}

		partial void ActionBack(UIButton sender)
		{
			LoginViewController mainVC = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
			this.PresentViewController(mainVC, false, null);
		}

		#region keyboard process
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtEmail.IsEditing)
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
			UIView.BeginAnimations(string.Empty, IntPtr.Zero);
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
	}
}