
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.GrapeCity.Xuni.Calendar;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventCalendarActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EventCalendarActivity : BaseActivity
	{
		Android.Graphics.Color COLOR_PAST = Android.Graphics.Color.Rgb(229, 161, 9);
		Android.Graphics.Color COLOR_FUTURE = Android.Graphics.Color.Rgb(63, 187, 190);

		XuniCalendar calendar;

		ListView eventsList;
		List<GoHejaEvent> _events = new List<GoHejaEvent>();
		LinearLayout noEventsContent;

		TextView lblTSB, lblCTL, lblATL, lblLoad;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventCalendarActivity);

			ReloadEvents();

			InitUISettings();
		}

		void InitUISettings()
		{
			#region xuni calendar
			calendar = FindViewById<XuniCalendar>(Resource.Id.calendar);
			calendar.Orientation = CalendarOrientation.Vertical;
			calendar.MaxSelectionCount = 1;

			// change appearance
			calendar.DayOfWeekBackgroundColor = System.Drawing.Color.Transparent.ToArgb();
			calendar.DayOfWeekTextColor = System.Drawing.Color.LightGray.ToArgb();
			calendar.TodayTextColor = System.Drawing.Color.Red.ToArgb();
			calendar.SelectionBackgroundColor = GROUP_COLOR;

			calendar.DaySlotLoading += CalendarDaySlotLoading;
			calendar.SelectionChanged += CalendarSelectionChanged;
			#endregion

			lblTSB = FindViewById<TextView>(Resource.Id.lblTSB);
			lblCTL = FindViewById<TextView>(Resource.Id.lblCTL);
			lblATL = FindViewById<TextView>(Resource.Id.lblATL);
			lblLoad = FindViewById<TextView>(Resource.Id.lblLoad);

			FindViewById<TextView>(Resource.Id.lblNoEvent).SetTextColor(GROUP_COLOR);

			lblTSB.SetTextColor(GROUP_COLOR);
			lblCTL.SetTextColor(GROUP_COLOR);
			lblATL.SetTextColor(GROUP_COLOR);
			lblLoad.SetTextColor(GROUP_COLOR);

			noEventsContent = FindViewById<LinearLayout>(Resource.Id.noEventsContent);
			noEventsContent.Visibility = ViewStates.Gone;

			FindViewById(Resource.Id.ActionReload).Click += (sender, e) => ReloadEvents();
			FindViewById(Resource.Id.ActionToday).Click += (sender, e) => GotoToday();
		}

		void CalendarSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
		{
			var filterDate = (sender as XuniCalendar).SelectedDate;
			FilterEventsByDate(filterDate);
		}

		void FilterEventsByDate(DateTime filterDate)
		{
			try
			{
				List<GoHejaEvent> filteredEvents = new List<GoHejaEvent>();

				if (_events != null && _events.Count != 0)
				{
					for (int i = 0; i < _events.Count; i++)
					{
						var startDate = _events[i].StartDateTime();
						if (startDate.DayOfYear == filterDate.DayOfYear)
						{
							filteredEvents.Add(_events[i]);
						}
					}
				}
				if (DateTime.Compare(filterDate, DateTime.Now) > 0)
					SetPerformanceDataColor(true);
				else
					SetPerformanceDataColor(false);

				if (filteredEvents.Count == 0)
					noEventsContent.Visibility = ViewStates.Visible;
				else
					noEventsContent.Visibility = ViewStates.Gone;

				eventsList = FindViewById(Resource.Id.eventsList) as ListView;
				var adapter = new GoHejaEventAdapter(filteredEvents, this);
				eventsList.Adapter = adapter;
				adapter.NotifyDataSetChanged();
				SetListViewHeightBasedOnChildren(eventsList);

				InitPerformanceData(filterDate);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		private void CalendarDaySlotLoading(object sender, CalendarDaySlotLoadingEventArgs e)
		{
			try
			{
				var currentDateTime = FromUnixTime(e.Date.Time).ToLocalTime();

				Java.Util.Date date = e.Date;
				Java.Util.Calendar cal = Java.Util.Calendar.GetInstance(Java.Util.Locale.English);
				cal.Time = date;
				int day = cal.Get(Java.Util.CalendarField.DayOfMonth);

				CalendarDaySlotBase layout = new CalendarDaySlotBase(ApplicationContext);
				layout.SetGravity(GravityFlags.Center);
				layout.SetVerticalGravity(GravityFlags.Center);
				layout.Orientation = Orientation.Vertical;
				layout.SetPadding(5, 5, 5, 5);
				LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
				layout.LayoutParameters = linearLayoutParams;

				TextView tv = new TextView(ApplicationContext);
				tv.Gravity = GravityFlags.Center;
				tv.Text = day.ToString();

				if (currentDateTime.Date == DateTime.Now.Date)
					tv.SetTextColor(Android.Graphics.Color.Red);

				if (e.AdjacentDay)
					tv.SetTextColor(Android.Graphics.Color.DarkGray);

				layout.AddView(tv);

				if (_events != null && _events.Count != 0)
				{
					for (int i = 0; i < _events.Count; i++)
					{
						var startDate = _events[i].StartDateTime();
						if (startDate.Date == currentDateTime.Date)
						{
							tv.SetBackgroundColor(Android.Graphics.Color.Orange);
						}
					}
				}

				e.DaySlot = layout;
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
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

				AppSettings.currentEventsList = _events;

				RunOnUiThread(() =>
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
			if (!IsNetEnable()) return;

			calendar.ChangeViewMode(CalendarViewMode.Month, new Java.Util.Date(DateTime.Now.ToString()));
		}

		void InitPerformanceData(DateTime date)
		{
			try
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_LOADING_EVENTS);

					var performanceData = GetPerformanceForDate(date);

					HideLoadingView();

					RunOnUiThread(() =>
					{
						if (performanceData == null)
						{
							FindViewById<TextView>(Resource.Id.lblTSB).Text = "-";
							FindViewById<TextView>(Resource.Id.lblCTL).Text = "-";
							FindViewById<TextView>(Resource.Id.lblATL).Text = "-";
							FindViewById<TextView>(Resource.Id.lblLoad).Text = "-";
						}
						else
						{
							FindViewById<TextView>(Resource.Id.lblTSB).Text = performanceData.TSB == "NaN" ? "0" : performanceData.TSB;
							FindViewById<TextView>(Resource.Id.lblCTL).Text = performanceData.CTL == "NaN" ? "0" : performanceData.CTL;
							FindViewById<TextView>(Resource.Id.lblATL).Text = performanceData.ATL == "NaN" ? "0" : performanceData.ATL;
							FindViewById<TextView>(Resource.Id.lblLoad).Text = performanceData.LOAD == "NaN" ? "0" : performanceData.LOAD;
						}
					});
				});
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		void SetPerformanceDataColor(bool isFuture)
		{
			Android.Graphics.Color color = isFuture ? COLOR_FUTURE : COLOR_PAST;

			FindViewById<TextView>(Resource.Id.lblTSB).SetTextColor(color);
			FindViewById<TextView>(Resource.Id.lblCTL).SetTextColor(color);
			FindViewById<TextView>(Resource.Id.lblATL).SetTextColor(color);
			FindViewById<TextView>(Resource.Id.lblLoad).SetTextColor(color);
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				ActionBackCancel();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
