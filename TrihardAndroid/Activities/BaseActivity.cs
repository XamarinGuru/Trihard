using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidHUD;
using Firebase.Iid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableLibrary;
using PortableLibrary.Model;

namespace goheja
{
	[Activity(Label = "BaseActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class BaseActivity : FragmentActivity
	{
		public Color GROUP_COLOR;

		Color[] PATH_COLORS = { Color.Red, Color.Rgb(38, 127, 0), Color.Blue };
		Color COLOR_ORANGE = Color.Rgb(229, 161, 9);
		Color COLOR_RED = Color.Rgb(179, 66, 17);
		Color COLOR_BLUE = Color.Rgb(21, 181, 98);

		AlertDialog.Builder alert;

		trackSvc.Service1 mTrackSvc = new trackSvc.Service1();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);

			SetGroupColor();
		}

		void SetGroupColor()
		{
			GROUP_COLOR = Color.ParseColor("#" + Constants.GROUP_COLOR);
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected void OnBack()
		{
			base.OnBackPressed();
			//OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
		}

		protected void ActionBackCancel()
		{
			var activity = new Intent();
			SetResult(Result.Canceled, activity);
			Finish();
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (this.CurrentFocus == null) return true;

			InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
			return base.OnTouchEvent(e);
		}

		public void ShowLoadingView(string title)
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Show(this, title, -1, MaskType.Black);
			});
		}

		public void HideLoadingView()
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Dismiss(this);
			});
		}

		public void ShowMessageBox(string title, string message, string titleOKBtn, string titleCancelBtn, Action successHandler)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetPositiveButton(titleCancelBtn, (senderAlert, args) =>
			{
			});
			alert.SetNegativeButton(titleOKBtn, (senderAlert, args) =>
			{
				successHandler();
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		public void ShowTrackMessageBox(string trackMsg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle("");
			alert.SetMessage(Constants.MSG_COMMON);
			alert.SetNegativeButton("OK", (senderAlert, args) =>
			{
				TrackErrorIntoServer(trackMsg, filePath, lineNumber, caller);
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		public void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetCancelable(false);
			alert.SetPositiveButton("OK", delegate {
				if (isFinish)
					CloseApplication();
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		#region error handling
		public bool IsNetEnable()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

			if (!isOnline)
			{
				ShowMessageBox(null, Constants.MSG_NO_INTERNET, true);
				return false;
			}
			return true;
		}

		void TrackErrorIntoServer(string msgError, string filePath, int lineNumber, string caller)
		{
			string userID = GetUserID();

			var dateTime = DateTime.Now.ToString();
			var deviceModel = GetDeviceModel();
			var errorDetail = string.Format(Constants.MSG_TRACK_ERROR_DETAIL, msgError, filePath, lineNumber, caller);
			var msg = string.Format(Constants.MSG_TRACK_ERROR, dateTime, userID, deviceModel, Constants.SPEC_GROUP_TYPE, errorDetail);

			ShowLoadingView(Constants.MSG_TRAKING_ERROR);

			#region error track log
			mTrackSvc.specLog(msg);
			#endregion

			HideLoadingView();

			CloseApplication();
		}

		public string GetDeviceModel()
		{
			string manufacturer = Build.Manufacturer;
			string model = Build.Model;
			return string.Format("{0} {1}", Build.Manufacturer, Android.OS.Build.Model);
		}

		void CloseApplication()
		{
			FinishAffinity();
			Process.KillProcess(Process.MyPid());
		}
		#endregion

		#region integrate with web reference

		#region USER_MANAGEMENT
		public string RegisterUser(string fName, string lName, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			string result = "";

			try
			{
				string deviceUDID = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
				result = mTrackSvc.insertNewDevice(fName, lName, deviceUDID, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified, Constants.SPEC_GROUP_TYPE);
			}
			catch(Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
			return result;
		}

		public LoginUser LoginUser(string email, string password)
		{
			var loginUser = new LoginUser();

			try
			{
				var objUser = mTrackSvc.mobLogin(email, password, Constants.SPEC_GROUP_TYPE);
				var jsonUser = FormatJsonType(objUser.ToString());
				loginUser = JsonConvert.DeserializeObject<LoginUser>(jsonUser);

                if (loginUser.userId != null)
                {
                    AppSettings.CurrentUser = loginUser;

                    RegisterFCMUser(loginUser);

                    return loginUser;
                }
			}
			catch(Exception ex)
			{
                ShowTrackMessageBox(ex.Message);
			}

            return null;
		}

		void RegisterFCMUser(LoginUser user)
		{
            ThreadPool.QueueUserWorkItem(async delegate
            {
				user.fcmToken = FirebaseInstanceId.Instance.Token;
				user.osType = Constants.OS_TYPE.Android;

                var isFcmOn = await FirebaseService.RegisterFCMUser(user);
                user.isFcmOn = isFcmOn;
                AppSettings.CurrentUser = user;
            });
		}

        public async Task SignOutUser()
		{
            await FirebaseService.RemoveFCMUser(AppSettings.CurrentUser);
			AppSettings.CurrentUser = null;
		}

		public List<Athlete> GetAllUsers()
		{
			var result = new List<Athlete>();

			try
			{
				var objAthletes = mTrackSvc.athGeneralListMobWithTypeAndId(string.Empty, Constants.SPEC_GROUP_TYPE);
				var athletes = JsonConvert.DeserializeObject<Athletes>(objAthletes.ToString());
				result = athletes.athlete;
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return SortUsers(result);
		}

        public List<string> GetCoachIDs()
        {
            var result = new List<string>();

            try
            {
                var jsonCoachIDs = mTrackSvc.getCoachesMob(Constants.SPEC_GROUP_TYPE);
                var arrCoachIDs = jsonCoachIDs.Split(new char[] { ',' });

                result = new List<string>(arrCoachIDs);
            }
            catch (Exception ex)
            {
                ShowTrackMessageBox(ex.Message);
            }

            return result;
        }

		public SubGroups GetSubGroups(string groupId)
		{
			var result = new SubGroups();

			try
			{
				var objAthletes = mTrackSvc.fieldAthletsAndEvenetsMobWithIdAndType(string.Empty, groupId, Constants.SPEC_GROUP_TYPE);
				result = JsonConvert.DeserializeObject<SubGroups>(objAthletes.ToString());
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		List<Athlete> SortUsers(List<Athlete> users)
		{
			try
			{
				var index = users.FindIndex(x => x._id == AppSettings.CurrentUser.userId);
				var item = users[index];
				users[index] = users[0];
				item.name = "me";
				users[0] = item;
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return users;
		}
		public List<AthleteInSubGroup> SortSubUsers(List<AthleteInSubGroup> users)
		{
			try
			{
				var index = users.FindIndex(x => x.athleteId == AppSettings.CurrentUser.userId);

				if (index != -1)
				{
					var item = users[index];
					users[index] = users[0];
					item.athleteName = "me";
					users[0] = item;
				}
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return users;
		}

		public string GetCode(string email)
		{
			string result = "0";

			try
			{
				result = mTrackSvc.getCode(email, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public int ResetPassword(string email, string password)
		{
			int result = 0;

			try
			{
				bool resultSpecified = false;
				mTrackSvc.restPasword(email, password, Constants.SPEC_GROUP_TYPE, out result, out resultSpecified);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public string GetUserID()
		{
			var currentUser = AppSettings.CurrentUser;
			if (currentUser != null)
			{
				if (!string.IsNullOrEmpty(currentUser.athleteId))
					return currentUser.athleteId;
				else if (!string.IsNullOrEmpty(currentUser.userId))
					return currentUser.userId;
			}

			return null;
		}

        public RootMember GetUserObject(string userId = null)
		{
			RootMember result = new RootMember();

            userId = userId == null ? GetUserID() : userId;

			try
			{
				var objUser = mTrackSvc.getUsrObject(userId, Constants.SPEC_GROUP_TYPE);
				var jsonUser = FormatJsonType(objUser.ToString());
				result = JsonConvert.DeserializeObject<RootMember>(jsonUser);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public string UpdateUserDataJson(RootMember updatedUserObject)
		{
			string result = "";

			try
			{
				string userID = GetUserID();

				var jsonUser = JsonConvert.SerializeObject(updatedUserObject);
				Console.WriteLine(jsonUser);
				result = mTrackSvc.updateUserDataJson(userID, jsonUser, userID, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}
		#endregion

		#region USER_GAUGE & PERFORMANCE DATA
		public Gauge GetGauge()
		{
			Gauge result = new Gauge();

			string userID = GetUserID();

			try
			{
				var strGauge = mTrackSvc.getGaugeMob(DateTime.Now, true, userID, null, Constants.SPEC_GROUP_TYPE, null, "5");
				result = JsonConvert.DeserializeObject<Gauge>(strGauge);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public ReportGraphData GetPerformance()
		{
			ReportGraphData result = new ReportGraphData();

			string userID = GetUserID();

			try
			{
				var strPerformance = mTrackSvc.getUserPmc(userID, Constants.SPEC_GROUP_TYPE);
				result = JsonConvert.DeserializeObject<ReportGraphData>(strPerformance.ToString());
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public PerformanceDataForDate GetPerformanceForDate(DateTime date)
		{
			PerformanceDataForDate result = new PerformanceDataForDate();

			string userID = GetUserID();

			try
			{
				var strPerformance = mTrackSvc.getPerformanceFordate(userID, date, true, Constants.SPEC_GROUP_TYPE);
				result = JsonConvert.DeserializeObject<PerformanceDataForDate>(strPerformance.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}
		#endregion

		#region EVENT_MANAGEMENT
		public List<GoHejaEvent> GetPastEvents(bool isForDeviceCalendar = false)
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

            var userId = isForDeviceCalendar ? AppSettings.CurrentUser.userId : GetUserID();

			try
			{
				var strPastEvents = mTrackSvc.getUserCalendarPast(userId, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strPastEvents));
				result = CastGoHejaEvents(eventsData);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public List<GoHejaEvent> GetTodayEvents(bool isForDeviceCalendar = false)
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

			var userId = isForDeviceCalendar ? AppSettings.CurrentUser.userId : GetUserID();

			try
			{
				var strTodayEvents = mTrackSvc.getUserCalendarToday(userId, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strTodayEvents));
				result = CastGoHejaEvents(eventsData);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public List<GoHejaEvent> GetFutureEvents(bool isForDeviceCalendar = false)
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

			var userId = isForDeviceCalendar ? AppSettings.CurrentUser.userId : GetUserID();

			try
			{
				var strFutureEvents = mTrackSvc.getUserCalendarFuture(userId, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strFutureEvents));
				result = CastGoHejaEvents(eventsData);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public GoHejaEvent GetEventDetail(string eventID)
		{
			GoHejaEvent result = new GoHejaEvent();

			try
			{
				var strEventDetail = mTrackSvc.getEventMob(eventID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strEventDetail.ToString()));
				result = CastGoHejaEvents(eventsData)[0];
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
				//return null;
			}

			return result;
		}

		public List<GoHejaEvent> CastGoHejaEvents(JArray events)
		{
			var returnEvents = new List<GoHejaEvent>();

			if (events == null) return returnEvents;

			try
			{
				foreach (var eventJson in events)
				{
					GoHejaEvent goHejaEvent = JsonConvert.DeserializeObject<GoHejaEvent>(eventJson.ToString());
					returnEvents.Add(goHejaEvent);
				}
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return returnEvents;
		}

		public EventTotal GetEventTotals(string eventID)
		{
			var eventTotal = new EventTotal();
			try
			{
				var totalObject = mTrackSvc.getEventTotalsMob(eventID, Constants.SPEC_GROUP_TYPE);
				eventTotal = JsonConvert.DeserializeObject<EventTotal>(totalObject.ToString());
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
				//return null;
			}
			return eventTotal;
		}

        public ReportData GetEventReport(string eventID)
		{
            var result = new ReportData();

            try
            {
                var reportObject = mTrackSvc.getEventReport(eventID, Constants.SPEC_GROUP_TYPE);
                result = JsonConvert.DeserializeObject<ReportData>(reportObject.ToString());
            }
            catch (Exception ex)
            {
                //ShowTrackMessageBox(ex.Message);
                return null;
            }
			return result;
		}

		public EventPoints GetAllMarkers(string eventID)
		{
			var eventMarkers = new EventPoints();
			try
			{
				var markerObject = mTrackSvc.getAllMarkersByPractice(eventID, Constants.SPEC_GROUP_TYPE);
				eventMarkers = JsonConvert.DeserializeObject<EventPoints>(markerObject.ToString());
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
				//return null;
			}
			return eventMarkers;
		}

		public EventPoints GetNearestEventMarkers(string userID)
		{
			var eventMarkers = new EventPoints();
			try
			{
				var markerObject = mTrackSvc.getNearestEventMakers(userID, Constants.SPEC_GROUP_TYPE);
				eventMarkers = JsonConvert.DeserializeObject<EventPoints>(markerObject.ToString());
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
				//return null;
			}
			return eventMarkers;
		}

		public List<List<TPoint>> GetTrackPoints(string eventID)
		{
			List<List<TPoint>> returnTPoints = new List<List<TPoint>>();
			try
			{
				var pointsObject = mTrackSvc.getTrackPoints(eventID, Constants.SPEC_GROUP_TYPE);
				var jsonPoints = FormatJsonType(pointsObject.ToString());
				var trackPoints = JsonConvert.DeserializeObject<EventTracks>(jsonPoints);

				if (trackPoints != null && trackPoints.TrackPoints.Count > 0)
				{
					List<TPoint> points = new List<TPoint>();
					List<string> lapNOs = new List<string>();
					foreach (var tPoint in trackPoints.TrackPoints)
					{
						tPoint.index = DateTime.Parse(tPoint.LocalTime);
						points.Add(tPoint);

						bool isExist = false;
						foreach (var lapNo in lapNOs)
						{
							if (lapNo == tPoint.lapNo)
								isExist = true;
						}

						if (!isExist)
							lapNOs.Add(tPoint.lapNo);
					}

					points.Sort((x, y) => DateTime.Compare(x.index, y.index));

					foreach (var lapNo in lapNOs)
					{
						List<TPoint> tPoints = new List<TPoint>();
						foreach (var tPoint in points)
						{
							if (lapNo == tPoint.lapNo && !Equals(tPoint.Latitude, 0d) && !Equals(tPoint.Longitude, 0d))
								tPoints.Add(tPoint);
						}
						if (tPoints.Count != 0)
							returnTPoints.Add(tPoints);
					}
				}

			}
			catch (Exception ex)
			{
				//ShowTrackMessageBox(ex.Message);
				return null;
			}
			return returnTPoints;
		}

		public Color GetRandomColor(int index)
		{
			return PATH_COLORS[index % 3];
		}

		public Comments GetComments(string eventID, string type = "1")
		{
			var comments = new Comments();
			try
			{
				var commentObject = mTrackSvc.getComments(eventID, "1", Constants.SPEC_GROUP_TYPE);
				comments = JsonConvert.DeserializeObject<Comments>(commentObject.ToString());
                comments.comments.Reverse();
			}
			catch (Exception ex)
			{
				//ShowTrackMessageBox(ex.Message);
				return null;
			}
			return comments;
		}

		public Comment AddComment(string commentText)
		{
			Comment result = new Comment();

			try
			{
				var author = string.Empty;
				var authorId = AppSettings.CurrentUser.userId;

                var commentResponseObject = mTrackSvc.setCommentsMob(author, authorId, commentText, AppSettings.selectedEvent._id, Constants.SPEC_GROUP_TYPE);
                result = JsonConvert.DeserializeObject<Comment>(commentResponseObject.ToString());

                if (result != null)
					SendNotification(result);

			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

        async void SendNotification(Comment comment)
        {
			var notificationContent = new FCMDataNotification();
            notificationContent.senderId = comment.authorId;
            notificationContent.senderName = comment.author;
            notificationContent.practiceId = comment.eventId;
            notificationContent.commentId = comment.commentId;
			notificationContent.description = comment.commentText;
            notificationContent.practiceType = GetTypeStrFromID(AppSettings.selectedEvent.type);
            notificationContent.practiceName = AppSettings.selectedEvent.title;
            notificationContent.practiceDate = String.Format("{0:f}", AppSettings.selectedEvent.StartDateTime());
			notificationContent.osType = Constants.OS_TYPE.Android;

            var recipientIDs = new List<string>();
			if (AppSettings.isFakeUser)
			{
                recipientIDs.Add(AppSettings.CurrentUser.athleteId);
			}
            else
            {
                recipientIDs = GetCoachIDs();
            }

			recipientIDs.RemoveAll(pID => pID == AppSettings.CurrentUser.userId);

			await FirebaseService.SendNotification(notificationContent, recipientIDs);
        }

        public void UpdateMemberNotes(string notes, string userID, string eventId, string username, string attended, string duration, string distance, string trainScore, string type)
        {
            try
            {
                var response = mTrackSvc.updateMeberNotes(notes, userID, eventId, username, attended, duration, distance, trainScore, type, Constants.SPEC_GROUP_TYPE);
            }
            catch (Exception ex)
            {
                ShowTrackMessageBox(ex.Message);
            }
        }

        List<TRecord> _offlineRecords = new List<TRecord>();

        public void RecordPracticeTrack(TRecord record)
        {
            _offlineRecords.Add(record);

            if (IsNetEnable())
            {
                foreach (TRecord r in _offlineRecords)
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            mTrackSvc.updateMomgoData(r.fullName, r.loc, r.date, true, r.deviceId, r.speed, true, r.athid, r.country, r.distance, true, r.gainedAlt, true, r.bearinng, true, (int)r.recordType, true, ((int)r.sportType).ToString(), Constants.SPEC_GROUP_TYPE);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
                _offlineRecords.Clear();
            }
        }

		#endregion

		public string GetTypeStrFromID(string typeID)
		{
			string result = "";

			try
			{
				result = Constants.PRACTICE_TYPES[int.Parse(typeID) - 1];
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}
		public string GetTypeIDFromStr(string typeStr)
		{
			string result = "";

			try
			{
				result = (Array.IndexOf(Constants.PRACTICE_TYPES, typeStr) + 1).ToString();
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public double DistanceAtoB(TPoint pA, TPoint pB)
		{
			double result = 0;

			try
			{
				Location pointA = new Location("");
				pointA.Latitude = pA.Latitude;
				pointA.Longitude = pA.Longitude;

				Location pointB = new Location("");
				pointB.Latitude = pB.Latitude;
				pointB.Longitude = pB.Longitude;

				result = pointA.DistanceTo(pointB);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public void CompareEventResult(float planned, float performed, TextView lblPlanned, TextView lblPerformed)
		{
			try
			{
				if (planned == performed || planned == 0 || performed == 0)
				{
					lblPlanned.SetTextColor(COLOR_ORANGE);
					lblPerformed.SetTextColor(COLOR_ORANGE);
					return;
				}

				if (planned > performed)
				{
					var delta = (planned - performed) / performed;
					if (delta < 0.15)
					{
						lblPlanned.SetTextColor(COLOR_ORANGE);
						lblPerformed.SetTextColor(COLOR_ORANGE);
					}
					else
					{
						lblPlanned.SetTextColor(COLOR_BLUE);
						lblPerformed.SetTextColor(COLOR_BLUE);
					}
				}
				else if (planned < performed)
				{
					var delta = (performed - planned) / planned;
					if (delta < 0.15)
					{
						lblPlanned.SetTextColor(COLOR_ORANGE);
						lblPerformed.SetTextColor(COLOR_ORANGE);
					}
					else
					{
						lblPlanned.SetTextColor(COLOR_RED);
						lblPerformed.SetTextColor(COLOR_RED);
					}
				}
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		public bool ValidateUserNickName(string nickName)
		{
			string result = "0";

			try
			{
				result = mTrackSvc.validateNickName(nickName, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result == "1" ? false : true;
		}

		private string FormatJsonType(string jsonData)
		{
			string result = "";
			try
			{
				var returnString = jsonData.Replace(Constants.INVALID_JSONS1[0], "\"");
				returnString = returnString.Replace(Constants.INVALID_JSONS1[1], "\"");
				result = returnString.Replace(Constants.INVALID_JSONS1[2], "\"");
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public string FormatEventDescription(string rawString)
		{
			if (rawString == "") return rawString;

			string result = "";

			try
			{
				int startIndex = rawString.IndexOf("<textarea");
				int endIndex = rawString.IndexOf(">");

				if (startIndex < 0 || endIndex < 0) return rawString;

				int count = endIndex - startIndex;

				var theString = new StringBuilder(rawString);
				theString.Remove(startIndex, count);

				var returnString = theString.ToString();
				result = returnString.Replace("</textarea><br/>", "");
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public int GetFormatedDurationAsMin(string strTime)
		{
			if (strTime == "") return 0;

			int result = 0;

			try
			{
				var arrTimes = strTime.Split(new char[] { ':' });

				var hrs = int.Parse(arrTimes[0]);
				var min = int.Parse(arrTimes[1]);

				result = hrs * 60 + min;
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public float TotalSecFromString(string strTime)
		{
			if (strTime == "") return 0;

			float result = 0;

			try
			{
				var arrTimes = strTime.Split(new char[] { ':' });

				var hrs = int.Parse(arrTimes[0]);
				var min = int.Parse(arrTimes[1]);
				var sec = int.Parse(arrTimes[2]);

				result = hrs * 3600 + min * 60 + sec;
			}
			catch (Exception ex)
			{
				//ShowTrackMessageBox(ex.Message);
				return 0;
			}

			return result;
		}

		public float ConvertToFloat(string value)
		{
			if (value == null || value == "") return 0;

			float result = 0;

			try
			{
				result = float.Parse(value);
			}
			catch(Exception ex)
			{
				//ShowTrackMessageBox(ex.Message);
				return 0;
			}

			return result;
		}

		public string FormatNumber(string number)
		{
			try
			{
				var fNumber = float.Parse(number);
				return fNumber.ToString("F1");
			}
			catch
			{
				return number;
			}
		}

		public void SaveUserImage(byte[] fileBytes)
		{
			var userId = GetUserID();

			try
			{
				var response = mTrackSvc.saveUserImage(userId, fileBytes, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		public void MarkAsInvalide(ImageView validEmail, LinearLayout errorEmail, bool isInvalid)
		{
			if (validEmail != null)
				validEmail.SetImageResource(isInvalid ? Resource.Drawable.icon_cross : Resource.Drawable.icon_check);

			if (errorEmail != null)
				errorEmail.Visibility = isInvalid ? ViewStates.Visible : ViewStates.Invisible;
		}

		public Bitmap GetPinIconByType(string pointType)
		{
			Bitmap result = null;

			try
			{
				int strPinImg = 0;
				switch (pointType)
				{
					case "1":
					case "START":
						strPinImg = Resource.Drawable.pin_start;
						break;
					case "2":
					case "FINISH":
						strPinImg = Resource.Drawable.pin_finish;
						break;
					case "3":
					case "CHECK_POINT":
						strPinImg = Resource.Drawable.pin_check_mark;
						break;
					case "4":
					case "CAMERA":
						strPinImg = Resource.Drawable.pin_camera;
						break;
					case "5":
					case "NORTH":
						strPinImg = Resource.Drawable.pin_north;
						break;
					case "6":
					case "EAST":
						strPinImg = Resource.Drawable.pin_east;
						break;
					case "7":
					case "SOUTH":
						strPinImg = Resource.Drawable.pin_south;
						break;
					case "8":
					case "WEST":
						strPinImg = Resource.Drawable.pin_west;
						break;
					case "9":
					case "T1":
						strPinImg = Resource.Drawable.pin_T1;
						break;
					case "10":
					case "T2":
						strPinImg = Resource.Drawable.pin_T2;
						break;
					case "pSTART":
						strPinImg = Resource.Drawable.pin_pstart;
						break;
					case "pFINISH":
						strPinImg = Resource.Drawable.pin_pfinish;
						break;
				}

				result = BitmapFactory.DecodeResource(Resources, strPinImg);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public Bitmap ScaleDownImg(Bitmap realImage, float maxImageSize, bool filter)
		{
			Bitmap result = null;

			try
			{
				float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Height);
				int width = (int)Math.Round((float)ratio * realImage.Width);
				int height = (int)Math.Round((float)ratio * realImage.Height);

				result = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		#endregion

		public float DifAlt(float prev, float curr)
		{
			try
			{
				if ((curr - prev) > 0)
					return curr - prev;
				else
					return 0;
			}
			catch
			{
				return 0;
			}
		}

		public DateTime FromUnixTime(long unixTimeMillis)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddMilliseconds(unixTimeMillis);
		}

		public void SetListViewHeightBasedOnChildren(ListView listView)
		{
			try
			{
				GoHejaEventAdapter listAdapter = listView.Adapter as GoHejaEventAdapter;
				if (listAdapter == null)
					return;

				int totalHeight = 0;
				int desiredWidth = View.MeasureSpec.MakeMeasureSpec(listView.Width, MeasureSpecMode.AtMost);
				for (int i = 0; i < listAdapter.Count; i++)
				{
					View listItem = listAdapter.GetView(i, null, listView);
					int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
					listItem.Measure(desiredWidth, heightSpec);
					totalHeight += listItem.MeasuredHeight;
				}

				ViewGroup.LayoutParams lp = listView.LayoutParameters;
				lp.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
				listView.LayoutParameters = lp;
				listView.RequestLayout();
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}
	}
}
