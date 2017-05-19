using System;
using UIKit;
using PortableLibrary;
using CoreGraphics;

namespace location2
{
    public partial class EventInstructionController : BaseViewController
    {
		public GoHejaEvent selectedEvent;
		EventTotal eventTotal;
		string eventID;

		float fDistance = 0;
		float fDuration = 0;
		float fLoad = 0;

        public EventInstructionController() : base()
		{
		}
		public EventInstructionController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			eventID = selectedEvent._id;

			InitUISettings();

			if (!IsNetEnable()) return;

			InitBindingEventData();
		}

		void InitUISettings()
		{
			if (DateTime.Compare(selectedEvent.StartDateTime(), DateTime.Now) > 0)
				heightAdjust.Constant = 0;
			else
				heightAdjust.Constant = 100;

			lblPDistance.TextColor = GROUP_COLOR;
			lblPDuration.TextColor = GROUP_COLOR;
			lblPLoad.TextColor = GROUP_COLOR;
			lblPHB.TextColor = GROUP_COLOR;

			lblTotalValue0.TextColor = GROUP_COLOR;
			lblTotalValue1.TextColor = GROUP_COLOR;
			lblTotalValue2.TextColor = GROUP_COLOR;
			lblTotalValue3.TextColor = GROUP_COLOR;
			lblTotalValue4.TextColor = GROUP_COLOR;
			lblTotalValue5.TextColor = GROUP_COLOR;
			lblTotalValue6.TextColor = GROUP_COLOR;
			lblTotalValue7.TextColor = GROUP_COLOR;
			lblTotalValue8.TextColor = GROUP_COLOR;

			btnAdjust.BackgroundColor = GROUP_COLOR;
			btnAddComment.BackgroundColor = GROUP_COLOR;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				selectedEvent = GetEventDetail(selectedEvent._id);
				selectedEvent._id = eventID;
				eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				InvokeOnMainThread(() =>
				{
					InitBindingEventData();
					InitBindingEventTotal();
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitBindingEventData()
		{
			var startDateFormats = String.Format("{0:f}", selectedEvent.StartDateTime());
			lblTitle.Text = selectedEvent.title;
			lblStartDate.Text = startDateFormats;
			lblData.Text = selectedEvent.eventData;

			var strDistance = selectedEvent.distance;
			fDistance = strDistance == "" || strDistance == null ? 0 : float.Parse(strDistance);
			var b = Math.Truncate(fDistance * 100);
			var c = b / 100;
			var formattedDistance = c.ToString("F2");

			lblPDistance.Text = FormatNumber(formattedDistance) + " KM";

			var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
			var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
			var pHrs = durMin / 60;
			fDuration = (durHrs * 60 + durMin) * 60;
			durHrs = durHrs + pHrs;
			durMin = durMin % 60;
			var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

			fLoad = selectedEvent.tss == "" ? 0 : float.Parse(selectedEvent.tss);

			lblPDuration.Text = FormatNumber(strDuration);
			lblPLoad.Text = FormatNumber(selectedEvent.tss);
			lblPHB.Text = FormatNumber(selectedEvent.hb);

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
		}

		void InitBindingEventTotal()
		{
			if (eventTotal == null || eventTotal.totals == null)
			{
				heightInstructions.Constant = 0;
				return;
			}

			heightInstructions.Constant = 320;

			lblTotalValue0.Text = FormatNumber(eventTotal.totals[0].value);
			lblTotalValue1.Text = FormatNumber(eventTotal.totals[1].value);
			lblTotalValue2.Text = FormatNumber(eventTotal.totals[2].value);
			lblTotalValue3.Text = FormatNumber(eventTotal.totals[3].value);
			lblTotalValue4.Text = FormatNumber(eventTotal.totals[4].value);
			lblTotalValue5.Text = FormatNumber(eventTotal.totals[5].value);
			lblTotalValue6.Text = FormatNumber(eventTotal.totals[6].value);
			lblTotalValue7.Text = FormatNumber(eventTotal.totals[7].value);
			lblTotalValue8.Text = FormatNumber(eventTotal.totals[8].value);

			CompareEventResult(fDistance, ConvertToFloat(eventTotal.totals[1].value), lblPDistance, lblTotalValue1);
			CompareEventResult(fDuration, TotalSecFromString(eventTotal.totals[2].value), lblPDuration, lblTotalValue2);
			CompareEventResult(fLoad, ConvertToFloat(eventTotal.totals[7].value), lblPLoad, lblTotalValue7);
		}

		void InitBindingEventComments(Comment comments)
		{
			foreach (var subView in contentComment.Subviews)
				subView.RemoveFromSuperview();

			lblCommentTitle.Text = "COMMENT" + " (" + comments.comments.Count + ")";

			nfloat posY = 0;
			foreach (var comment in comments.comments)
			{
				CommentView cv = CommentView.Create();
				var height = cv.SetView(comment);
				cv.Frame = new CGRect(0, posY, contentComment.Frame.Size.Width, height);
				contentComment.AddSubview(cv);

				posY += height;
			}

			heightCommentContent.Constant = posY;
		}

		#region Actions
		partial void ActionAdjustTrainning(UIButton sender)
		{
			AdjustTrainningController atVC = Storyboard.InstantiateViewController("AdjustTrainningController") as AdjustTrainningController;
			atVC.selectedEvent = selectedEvent;
			atVC.eventTotal = eventTotal;

			NavigationController.PushViewController(atVC, true);
		}

		partial void ActionLocation(UIButton sender)
		{
			LocationViewController locVC = Storyboard.InstantiateViewController("LocationViewController") as LocationViewController;
			locVC.eventID = selectedEvent._id;
			NavigationController.PushViewController(locVC, true);
		}

		partial void ActionAddComment(UIButton sender)
		{
			AddCommentViewController acVC = Storyboard.InstantiateViewController("AddCommentViewController") as AddCommentViewController;
			acVC.selectedEvent = selectedEvent;

			NavigationController.PushViewController(acVC, true);
		}
		#endregion
    }
}