using System;
using UIKit;

namespace location2
{
	public partial class PracticeSelectionViewController : BaseViewController
    {
        public PracticeSelectionViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();
		}

		void InitUISettings()
		{

		}

		partial void ActionSelectedType(UIButton sender)
		{
			AnalyticsViewController aVC = Storyboard.InstantiateViewController("AnalyticsViewController") as AnalyticsViewController;
			aVC.pType = int.Parse(sender.Tag.ToString());
			NavigationController.PushViewController(aVC, true);
		}
	}
}