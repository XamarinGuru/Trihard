using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class SelectPTypeViewController : BaseViewController
    {
		UIColor COLOR_DISABLE = new UIColor(67 / 255f, 67 / 255f, 67 / 255f, alpha: 1.0f);

		public GoHejaEvent selectedEvent;

        public SelectPTypeViewController() : base()
		{
		}
		public SelectPTypeViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			InitUISettings();
		}

		void InitUISettings()
		{
			stateCycling.BackgroundColor = COLOR_DISABLE;
			stateRunning.BackgroundColor = COLOR_DISABLE;
			stateSwimming.BackgroundColor = COLOR_DISABLE;
			stateTriathlon.BackgroundColor = COLOR_DISABLE;
			stateOther.BackgroundColor = COLOR_DISABLE;

			switch (selectedEvent.type)
			{
				case "1":
					stateCycling.BackgroundColor = GROUP_COLOR;
					break;
				case "2":
					stateRunning.BackgroundColor = GROUP_COLOR;
					break;
				case "3":
					stateSwimming.BackgroundColor = GROUP_COLOR;
					break;
				case "4":
					stateTriathlon.BackgroundColor = GROUP_COLOR;
					break;
				case "5":
					stateOther.BackgroundColor = GROUP_COLOR;
					break;
			}
		}

		partial void ActionSelectedType(UIButton sender)
		{
			selectedEvent.type = sender.Tag.ToString();
			NavigationController.PopViewController(true);
		}
	}
}