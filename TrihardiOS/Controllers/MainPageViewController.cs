using System;
using System.Collections.Generic;
using PortableLibrary;
using UIKit;

namespace location2
{
	public partial class MainPageViewController : BaseViewController
	{
		List<UINavigationController> subControllers = new List<UINavigationController>();
		int nCurrentIndex = -1;

		public MainPageViewController() : base()
		{
		}
		public MainPageViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.baseVC = this;

			AddSubController("CalendarHomeViewController");
			AddSubController("PracticeSelectionViewController");
			AddSubController("ProfileViewController");

			SetCurrentPage(0);

			btnCalendar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnHome.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnProfile.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			constraintTabBarHeight.Constant = AppSettings.isFakeUser ? 0 : 45;
			btnCalendar.Hidden = AppSettings.isFakeUser ? true : false;
			btnHome.Hidden = AppSettings.isFakeUser ? true : false;
			btnProfile.Hidden = AppSettings.isFakeUser ? true : false;
		}

		void AddSubController(string vcIdentifier)
		{
			var tabVC = (BaseViewController)this.Storyboard.InstantiateViewController(vcIdentifier);
			tabVC.rootVC = this;
			var tabNavVC = new UINavigationController(tabVC);

			tabNavVC.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			tabNavVC.View.BackgroundColor = UIColor.Clear;
			tabNavVC.NavigationBar.BackgroundColor = UIColor.Clear;
			tabNavVC.NavigationBar.ShadowImage = new UIImage();

            if (AppSettings.CurrentUser.userType == (int)Constants.USER_TYPE.COACH && vcIdentifier == "CalendarHomeViewController")
            {
                var leftButton = NavLeftButton();
                leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
                tabVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);
            }
            else
            {
                tabVC.NavigationItem.LeftBarButtonItem = null;
            }

			subControllers.Add(tabNavVC);

			tabNavVC.View.Frame = pageContent.Frame;

			pageContent.AddSubview(tabNavVC.View);

			tabNavVC.View.Hidden = true;
		}

		partial void ActionTab(UIButton sender)
		{
			SetCurrentPage((int)sender.Tag);
		}

		public void SetCurrentPage(int pIndex)
		{
			if (nCurrentIndex == pIndex) return;

			if (nCurrentIndex != -1)
				subControllers[nCurrentIndex].View.Hidden = true;

			nCurrentIndex = pIndex;

			subControllers[nCurrentIndex].View.Hidden = false;

			TabBarAnimation(pIndex);

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.navVC = subControllers[nCurrentIndex];
		}

		public void TabBarAnimation(int pageNumber)
		{
			btnCalendar.Selected = false;
			btnHome.Selected = false;
			btnProfile.Selected = false;

			switch (pageNumber)
			{
				case 0:
					btnCalendar.Selected = true;
					break;
				case 1:
					btnHome.Selected = true;
					break;
				case 2:
					btnProfile.Selected = true;
					break;
			}
		}
	}
}


