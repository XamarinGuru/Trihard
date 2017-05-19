using System;
using UIKit;

namespace location2
{
	public partial class InitViewController : BaseViewController
	{
		public InitViewController() : base()
		{
		}
		public InitViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();
		}

		void InitUISettings()
		{
			btnSignUp.BackgroundColor = GROUP_COLOR;
		}

		partial void ActionSignIn(UIButton sender)
		{
			LoginViewController mainVC = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
			this.PresentViewController(mainVC, true, null);
		}

		partial void ActionSignUp(UIButton sender)
		{
			SignUpViewController mainVC = Storyboard.InstantiateViewController("SignUpViewController") as SignUpViewController;
			this.PresentViewController(mainVC, true, null);
		}
	}
}

