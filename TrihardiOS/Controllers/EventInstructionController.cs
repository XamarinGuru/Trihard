using System;
using UIKit;
using PortableLibrary;
using CoreGraphics;
using PortableLibrary.Model;
using System.Collections.Generic;
using Foundation;
using System.Threading;

namespace location2
{
    public partial class EventInstructionController : BaseViewController
    {
		public string eventID;
        public bool isNotification;
        public string commentID;
        
        GoHejaEvent selectedEvent;
        ReportData reportData;

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

			NavigationController.NavigationBar.Hidden = false;

			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();

			if (!IsNetEnable()) return;

			InitUISettings();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

            ResetUISettings();
		}

        void ResetUISettings()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

                    selectedEvent = GetEventDetail(eventID);
                    selectedEvent._id = eventID;
                    reportData = GetEventReport(eventID);
                    var eventComment = GetComments(eventID);

                    InvokeOnMainThread(() =>
                    {
                        SetAdjustEnable();
                        InitBindingEventPlanned();
                        InitBindingEventReport();
                        InitBindingEventComments(eventComment);
                    });

                    HideLoadingView();
                });
            }
            catch(Exception ex)
            {
                ShowTrackMessageBox(ex.Message);
            }
        }

		void InitUISettings()
		{
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };
            NavigationItem.Title = AppSettings.isFakeUser ? AppSettings.fakeUserName : string.Empty;

            SetActiveTab(0);

            btnEdit.SetTitleColor(GROUP_COLOR, UIControlState.Normal);
            btnEdit.SetTitleColor(GROUP_COLOR, UIControlState.Selected);

            SetEditPerformField();

			btnAdjust.BackgroundColor = GROUP_COLOR;
		}

		void SetAdjustEnable()
		{
			if (DateTime.Compare(selectedEvent.StartDateTime(), DateTime.Now) > 0 || AppSettings.isFakeUser)
			{
                heightAdjust.Constant = 0;
				contentEditBtnHeight.Constant = 0;
			}
			else
			{
				heightAdjust.Constant = 100;
				contentEditBtnHeight.Constant = 30;
			}
		}

		void InitBindingEventPlanned()
		{
            try
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

                lblPlannedDistance.Text = FormatNumber(formattedDistance) + " KM";

                var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
                var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
                var pHrs = durMin / 60;
                fDuration = (durHrs * 60 + durMin) * 60;
                durHrs = durHrs + pHrs;
                durMin = durMin % 60;
                var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

                fLoad = selectedEvent.tss == "" ? 0 : float.Parse(selectedEvent.tss);

                lblPlannedDuration.Text = FormatNumber(strDuration);
                lblPlannedLoad.Text = FormatNumber(selectedEvent.tss);
                lblPlannedAvgHr.Text = FormatNumber(selectedEvent.hb);

                var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(selectedEvent.type));
                switch (pType)
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
            }
            catch(Exception ex)
            {
                ShowTrackMessageBox(ex.Message);
            }
		}

		void InitBindingEventReport()
		{
            if (reportData != null)
			{
				if (reportData.data != null)
					InitBindingEventPerformed(reportData.data);

				if (reportData.lapData != null)
					InitBindingEventLaps(reportData.lapData, reportData.type);
			}
		}

		void InitBindingEventPerformed(List<Item> eventTotal)
		{
			try
			{
				lblPerformedAvgSpeed.Text = FormatNumber(eventTotal[0].value);
				editPerformedDistance.Text = FormatNumber(eventTotal[1].value);

				var strEt = GetFormatedDurationAsMin(eventTotal[2].value);
				editPerformedDuration.Text = strEt.ToString();

				lblPerformedAcent.Text = FormatNumber(eventTotal[3].value);
				lblPerformedAvgHR.Text = FormatNumber(eventTotal[4].value);
				lblPerformedCalories.Text = FormatNumber(eventTotal[5].value);
				lblPerformedAvgPower.Text = FormatNumber(eventTotal[6].value);
				editPerformedLoad.Text = FormatNumber(eventTotal[7].value);
				lblPerformedLeveledPower.Text = FormatNumber(eventTotal[8].value);

				CompareEventResult(fDistance, ConvertToFloat(eventTotal[1].value), lblPlannedDistance, editPerformedDistance);
				CompareEventResult(fDuration, TotalSecFromString(eventTotal[2].value), lblPlannedDuration, editPerformedDuration);
				CompareEventResult(fLoad, ConvertToFloat(eventTotal[7].value), lblPlannedLoad, editPerformedLoad);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

        void InitBindingEventLaps(List<Lap> laps, int type)
        {
            lapHeaderForBikeOrRun.Hidden = true;
            lapHeaderForSwim.Hidden = true;
            lapHeaderForTriathlon.Hidden = true;
            lapHeaderForOther.Hidden = true;

            var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), type);
            switch (pType)
            {
                case Constants.EVENT_TYPE.BIKE:
                case Constants.EVENT_TYPE.RUN:
                    lapHeaderForBikeOrRun.Hidden = false;
                    break;
                case Constants.EVENT_TYPE.SWIM:
                    lapHeaderForSwim.Hidden = false;
                    break;
                case Constants.EVENT_TYPE.TRIATHLON:
                    lapHeaderForTriathlon.Hidden = false;
                    break;
                case Constants.EVENT_TYPE.ANOTHER:
                case Constants.EVENT_TYPE.OTHER:
                    lapHeaderForOther.Hidden = false;
                    break;
            }

            var tblDataSource = new LapsTableViewSource(laps, pType);

            tableView.Source = tblDataSource;
            tableView.ReloadData();
        }

		void InitBindingEventComments(Comments comments)
		{
			foreach (var subView in contentComment.Subviews)
				subView.RemoveFromSuperview();

            if (comments == null || comments.comments.Count == 0) return;

            var commentTitle = comments.comments.Count > 1 ? "COMMENTS" + " (" + comments.comments.Count + ")" : "COMMENT" + " (" + comments.comments.Count + ")";
			lblCommentTitle.Text = commentTitle;

			nfloat posY = 0;
			foreach (var comment in comments.comments)
			{
				CommentView cv = CommentView.Create();
				var height = cv.SetView(comment);

				cv.Frame = new CGRect(0, posY, contentComment.Frame.Size.Width, height);
				contentComment.AddSubview(cv);
				
				posY += height;
				heightCommentContent.Constant = posY;

				if (!string.IsNullOrEmpty(commentID) && commentID.Equals(comment.commentId))
				{
					var bottomOffset = new CGPoint(0, contentComment.Frame.Y + posY - scrollView.Frame.Size.Height);
                    scrollView.SetContentOffset(bottomOffset, true);

					cv.SetHighlight(this);
					
					commentID = null;
				}
			}
		}

		void SetEditPerformField()
		{
			editPerformedDistance.Enabled = btnEdit.Selected;
			editPerformedDuration.Enabled = btnEdit.Selected;
			editPerformedLoad.Enabled = btnEdit.Selected;

            editPerformedDistance.BorderStyle = btnEdit.Selected ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
			editPerformedDuration.BorderStyle = btnEdit.Selected ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
			editPerformedLoad.BorderStyle = btnEdit.Selected ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
		}

		void SetActiveTab(int tabType)
		{
			if (tabType == 0)
			{
                btnTotals.SetTitleColor(GROUP_COLOR, UIControlState.Normal);
				btnLaps.SetTitleColor(UIColor.White, UIControlState.Normal);
                contentTotals.Hidden = false;
				tabTotalsBorder.Hidden = true;
				contentLaps.Hidden = true;
				tabLapsBorder.Hidden = false;
			}
			else
			{
                btnTotals.SetTitleColor(UIColor.White, UIControlState.Normal);
				btnLaps.SetTitleColor(GROUP_COLOR, UIControlState.Normal);
				contentTotals.Hidden = true;
				tabTotalsBorder.Hidden = false;
				contentLaps.Hidden = false;
				tabLapsBorder.Hidden = true;
			}
		}

		#region Actions
		partial void ActionEditPerformed(UIButton sender)
		{
            sender.Selected = !sender.Selected;

            sender.SetTitle(btnEdit.Selected ? "Done" : "Edit", UIControlState.Normal);
            imgEdit.Image = UIImage.FromFile(btnEdit.Selected ? "icon_check.png" : "icon_pencil.png");

			SetEditPerformField();

			if (!sender.Selected)
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);
				
				var authorID = AppSettings.CurrentUser.userId;
				
				var pDuration = ConvertToFloat(editPerformedDuration.Text);
				var pDistance = ConvertToFloat(editPerformedDistance.Text);
				var pLoad = ConvertToFloat(editPerformedLoad.Text);

				ThreadPool.QueueUserWorkItem(delegate
				{
                    UpdateMemberNotes(string.Empty, authorID, selectedEvent._id, string.Empty, selectedEvent.attended, pDuration.ToString(), pDistance.ToString(), pLoad.ToString(), selectedEvent.type);

					HideLoadingView();

					ResetUISettings();
				});
			}
		}

		partial void ActionTab(UIButton sender)
		{
            SetActiveTab((int)sender.Tag);
		}

		partial void ActionAdjustTrainning(UIButton sender)
		{
			AdjustTrainningController atVC = Storyboard.InstantiateViewController("AdjustTrainningController") as AdjustTrainningController;
			atVC.selectedEvent = selectedEvent;
            atVC.selectedEventReport = reportData;

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

		#region UserTableViewSource
		class LapsTableViewSource : UITableViewSource
		{
            List<Lap> _laps = new List<Lap>();
            Constants.EVENT_TYPE _type;

			public LapsTableViewSource(List<Lap> laps, Constants.EVENT_TYPE type)
			{
				if (laps == null) return;
				_laps = laps;
                _type = type;
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return _laps.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 30;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
                BaseLapTableViewCell cell = null;
				switch (_type)
				{
					case Constants.EVENT_TYPE.BIKE:
					case Constants.EVENT_TYPE.RUN:
                        cell = tableView.DequeueReusableCell("LapBikeOrRunCell") as BaseLapTableViewCell;
						break;
					case Constants.EVENT_TYPE.SWIM:
                        cell = tableView.DequeueReusableCell("LapSwimCell") as LapSwimCell;
						break;
					case Constants.EVENT_TYPE.TRIATHLON:
                        cell = tableView.DequeueReusableCell("LapTriathlonCell") as LapTriathlonCell;
						break;
					case Constants.EVENT_TYPE.ANOTHER:
					case Constants.EVENT_TYPE.OTHER:
                        cell = tableView.DequeueReusableCell("LapOtherCell") as LapOtherCell;
						break;
				}

				cell.SetCell(_laps[indexPath.Row]);

				return cell;
			}
		}
		#endregion
	}
}