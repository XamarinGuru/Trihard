using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace location2
{
	public partial class CoachGroupsViewController : BaseViewController
	{
		public CoachGroupsViewController() : base()
		{
		}
		public CoachGroupsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();
		}

		void InitUISettings()
		{
			NavigationController.NavigationBar.Hidden = false;

			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);
		}

		partial void ActionSelectedGroup(UIButton sender)
		{
			var sb = UIStoryboard.FromName("Main", null);
			CoachSubGroupViewController coachSubGroupVC = sb.InstantiateViewController("CoachSubGroupViewController") as CoachSubGroupViewController;
			coachSubGroupVC._group_id = sender.Tag.ToString();
			NavigationController.PushViewController(coachSubGroupVC, true);
		}

		partial void ActionBack(UIButton sender)
		{
			NavigationController.PopViewController(true);
		}
	}
}