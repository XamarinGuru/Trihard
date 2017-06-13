using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using System.Collections.Generic;
using Xuni.iOS.Calendar;

namespace location2
{
	public partial class EventCalendarViewController : BaseViewController
    {
		UIColor COLOR_PAST = new UIColor(229 / 255f, 161 / 255f, 9 / 255f, 1.0f);
		UIColor COLOR_FUTURE = new UIColor(63 / 255f, 187 / 255f, 190 / 255f, 1.0f);

		List<GoHejaEvent> _events = new List<GoHejaEvent>();

        public EventCalendarViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ReloadEvents();

			InitUISettings();
		}

		void InitUISettings()
		{
			#region xuni calendar
			calendar.Orientation = XuniCalendarOrientation.Vertical;
			calendar.MaxSelectionCount = 1;
			calendar.BackgroundColor = UIColor.FromRGB(29 / 255f, 29 / 255f, 29 / 255f);

			calendar.CalendarFont = UIFont.BoldSystemFontOfSize (14);
			calendar.TodayFont = UIFont.BoldSystemFontOfSize (14);

			calendar.TodayTextColor = UIColor.Red;
			calendar.AdjacentDayTextColor = UIColor.Gray;
			calendar.TextColor = UIColor.White;
			calendar.SelectionBackgroundColor = GROUP_COLOR;

			calendar.DaySlotLoading += CalendarDaySlotLoading;
			calendar.SelectionChanged += CalendarSelectionChanged;
			#endregion

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			var rightButton = new UIButton(new CGRect(0, 0, 70, 20));
			rightButton.SetTitle("Reload", UIControlState.Normal);
			rightButton.TouchUpInside += (sender, e) => ReloadEvents();

			var rightButton1 = new UIButton(new CGRect(100, 0, 70, 20));
			rightButton1.SetTitle("Today", UIControlState.Normal);
			rightButton1.TouchUpInside += (sender, e) => GotoToday();

			UIBarButtonItem[] rightButtons = { new UIBarButtonItem(rightButton), new UIBarButtonItem(rightButton1) };

			NavigationItem.RightBarButtonItems = rightButtons;

			lblTSB.TextColor = GROUP_COLOR;
			lblCTL.TextColor = GROUP_COLOR;
			lblATL.TextColor = GROUP_COLOR;
			lblLoad.TextColor = GROUP_COLOR;
			lblNoEvents.TextColor = GROUP_COLOR;
		}

		void CalendarSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var filterDate = (sender as XuniCalendar).SelectedDate;
			FilterEventsByDate(filterDate);
		}

		CalendarDaySlotBase CalendarDaySlotLoading(XuniCalendar sender, NSDate date, bool isAdjacentDay, CalendarDaySlotBase daySlot)
		{
			CGRect rect = daySlot.Frame;
			CGSize size = rect.Size;

			CalendarImageDaySlot imageDaySlot = new CalendarImageDaySlot(sender, rect);

			try
			{
				if (_events != null && _events.Count != 0)
				{
					for (int i = 0; i<_events.Count; i++)
					{
						var startDate = _events[i].StartDateTime();
						if (startDate.Date == NSDateToDateTime(date))
						{
							imageDaySlot.ImageRect = new CGRect(size.Width / 2 - 6 / 2, size.Height / 6 * 5, 5, 5);
							imageDaySlot.ImageSource = UIImage.FromFile("slider-default-handle-disabled.png");
						}
					}
				}

			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
			return imageDaySlot;
		}

		void ReloadEvents()
		{
			if (!IsNetEnable()) return;

			_events = new List<GoHejaEvent>();
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				InvokeOnMainThread(() =>
				{
					calendar.Refresh();
                    FilterEventsByDate(DateTime.Now);
					calendar.SelectedDate = DateTime.Now;

					HideLoadingView();
				});
			});
		}

		void GotoToday()
		{
			calendar.ChangeViewModeAsync(XuniCalendarViewMode.Month, (NSDate)DateTime.Now);
		}

		void FilterEventsByDate(DateTime filterDate)
		{
			var eventsByDate = new List<GoHejaEvent>();
			foreach (var goHejaEvent in _events)
			{
				var eventDate = goHejaEvent.StartDateTime();
				if (filterDate.Date == eventDate.Date)
				{
					eventsByDate.Add(goHejaEvent);
				}
			}
			if (DateTime.Compare(filterDate, DateTime.Now) > 0)
				SetPerformanceDataColor(true);
			else
				SetPerformanceDataColor(false);

			if (eventsByDate.Count == 0)
				lblNoEvents.Hidden = false;
			else
				lblNoEvents.Hidden = true;
			
			var tblDataSource = new GoHejaEventTableViewSource(eventsByDate, this);
			this.InvokeOnMainThread(delegate
			{
				tableView.Source = tblDataSource;
				tableView.ReloadData();

				SetTableViewHeightBasedOnChildren(tableView, eventsByDate, heightEventDetail);
				InitPerformanceData(filterDate);
			});
		}

		void InitPerformanceData(DateTime date)
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var performanceData = GetPerformanceForDate(date);

				HideLoadingView();


				InvokeOnMainThread(() =>
				{
					if (performanceData == null)
					{
						lblTSB.Text = "-";
						lblCTL.Text = "-";
						lblATL.Text = "-";
						lblLoad.Text = "-";
					}
					else
					{
						lblTSB.Text = performanceData.TSB == "NaN" ? "0" : performanceData.TSB;
						lblCTL.Text = performanceData.CTL == "NaN" ? "0" : performanceData.CTL;
						lblATL.Text = performanceData.ATL == "NaN" ? "0" : performanceData.ATL;
						lblLoad.Text = performanceData.LOAD == "NaN" ? "0" : performanceData.LOAD;
					}
				});
			});
		}

		void SetPerformanceDataColor(bool isFuture)
		{
			UIColor color = isFuture ? COLOR_FUTURE : COLOR_PAST;
			lblTSB.TextColor = color;
			lblCTL.TextColor = color;
			lblATL.TextColor = color;
			lblLoad.TextColor = color;
		}

		#region GoHejaEventTableViewSource

		class GoHejaEventTableViewSource : UITableViewSource
		{
			List<GoHejaEvent> goHejaEvents;
			EventCalendarViewController eventCalendarVC;

			public GoHejaEventTableViewSource(List<GoHejaEvent> events, EventCalendarViewController eventCalendarVC)
			{
				this.goHejaEvents = new List<GoHejaEvent>();

				if (events == null) return;

				this.goHejaEvents = events;
				this.eventCalendarVC = eventCalendarVC;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return goHejaEvents.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 60;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				GoHejaEventCell cell = tableView.DequeueReusableCell("NitroEventCell") as GoHejaEventCell;
				cell.SetCell(goHejaEvents[indexPath.Row]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!eventCalendarVC.IsNetEnable()) return;

				var selectedEvent = goHejaEvents[indexPath.Row];
				UIStoryboard sb = UIStoryboard.FromName("Main", null);
				EventInstructionController eventInstructionVC = sb.InstantiateViewController("EventInstructionController") as EventInstructionController;
				eventInstructionVC.selectedEvent = selectedEvent;
				eventCalendarVC.NavigationController.PushViewController(eventInstructionVC, true);
			}
		}
		#endregion
    }
}
