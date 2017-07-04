﻿using Foundation;
using UIKit;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventKit;
using PortableLibrary;
using System.Collections.Generic;
using Google.Maps;

using Firebase.InstanceID;
using Firebase.Analytics;
using Firebase.CloudMessaging;

using UserNotifications;

namespace location2
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
	{
        public NSDictionary _notiInfo;

        public static LocationHelper MyLocationHelper = new LocationHelper();

        public BaseViewController baseVC;
        public UINavigationController navVC;

		UIStoryboard _storyboard;
        nint bgThread = -1;
		EKCalendar goHejaCalendar = null;

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			MapServices.ProvideAPIKey(PortableLibrary.Constants.GOOGLE_MAP_API_KEY);

			_storyboard = UIStoryboard.FromName("Main", null);

			// Monitor token generation
			InstanceId.Notifications.ObserveTokenRefresh(TokenRefreshNotification);

            RegisterNotificationSettings();

            App.Configure();

            ConnectToFCM();

			return true;
		}

		public void RegisterNotificationSettings()
		{
			// Register your app for remote notifications.
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				// iOS 10 or later
				var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
				UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
				{
					Console.WriteLine(granted);
				});

				// For iOS 10 display notification (sent via APNS)
				UNUserNotificationCenter.Current.Delegate = this;

				// For iOS 10 data message (sent via FCM)
				Messaging.SharedInstance.RemoteMessageDelegate = this;
			}
			else
			{
				// iOS 9 or before
				var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
				var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}

			UIApplication.SharedApplication.RegisterForRemoteNotifications();
		}

		// To receive notifications in foregroung on iOS 9 and below.
		// To receive notifications in background in any iOS version
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary notiInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			Console.WriteLine("WillPresentNotification===" + notiInfo);
		}

        // Workaround for handling notifications in foreground for iOS 10
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
		public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
		{
			Console.WriteLine("WillPresentNotification===" + notification.Request.Content.UserInfo);
            var notiInfo = notification.Request.Content.UserInfo;
            var nTitle = ((notiInfo["aps"] as NSDictionary)["alert"] as NSDictionary)["title"].ToString();

            baseVC.ShowMessageBox(null, nTitle, "Cancel", new[] { "Go to detail" }, NotificationInfoProcess, notiInfo);
		}
		
        // Workaround for handling notifications in background for iOS 10
		[Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
		public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			Console.WriteLine("DidReceiveNotificationResponse===" + response.Notification.Request.Content.UserInfo);
			NotificationInfoProcess(response.Notification.Request.Content.UserInfo);
		}

		// Workaround for data message for iOS 10
		public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
		{
			Console.WriteLine("ApplicationReceivedRemoteMessage===" + remoteMessage.AppData);
		}

		async void TokenRefreshNotification(object sender, NSNotificationEventArgs e)
		{
		  var refreshedToken = InstanceId.SharedInstance.Token;
		  Console.WriteLine("Refreshed token: " + refreshedToken);

		  await SendRegistrationToServer(refreshedToken);
		}

		void ConnectToFCM()
		{
			Messaging.SharedInstance.Connect(error =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Token: {InstanceId.SharedInstance.Token}");
                }
                else
                {
                    Console.WriteLine($"Token: {InstanceId.SharedInstance.Token}");
                }
            });
		}

		async Task SendRegistrationToServer(string token)
		{
			if (AppSettings.CurrentUser == null) return;

			var currentUser = AppSettings.CurrentUser;
			currentUser.fcmToken = token;
			AppSettings.CurrentUser = currentUser;

			await FirebaseService.RegisterFCMUser(currentUser);
		}

		void NotificationInfoProcess(NSDictionary notiInfo)
		{
			var currentUser = AppSettings.CurrentUser;

			if (currentUser.userType == (int)PortableLibrary.Constants.USER_TYPE.COACH)
			{
                currentUser.athleteId = notiInfo["senderId"].ToString();
				AppSettings.isFakeUser = true;
                AppSettings.fakeUserName = notiInfo["senderName"].ToString();

				AppSettings.CurrentUser = currentUser;
			}

			_notiInfo = notiInfo;

            if (navVC != null)
            {
                GotoEventInstruction();
            }
		}

        public void GotoEventInstruction()
        {
			EventInstructionController eventInstructionVC = _storyboard.InstantiateViewController("EventInstructionController") as EventInstructionController;
			eventInstructionVC.eventID = _notiInfo["practiceId"].ToString();
			eventInstructionVC.isNotification = true;
			eventInstructionVC.commentID = _notiInfo["commentId"].ToString();
			navVC.PushViewController(eventInstructionVC, true);

            _notiInfo = null;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            if (AppSettings.CurrentUser == null || baseVC == null)
                return;

            DeviceCalendar.Current.EventStore.RequestAccess(EKEntityType.Event,
                (bool granted, NSError e) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (granted)
                            UpdateCalendarTimer();
                    });
                });
        }

		void UpdateCalendarTimer()
		{
			if (bgThread == -1)
			{
				bgThread = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
				new Task(() => { new Timer(UpdateCalendar, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60 * 30)); }).Start();
			}
		}
		void UpdateCalendar(object state)
		{
			InvokeOnMainThread(() => { AddGoHejaCalendarToDevice(); });
		}

		void AddGoHejaCalendarToDevice()
		{
			try
			{
				NSError error;

				////remove existing descending events from now in "goHeja Events" calendar of device.
				var calendars = DeviceCalendar.Current.EventStore.GetCalendars(EKEntityType.Event);
				foreach (var calendar in calendars)
				{
					if (calendar.Title == PortableLibrary.Constants.DEVICE_CALENDAR_TITLE)
					{
						goHejaCalendar = calendar;

						EKCalendar[] calendarArray = new EKCalendar[1];
						calendarArray[0] = calendar;
						NSPredicate pEvents = DeviceCalendar.Current.EventStore.PredicateForEvents(NSDate.Now.AddSeconds(-(3600 * 10000)), NSDate.Now.AddSeconds(3600 * 10000), calendarArray);
						EKEvent[] allEvents = DeviceCalendar.Current.EventStore.EventsMatching(pEvents);
						if (allEvents == null)
							continue;
						foreach (var pEvent in allEvents)
						{
							NSError pE;
							DateTime now = DateTime.Now;
							DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
							var startString = baseVC.ConvertDateTimeToNSDate(startNow);
							if (pEvent.StartDate.Compare(startString) == NSComparisonResult.Descending)
								DeviceCalendar.Current.EventStore.RemoveEvent(pEvent, EKSpan.ThisEvent, true, out pE);
						}
					}
				}

				if (goHejaCalendar == null)
				{
					goHejaCalendar = EKCalendar.Create(EKEntityType.Event, DeviceCalendar.Current.EventStore);
					EKSource goHejaSource = null;

					foreach (EKSource source in DeviceCalendar.Current.EventStore.Sources)
					{
						if (source.SourceType == EKSourceType.CalDav && source.Title == "iCloud")
						{
							goHejaSource = source;
							break;
						}
					}
					if (goHejaSource == null)
					{
						foreach (EKSource source in DeviceCalendar.Current.EventStore.Sources)
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

				DeviceCalendar.Current.EventStore.SaveCalendar(goHejaCalendar, true, out error);

				if (error == null)
					AddEvents();
			}
			catch (Exception e)
			{
				baseVC.ShowMessageBox("add events process", e.Message);
			}
		}

		void AddEvents()
		{
			var pastEvents = baseVC.GetPastEvents();
			var todayEvents = baseVC.GetTodayEvents();
			var futureEvents = baseVC.GetFutureEvents();

			AddEventsToGoHejaCalendar(pastEvents);
			AddEventsToGoHejaCalendar(todayEvents);
			AddEventsToGoHejaCalendar(futureEvents);
		}

		void AddEventsToGoHejaCalendar(List<GoHejaEvent> eventsData)
		{
			if (goHejaCalendar == null || eventsData == null)
				return;

			foreach (var goHejaEvent in eventsData)
			{
				EKEvent newEvent = EKEvent.FromStore(DeviceCalendar.Current.EventStore);

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
					newEvent.Notes += arryEventDes[i] + Environment.NewLine;
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
								"Planned Load : " + goHejaEvent.tss + Environment.NewLine +
								"Planned distance : " + formattedDistance + "KM" + Environment.NewLine +
								"Duration : " + strDuration + Environment.NewLine;

				//add alarm to event
				EKAlarm[] alarmsArray = new EKAlarm[2];
				alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 45)));
				alarmsArray[1] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 60 * 12)));
				newEvent.Alarms = alarmsArray;

				newEvent.Calendar = goHejaCalendar;

				NSError e;
				DeviceCalendar.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
			}
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


