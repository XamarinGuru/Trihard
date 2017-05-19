using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class AdjustTrainningController : BaseViewController
    {
		public GoHejaEvent selectedEvent;
		public EventTotal eventTotal;

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

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			//InitBindingEventTotal();
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

			switch (selectedEvent.type)
			{
				case "0":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "1":
					imgType.Image = UIImage.FromFile("icon_bike.png");
					break;
				case "2":
					imgType.Image = UIImage.FromFile("icon_run.png");
					break;
				case "3":
					imgType.Image = UIImage.FromFile("icon_swim.png");
					break;
				case "4":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "5":
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
			}
            InitBindingEventTotal();
		}

		void InitBindingEventTotal()
		{
			attended.On = selectedEvent.attended == "1" ? true : false;

			seekDistance.MaxValue = selectedEvent.type == "3" ? 10 : 250;

			txtTime.ShouldChangeCharacters = ActionChangeSliderValue;
			txtDistance.ShouldChangeCharacters = ActionChangeSliderValue;
			txtTss.ShouldChangeCharacters = ActionChangeSliderValue;

			if (eventTotal == null || eventTotal.totals == null) return;

			var strEt = GetFormatedDurationAsMin(eventTotal.GetValue(Constants.TOTALS_ES_TIME));
			var strTd = eventTotal.GetValue(Constants.TOTALS_DISTANCE);
			var strTss = eventTotal.GetValue(Constants.TOTALS_LOAD);

			txtTime.Text = strEt.ToString();
			//txtDistance.Text = float.Parse(strTd).ToString("F1");
			txtTss.Text = float.Parse(strTss).ToString("F1");

			seekTime.Value = strEt;
			//seekDistance.Value = float.Parse(strTd);
			seekTSS.Value = float.Parse(strTss);

			var valDistance = float.Parse(strTd);
			if (selectedEvent.type == "3")
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
					maxValue = selectedEvent.type == "3" ? 10 : 250;
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

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

				InvokeOnMainThread(() =>
				{
					var authorID = AppSettings.CurrentUser.userId;

					UpdateMemberNotes(txtComment.Text, authorID, selectedEvent._id, MemberModel.username, attended.On ? "1" : "0", txtTime.Text, txtDistance.Text, txtTss.Text, selectedEvent.type);

					HideLoadingView();
					NavigationController.PopViewController(true);
				});
			});
		}

		#region keyboard process
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtComment.IsFirstResponder)
				return;

			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 1.5f;

			if (scroll_amount > 0)
			{
				moveViewUp = true;
				ScrollTheView(moveViewUp);
			}
			else {
				moveViewUp = false;
			}
		}


		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}
		private void ScrollTheView(bool move)
		{
			// scroll the view up or down
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = this.View.Frame;

			if (move)
			{
				frame.Y = -(scroll_amount);
			}
			else {
				frame.Y = 0;
			}

			this.View.Frame = frame;
			UIView.CommitAnimations();
		}
		#endregion
    }
}