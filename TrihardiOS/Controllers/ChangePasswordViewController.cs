using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
	public partial class ChangePasswordViewController : BaseViewController
    {
		public string email;

        public ChangePasswordViewController() : base()
		{
		}
		public ChangePasswordViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

            var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			InitUISettings();
		}

		void InitUISettings()
		{
			btnChangePW.BackgroundColor = GROUP_COLOR;
		}

		private bool Validate()
		{
			btnValidPassword.Hidden = false;
			btnValidPwConfirm.Hidden = false;

			bool isValid = true;

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, false);
			}

			if (txtPwConfirm.Text.Length <= 0 || txtPwConfirm.Text != txtPassword.Text)
			{
				MarkAsInvalide(btnValidPwConfirm, viewErrorPwConfirm, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidPwConfirm, viewErrorPwConfirm, false);
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
					ShowLoadingView(Constants.MSG_CHANGE_PASSWORD);

					int isSuccess = 0;

					InvokeOnMainThread(() => { isSuccess = ResetPassword(email, txtPassword.Text); });

					HideLoadingView();

					if (isSuccess == 1)
					{
						ShowMessageBox1(null, Constants.MSG_CHANGE_PW_SUC, "OK", null, ReturnBack);
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
		void ReturnBack()
		{
			InvokeOnMainThread(() =>
			{
				NavigationController.PopViewController(true);
			});
		}

		#region keyboard process
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtPassword.IsEditing && !txtPwConfirm.IsEditing)
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
	}
}