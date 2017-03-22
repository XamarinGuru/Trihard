using CoreGraphics;
using Foundation;
using Softweb.Xamarin.Controls.iOS;
using System;
using UIKit;

namespace XCalendarSample
{
	public class XCalendarViewController : UIViewController
	{
		private readonly Calendar _calendar;

		public XCalendarViewController()
		{
			_calendar = new Calendar ();
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = UIColor.FromRGBA(66, 86, 110, 255);
			NavigationController.NavigationBarHidden = true;

			//Create required objects
			var menuView = new CalendarMenuView { Frame = new CGRect (0f, 20f, 320f, 60f) };
			var contentView = new CalendarContentView { Frame = new CGRect (0f, 85f, 320f, 270f) };
			var eventView = new CalendarEventView { Frame = new CGRect (0f, 400f, 320f, 170f) };
			
			//Remember to specify the DateTimeKind for event date
			var fSharpMeetDate =
				new DateTime (2015, 1, 1, 11, 30, 00, DateTimeKind.Local);
			var weddingDate = fSharpMeetDate.AddDays(5);

			//Add events
			_calendar.EventSchedule = new EventDetails[] {
				new EventDetails (
					startDate: (NSDate) DateTime.Now,
					endDate: NSDate.Now.AddSeconds(3000),
					title: "Writing documentation"),
				new EventDetails (
					(NSDate) fSharpMeetDate,
					(NSDate) fSharpMeetDate.AddHours(1.5),
					"F# Group Meeting"),
				new EventDetails (
					(NSDate) weddingDate,
					(NSDate) weddingDate.AddHours(2),
					"Peter and Denise's Wedding"),
			};
			
			//Customize calendar appearance
			var appearance = _calendar.CalendarAppearance;
			appearance.GetNSCalendar().FirstWeekDay = (nuint) 2;
			appearance.DayCircleColorSelected = UIColor.FromRGB(154, 188, 227);
			appearance.DayTextColorOtherMonth = appearance.DayDotColorOtherMonth = UIColor.FromRGB(157, 177, 199);
			appearance.DayDotColor = appearance.DayTextColor = appearance.MenuMonthTextColor = UIColor.White;
			appearance.DayCircleColorToday = UIColor.LightTextColor;
			appearance.DayCircleRatio = (9f / 10f);
			appearance.WeekDayFormat = CalendarWeekDayFormat.Short;
			
			// Pass a function that returns text to display in the month label. E.g. "JAN 2014" OR “01/2014"
			appearance.SetMonthLabelTextCallback(
				//Returns full month-name and year. E.g. DECEMBER 2014
				(NSDate date, Calendar cal) => new NSString (((DateTime) date).ToString("MMMM\nyyyy")));
			
			//Link the views to the calendar
			_calendar.MenuMonthsView = menuView;
			_calendar.ContentView = contentView;
			_calendar.EventView = eventView;

			_calendar.DateSelected += DateSelected;
			_calendar.NextPageLoaded += DidLoadNextPage;
			_calendar.PreviousPageLoaded += DidLoadPreviousPage;
			
			//Add calendar views to the main view
			View.Add(menuView);
			View.Add(contentView);
			View.Add(eventView);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Reload calendar
			_calendar.ReloadData();
		}


		public void DateSelected(object sender, DateSelectedEventArgs args)
		{
			Console.WriteLine(String.Format("Selected date is {0}", ((DateTime) args.Date).ToLocalTime().ToString("dd-MMM-yyyy")));
		}

		public void DidLoadPreviousPage(object sender, EventArgs args)
		{
			Console.WriteLine("loaded previous page");
		}

		public void DidLoadNextPage(object sender, EventArgs args)
		{
			Console.WriteLine("loaded next page");
		}
	}
}
