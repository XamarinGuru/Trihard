using System;
using UIKit;
using System.Threading.Tasks;

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
			if (AppSettings.UserID != "0" && AppSettings.UserID != null && AppSettings.UserID != "")
			{
				MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				this.PresentViewController(mainVC, false, null);
			}
			else {
				InitViewController mainVC = Storyboard.InstantiateViewController("InitViewController") as InitViewController;
				this.PresentViewController(mainVC, false, null);
			}
		}
    }
}