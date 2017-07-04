using System;
using UIKit;
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

			var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(selectedEvent.type));
			switch (pType)
			{
				case Constants.EVENT_TYPE.OTHER:
					stateOther.BackgroundColor = GROUP_COLOR;
					break;
				case Constants.EVENT_TYPE.BIKE:
					stateCycling.BackgroundColor = GROUP_COLOR;
					break;
				case Constants.EVENT_TYPE.RUN:
					stateRunning.BackgroundColor = GROUP_COLOR;
					break;
				case Constants.EVENT_TYPE.SWIM:
					stateSwimming.BackgroundColor = GROUP_COLOR;
					break;
				case Constants.EVENT_TYPE.TRIATHLON:
					stateTriathlon.BackgroundColor = GROUP_COLOR;
					break;
				case Constants.EVENT_TYPE.ANOTHER:
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