using Foundation;
using System;
using UIKit;
using PortableLibrary;
using PortableLibrary.Model;
using System.Threading;

namespace location2
{
    public partial class AdjustTrainningController : BaseViewController
    {
		public GoHejaEvent selectedEvent;
        public ReportData selectedEventReport;

        Constants.EVENT_TYPE _pType;

        public AdjustTrainningController() : base()
		{
		}
		public AdjustTrainningController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

            InitUISettings();

            if (!IsNetEnable()) return;

            ThreadPool.QueueUserWorkItem(delegate
            {
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});
		}

		void InitUISettings()
		{
			viewType.BackgroundColor = GROUP_COLOR;
			attended.OnTintColor = GROUP_COLOR;
			attended.TintColor = GROUP_COLOR;
			seekTime.ThumbTintColor = GROUP_COLOR;
			seekTime.TintColor = GROUP_COLOR;
			seekDistance.ThumbTintColor = GROUP_COLOR;
			seekDistance.TintColor = GROUP_COLOR;
			seekTSS.ThumbTintColor = GROUP_COLOR;
			seekTSS.TintColor = GROUP_COLOR;
			btnSaveAdjust.BackgroundColor = GROUP_COLOR;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			strType.Text = GetTypeStrFromID(selectedEvent.type);

			_pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(selectedEvent.type));

			switch (_pType)
			{
				case Constants.EVENT_TYPE.OTHER:
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
				case Constants.EVENT_TYPE.BIKE:
					imgType.Image = UIImage.FromFile("icon_bike.png");
					break;
				case Constants.EVENT_TYPE.RUN:
					imgType.Image = UIImage.FromFile("icon_run.png");
					break;
				case Constants.EVENT_TYPE.SWIM:
					imgType.Image = UIImage.FromFile("icon_swim.png");
					break;
				case Constants.EVENT_TYPE.TRIATHLON:
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case Constants.EVENT_TYPE.ANOTHER:
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
			}

            InitBindingEventTotal();
		}

		void InitBindingEventTotal()
		{
			attended.On = selectedEvent.attended == "1" ? true : false;

			seekDistance.MaxValue = _pType == Constants.EVENT_TYPE.SWIM ? 10 : 250;

			txtTime.ShouldChangeCharacters = ActionChangeSliderValue;
			txtDistance.ShouldChangeCharacters = ActionChangeSliderValue;
			txtTss.ShouldChangeCharacters = ActionChangeSliderValue;

			if (selectedEventReport == null || selectedEventReport.data == null) return;

			var strEt = GetFormatedDurationAsMin(selectedEventReport.GetTotalValue(Constants.TOTALS_ES_TIME));
			var strTd = selectedEventReport.GetTotalValue(Constants.TOTALS_DISTANCE);
			var strTss = selectedEventReport.GetTotalValue(Constants.TOTALS_LOAD);

			txtTime.Text = strEt.ToString();
			txtTss.Text = float.Parse(strTss).ToString("F1");

			seekTime.Value = strEt;
			seekTSS.Value = float.Parse(strTss);

			var valDistance = float.Parse(strTd);
			if (_pType == Constants.EVENT_TYPE.SWIM)
			{
				if (valDistance > 10)
				{
					txtDistance.Text = "10";
					seekDistance.Value = 10;
				}
				else
				{
					txtDistance.Text = valDistance.ToString("F1");
					seekDistance.Value = valDistance;
				}
			}
			else
			{
				txtDistance.Text = valDistance.ToString("F1");
				seekDistance.Value = valDistance;
			}
		}

		partial void ActionSwitchType(UIButton sender)
		{
			SelectPTypeViewController ptVC = Storyboard.InstantiateViewController("SelectPTypeViewController") as SelectPTypeViewController;
			ptVC.selectedEvent = selectedEvent;

			NavigationController.PushViewController(ptVC, true);
		}

		bool ActionChangeSliderValue(UITextField textField, NSRange range, string replacementString)
		{
			int maxValue = 0;
			UISlider seekBar = null;
			switch (textField.Tag)
			{
				case 0:
					maxValue = 360;
					seekBar = seekTime;
					break;
				case 1:
					maxValue = _pType == Constants.EVENT_TYPE.SWIM ? 10 : 250;
					seekBar = seekDistance;
					break;
				case 2:
					maxValue = 400;
					seekBar = seekTSS;
					break;
			}

			string newValue = "";
			using (NSString original = new NSString(textField.Text), replace = new NSString(replacementString.ToUpper()))
			{
				newValue = original.Replace(range, replace);
			}
			var nValue = newValue == "" ? 0 : float.Parse(newValue);
			if (nValue >= 0 && nValue <= maxValue)
			{
				seekBar.Value = nValue;
				textField.Text = newValue;
			}
			return false;
		}

		partial void ActionDataChanged(UISlider sender)
		{
			switch (sender.Tag)
			{
				case 0:
					txtTime.Text = ((int)sender.Value).ToString();
					break;
				case 1:
					txtDistance.Text = sender.Value.ToString("F1");
					break;
				case 2:
					txtTss.Text = sender.Value.ToString("F1");
					break;
			}
		}

		partial void ActionAdjustTrainning(UIButton sender)
		{
			if (!IsNetEnable()) return;

			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

				InvokeOnMainThread(() =>
				{
					var authorID = AppSettings.CurrentUser.userId;

					UpdateMemberNotes(string.Empty, authorID, selectedEvent._id, MemberModel.username, attended.On ? "1" : "0", txtTime.Text, txtDistance.Text, txtTss.Text, selectedEvent.type);

					HideLoadingView();
					NavigationController.PopViewController(true);
				});
			});
		}
    }
}