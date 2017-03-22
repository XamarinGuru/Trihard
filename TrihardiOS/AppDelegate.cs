using Foundation;
using UIKit;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventKit;
using PortableLibrary;
using System.Collections.Generic;
using Google.Maps;

namespace location2
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public static LocationHelper MyLocationHelper = new LocationHelper();

		private nint bgThread = -1;

		public BaseViewController baseVC;

		EKCalendar goHejaCalendar = null;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			MapServices.ProvideAPIKey(PortableLibrary.Constants.GOOGLE_MAP_API_KEY);

			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			if (AppSettings.UserID == null || baseVC == null)
				return;

			App.Current.EventStore.RequestAccess(EKEntityType.Event,
				(bool granted, NSError e) =>
				{
					InvokeOnMainThread(() =>
					{
						if (granted)
							UpdateCalendarTimer();
					});
				});
		}

		private void UpdateCalendarTimer()
		{
			if (bgThread == -1)
			{
				bgThread = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
				new Task(() => { new Timer(UpdateCalendar, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60 * 30)); }).Start();
			}
		}
		private void UpdateCalendar(object state)
		{
			InvokeOnMainThread(() => { AddGoHejaCalendarToDevice(); });
		}

		private void AddGoHejaCalendarToDevice()
		{
			try
			{
				NSError error;

				////remove existing descending events from now in "goHeja Events" calendar of device.
				var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
				foreach (var calendar in calendars)
				{
					if (calendar.Title == PortableLibrary.Constants.DEVICE_CALENDAR_TITLE)
					{
						goHejaCalendar = calendar;

						EKCalendar[] calendarArray = new EKCalendar[1];
						calendarArray[0] = calendar;
						NSPredicate pEvents = App.Current.EventStore.PredicateForEvents(NSDate.Now.AddSeconds(-(3600 * 10000)), NSDate.Now.AddSeconds(3600 * 10000), calendarArray);
						EKEvent[] allEvents = App.Current.EventStore.EventsMatching(pEvents);
						if (allEvents == null)
							continue;
						foreach (var pEvent in allEvents)
						{
							NSError pE;
							DateTime now = DateTime.Now;
							DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
							var startString = baseVC.ConvertDateTimeToNSDate(startNow);
							if (pEvent.StartDate.Compare(startString) == NSComparisonResult.Descending)
								App.Current.EventStore.RemoveEvent(pEvent, EKSpan.ThisEvent, true, out pE);
						}
					}
				}

				if (goHejaCalendar == null)
				{
					goHejaCalendar = EKCalendar.Create(EKEntityType.Event, App.Current.EventStore);
					EKSource goHejaSource = null;

					foreach (EKSource source in App.Current.EventStore.Sources)
					{
						if (source.SourceType == EKSourceType.CalDav && source.Title == "iCloud")
						{
							goHejaSource = source;
							break;
						}
					}
					if (goHejaSource == null)
					{
						foreach (EKSource source in App.Current.EventStore.Sources)
						{
							if (source.SourceType == EKSourceType.Local)
							{
								goHejaSource = source;
								break;
							}
						}
					}
					if (goHejaSource == null)
					{
						return;
					}
					goHejaCalendar.Title = PortableLibrary.Constants.DEVICE_CALENDAR_TITLE;
					goHejaCalendar.Source = goHejaSource;
				}

				App.Current.EventStore.SaveCalendar(goHejaCalendar, true, out error);

				if (error == null)
					AddEvents();
			}
			catch (Exception e)
			{
				new UIAlertView("add events process", e.Message, null, "ok", null).Show();
			}
		}

		private void AddEvents()
		{
			var pastEvents = baseVC.GetPastEvents();
			var todayEvents = baseVC.GetTodayEvents();
			var futureEvents = baseVC.GetFutureEvents();

			AddEventsToGoHejaCalendar(pastEvents);
			AddEventsToGoHejaCalendar(todayEvents);
			AddEventsToGoHejaCalendar(futureEvents);
		}

		private void AddEventsToGoHejaCalendar(List<GoHejaEvent> eventsData)
		{
			if (goHejaCalendar == null || eventsData == null)
				return;

			foreach (var goHejaEvent in eventsData)
			{
				EKEvent newEvent = EKEvent.FromStore(App.Current.EventStore);

				var startDate = goHejaEvent.StartDateTime();
				var endDate = goHejaEvent.EndDateTime();

				DateTime now = DateTime.Now;
				DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
				DateTime startDay = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, startDate.Second);
				var deltaSec = (startDay - startNow).TotalSeconds;
				if (deltaSec < 0)
					continue;

				newEvent.StartDate = baseVC.ConvertDateTimeToNSDate(startDate);
				newEvent.EndDate = baseVC.ConvertDateTimeToNSDate(endDate);
				newEvent.Title = goHejaEvent.title;

				string eventDescription = baseVC.FormatEventDescription(goHejaEvent.eventData);

				string[] arryEventDes = eventDescription.Split(new char[] { '~' });

				for (var i = 0; i < arryEventDes.Length; i++)
				{
					newEvent.Notes += arryEventDes[i].ToString() + Environment.NewLine;
				}

				var strDistance = goHejaEvent.distance;
				var floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
				var b = Math.Truncate(floatDistance * 100);
				var c = b / 100;
				var formattedDistance = c.ToString("F2");

				var durMin = goHejaEvent.durMin == "" ? 0 : int.Parse(goHejaEvent.durMin);
				var durHrs = goHejaEvent.durHrs == "" ? 0 : int.Parse(goHejaEvent.durHrs);
				var pHrs = durMin / 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;

				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				newEvent.Notes += Environment.NewLine + "Planned HB : " + goHejaEvent.hb + Environment.NewLine +
								"Planned TSS : " + goHejaEvent.tss + Environment.NewLine +
								"Planned distance : " + formattedDistance + "KM" + Environment.NewLine +
								"Duration : " + strDuration + Environment.NewLine;

				//var encodedTitle = System.Web.HttpUtility.UrlEncode(goHejaEvent.title);

				//var urlDate = newEvent.StartDate;
				//var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				//var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				//var encodedEventURL = String.Format(PortableLibrary.Constants.URL_EVENT_MAP, encodedTitle, encodedDate, AppSettings.Username);

				//newEvent.Url = new NSUrl(System.Web.HttpUtility.UrlEncode(encodedEventURL)); ;

				//add alarm to event
				EKAlarm[] alarmsArray = new EKAlarm[2];
				alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 45)));
				alarmsArray[1] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 60 * 12)));
				newEvent.Alarms = alarmsArray;

				newEvent.Calendar = goHejaCalendar;

				NSError e;
				App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
			}
		}


		
		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			if (bgThread != -1)
			{
				UIApplication.SharedApplication.EndBackgroundTask(bgThread);
			}
		}
	}

}


