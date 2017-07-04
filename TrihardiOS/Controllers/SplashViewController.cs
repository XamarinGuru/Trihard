using System;
using UIKit;
using System.Threading.Tasks;
using PortableLibrary;

namespace location2
{
    public partial class SplashViewController : BaseViewController
    {
        public SplashViewController (IntPtr handle) : base (handle)
        {
        }

		async public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			await Task.Delay(100);

			GotoMainIfAlreadyLoggedin();
		}

		private void GotoMainIfAlreadyLoggedin()
		{
            if (!IsNetEnable()) return;

			var nextVC = Storyboard.InstantiateViewController("InitViewController");

			var currentUser = AppSettings.CurrentUser;
			if (currentUser != null)
			{
				if (currentUser.userType == Constants.USER_TYPE.ATHLETE)
				{
					nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				}
				else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
				{
					var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
					nextVC = new UINavigationController(tabVC);

					AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
					myDelegate.navVC = nextVC as UINavigationController;
				}
			}

			PresentViewController(nextVC, false, null);
		}
    }
}
