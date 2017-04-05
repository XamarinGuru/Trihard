using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Widget;
using Java.Util;
using PortableLibrary;

namespace goheja
{
	[Service]
	public class BackgroundService : Service
	{
		System.Threading.Timer _timer;

		long calendarID = -1;

		public BaseActivity baseVC;

		[Android.Runtime.Register("onStart", "(Landroid/content/Intent;I)V", "GetOnStart_Landroid_content_Intent_IHandler")]
		[Obsolete("deprecated")]

		public override void OnStart(Intent intent, int startId)
		{
			base.OnStart(intent, startId);

			if (AppSettings.UserID == null || AppSettings.baseVC == null)
				return;

			baseVC = AppSettings.baseVC;

			StartUpdateTimer();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			_timer.Dispose();
		}

		public override Android.OS.IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException();
		}

		public void StartUpdateTimer()
		{
			_timer = new System.Threading.Timer((o) =>
			{
				AddGoHejaCalendarToDevice();
			} , null, 0, 1000 * 60 * 30);
		}

		private void AddGoHejaCalendarToDevice()
		{
			try
			{
				var calendarsUri = CalendarContract.Calendars.ContentUri;

				string[] calendarsProjection = {
				   CalendarContract.Calendars.InterfaceConsts.Id,
				   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				   CalendarContract.Calendars.InterfaceConsts.AccountName
				};

				var cursor = ApplicationContext.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);

				#region remove existing GoHeja calendar
				if (cursor.MoveToFirst())
				{
					do
					{
						long id = cursor.GetLong(0);
						String displayName = cursor.GetString(1);
						if (displayName == Constants.DEVICE_CALENDAR_TITLE)
							//RemoveCalendar(id);
							calendarID = id;
					} while (cursor.MoveToNext());
				}
				#endregion

				#region create GoHeja Calendar
				if (calendarID == -1)
				{
					var uri = CalendarContract.Calendars.ContentUri;
					ContentValues val = new ContentValues();
					val.Put(CalendarContract.Calendars.InterfaceConsts.AccountName, Constants.DEVICE_CALENDAR_TITLE);
					val.Put(CalendarContract.Calendars.InterfaceConsts.AccountType, CalendarContract.AccountTypeLocal);
					val.Put(CalendarContract.Calendars.Name, Constants.DEVICE_CALENDAR_TITLE);
					val.Put(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, Constants.DEVICE_CALENDAR_TITLE);
					val.Put(CalendarContract.Calendars.InterfaceConsts.CalendarColor, Android.Graphics.Color.Yellow);
					val.Put(CalendarContract.Calendars.InterfaceConsts.OwnerAccount, Constants.DEVICE_CALENDAR_TITLE);
					val.Put(CalendarContract.Calendars.InterfaceConsts.Visible, true);
					val.Put(CalendarContract.Calendars.InterfaceConsts.SyncEvents, true);
					uri = uri.BuildUpon()
						.AppendQueryParameter(CalendarContract.CallerIsSyncadapter, "true")
						.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.AccountName, Constants.DEVICE_CALENDAR_TITLE)
						.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.AccountType, CalendarContract.AccountTypeLocal)
						.Build();
					var calresult = ContentResolver.Insert(uri, val);
					calendarID = long.Parse(calresult.LastPathSegment);
				}

				#endregion
				RemoveGoHejaTodayAndFutureEvents();

				UpdateGoHejaEvents();
			}
			catch (Exception ex)
			{
				baseVC.ShowTrackMessageBox(ex.Message);
			}
		}

		public void UpdateGoHejaEvents()
		{
			var pastEvents = baseVC.GetPastEvents();
			var todayEvents = baseVC.GetTodayEvents();
			var futureEvents = baseVC.GetFutureEvents();

			AddEventsToGoHejaCalendar(pastEvents);
			AddEventsToGoHejaCalendar(todayEvents);
			AddEventsToGoHejaCalendar(futureEvents);
		}

		private void RemoveGoHejaTodayAndFutureEvents()
		{
			try
			{
				if (calendarID == -1) return;

				var eventURI = CalendarContract.Events.ContentUri;

				string[] eventsProjection = {
					"_id",
					CalendarContract.Events.InterfaceConsts.Title
				   , CalendarContract.Events.InterfaceConsts.Dtstart
				   , CalendarContract.Events.InterfaceConsts.Dtend
				};
				DateTime now = DateTime.Now;
				DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
				var startString = GetDateTimeMS(startNow).ToString();
				var endString = GetDateTimeMS(DateTime.Now.AddYears(5)).ToString();
				var selection = "((dtstart >= ?) AND (dtend <= ?) AND (calendar_id = ?))";
				var selectionArgs = new string[] { startString, endString, calendarID.ToString() };
				var calCursor = ApplicationContext.ContentResolver.Query(eventURI, eventsProjection, selection, selectionArgs, null);
				var cou = calCursor.Count;
				if (calCursor.MoveToFirst())
				{
					do
					{
						long id = calCursor.GetLong(0);
						String displayName = calCursor.GetString(1);
						long eventId = calCursor.GetLong(calCursor.GetColumnIndex("_id"));
						RemoveEvent(eventId);
					} while (calCursor.MoveToNext());
				}
			}
			catch (Exception ex)
			{
				baseVC.ShowTrackMessageBox(ex.Message);
			}
		}

		void AddEventsToGoHejaCalendar(List<GoHejaEvent> events)
		{
			if (calendarID == -1 || events == null || events.Count == 0) return;

			try
			{
				foreach (var goHejaEvent in events)
				{
					var startDate = goHejaEvent.StartDateTime();//Convert.ToDateTime(goHejaEvent.start);
					var endDate = goHejaEvent.EndDateTime();//Convert.ToDateTime(goHejaEvent.end);

					DateTime now = DateTime.Now;
					DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
					DateTime startDay = new DateTime(startDate.Year, startDate.Month, startDate.Day);
					var deltaSec = (startDay - startNow).TotalSeconds;
					if (deltaSec < 0)
						continue;

					var title = goHejaEvent.title;

					string eventDescription = goHejaEvent.eventData;

					var filteredDescription = baseVC.FormatEventDescription(eventDescription);

					string[] arryEventDes = filteredDescription.Split(new char[] { '~' });

					string note = "";
					for (var i = 0; i < arryEventDes.Length; i++)
					{
						note += arryEventDes[i] + Environment.NewLine;
					}


					var strDistance = goHejaEvent.distance;
					float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
					var b = Math.Truncate(floatDistance * 100);
					var c = b / 100;
					var formattedDistance = c.ToString("F2");

					var durMin = goHejaEvent.durMin == "" ? 0 : int.Parse(goHejaEvent.durMin);
					var durHrs = goHejaEvent.durHrs == "" ? 0 : int.Parse(goHejaEvent.durHrs);
					var pHrs = durMin / 60;
					durHrs = durHrs + pHrs;
					durMin = durMin % 60;

					var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

					note += System.Environment.NewLine + "Planned HB : " + goHejaEvent.hb + Environment.NewLine +
								  "Planned TSS : " + goHejaEvent.tss + Environment.NewLine +
									"Planned distance : " + formattedDistance + "KM" + Environment.NewLine +
									"Duration : " + strDuration + Environment.NewLine;

					#region create event
					ContentValues eventValues = new ContentValues();
					eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calendarID);
					eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, title);
					eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, note);
					eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(startDate));
					eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(endDate));
					eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
					eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
					eventValues.Put(CalendarContract.Events.InterfaceConsts.HasAlarm, 1);

					var eventURI = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
					var eventID = long.Parse(eventURI.LastPathSegment);


					var deltaMin = (startDate - DateTime.Now).TotalMinutes;
					if (deltaMin > 45)
					{
						ContentValues reminderValues1 = new ContentValues();
						reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
						reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 45.0f);
						reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.Method, 1);
						ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues1);

						if (deltaMin > (10 * 60))
						{
							ContentValues reminderValues2 = new ContentValues();
							reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
							reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 60 * 10);
							reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.Method, 1);
							ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues2);
						}
					}

					#endregion
				}
			}
			catch (Exception ex)
			{
				baseVC.ShowTrackMessageBox(ex.Message);
			}
		}

		private void RemoveCalendar(long calID)
		{
			Android.Net.Uri.Builder builder1 = CalendarContract.Calendars.ContentUri.BuildUpon();
			builder1.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, Constants.DEVICE_CALENDAR_TITLE);

			String[] selArgs = new String[] { Constants.DEVICE_CALENDAR_TITLE };
			int deleted = ContentResolver.Delete(CalendarContract.Calendars.ContentUri, CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName + " =? ", selArgs);
		}
		private void RemoveEvent(long eventID)
		{
			String[] selArgs = new String[] { eventID.ToString() };
			int deleted = ContentResolver.Delete(CalendarContract.Events.ContentUri, "_id =? ", selArgs);
		}
		long GetDateTimeMS(DateTime date)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);
			c.Set(CalendarField.DayOfMonth, date.Day);
			c.Set(CalendarField.HourOfDay, date.Hour);
			c.Set(CalendarField.Minute, date.Minute);
			c.Set(CalendarField.Month, (date.Month - 1));
			c.Set(CalendarField.Year, date.Year);

			return c.TimeInMillis;
		}


	}
}