using System;
using UIKit;
using System.Threading.Tasks;
using PortableLibrary;

namespace location2
{
    public partial class SplashViewController : UIViewController
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
			var nextVC = Storyboard.InstantiateViewController("InitViewController");

			var currentUser = AppSettings.CurrentUser;
			if (currentUser != null)
			{
				if (currentUser.userType == (int)Constants.USER_TYPE.ATHLETE)
				{
					nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				}
				else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
				{
					var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
					nextVC = new UINavigationController(tabVC);
					//nextVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
				}
			}

			this.PresentViewController(nextVC, false, null);
		}
    }
}
