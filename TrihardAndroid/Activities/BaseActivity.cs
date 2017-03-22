using System;
using System.Collections.Generic;
using System.Text;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableLibrary;

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

		public void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetPositiveButton("Cancel", (senderAlert, args) =>
			{
			});
			alert.SetNegativeButton("OK", (senderAlert, args) =>
			{
				successHandler();
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		//public void ShowMessageBox(string title, string message)
		//{
		//	ShowMessageBox(title, message, "Ok", null, null);
		//}

		public void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetCancelable(false);
			alert.SetPositiveButton("OK", delegate { if (isFinish) Finish(); });
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		protected void OnBack()
		{
			base.OnBackPressed();
			OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
		}

		protected void ActionBackCancel()
		{
			var activity = new Intent();
			SetResult(Result.Canceled, activity);
			Finish();
		}

		public bool IsNetEnable()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

			if (!isOnline)
			{
				ShowMessageBox(null, Constants.MSG_NO_INTERNET);
				return false;
			}
			return true;
		}

		#region integrate with web reference

		#region USER_MANAGEMENT
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			var result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified, Constants.SPEC_GROUP_TYPE);
			return result;
		}

		public bool LoginUser(string email, string password)
		{
			try
			{
				var userID = mTrackSvc.getListedDeviceId(email, password, Constants.SPEC_GROUP_TYPE);

				if (userID == "0")
					return false;

				AppSettings.UserID = userID;
				return true;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return false;
			}
		}

		public void SignOutUser()
		{
			AppSettings.UserID = string.Empty;
			AppSettings.Email = string.Empty;
			AppSettings.Password = string.Empty;
			AppSettings.DeviceID = string.Empty;
			AppSettings.DeviceUDID = string.Empty;
		}

		public string GetCode(string email)
		{
			try
			{
				var response = mTrackSvc.getCode(email, Constants.SPEC_GROUP_TYPE);

				return response;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public int ResetPassword(string email, string password)
		{
			try
			{
				int result = 0;
				bool resultSpecified = false;
				mTrackSvc.restPasword(email, password, Constants.SPEC_GROUP_TYPE, out result, out resultSpecified);

				return result;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return 0;
			}
		}

		public string GetUserID()
		{
			var userID = AppSettings.UserID;
			if (userID != null && userID != "0" && userID != string.Empty)
				return userID;

			if (AppSettings.Email == string.Empty || AppSettings.Password == string.Empty || AppSettings.Email == null || AppSettings.Password == null)
				return "0";

			try
			{
				userID = mTrackSvc.getListedDeviceId(AppSettings.Email, AppSettings.Password, Constants.SPEC_GROUP_TYPE);

				if (userID != "0")
					AppSettings.UserID = userID;

				return userID;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return "0";
			}
		}

		public RootMember GetUserObject()
		{
			var userID = GetUserID();

			try
			{
				var objUser = mTrackSvc.getUsrObject(userID, Constants.SPEC_GROUP_TYPE);
				var jsonUser = FormatJsonType(objUser.ToString());
				RootMember rootMember = JsonConvert.DeserializeObject<RootMember>(jsonUser);
				return rootMember;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}

		public string UpdateUserDataJson(RootMember updatedUserObject, string updatedById = null)
		{
			var userID = GetUserID();
			var jsonUser = JsonConvert.SerializeObject(updatedUserObject);
			Console.WriteLine(jsonUser);
			updatedById = userID;
			var result = mTrackSvc.updateUserDataJson(userID, jsonUser, updatedById, Constants.SPEC_GROUP_TYPE);
			return result;
		}
		#endregion

		#region USER_GAUGE & PERFORMANCE DATA
		public Gauge GetGauge()
		{
			var userID = GetUserID();

			try
			{
				var strGauge = mTrackSvc.getGaugeMob(DateTime.Now, true, userID, null, Constants.SPEC_GROUP_TYPE, null, "5");
				Gauge gaugeObject = JsonConvert.DeserializeObject<Gauge>(strGauge);
				return gaugeObject;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}

		public ReportGraphData GetPerformance()
		{
			var userID = GetUserID();

			try
			{
				var strPerformance = mTrackSvc.getUserPmc(userID, Constants.SPEC_GROUP_TYPE);
				var performanceObject = JsonConvert.DeserializeObject<ReportGraphData>(strPerformance.ToString());
				return performanceObject;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}

		public PerformanceDataForDate GetPerformanceForDate(DateTime date)
		{
			var userID = GetUserID();

			try
			{
				var strPerformance = mTrackSvc.getPerformanceFordate(userID, date, true, Constants.SPEC_GROUP_TYPE);
				var performanceObject = JsonConvert.DeserializeObject<PerformanceDataForDate>(strPerformance.ToString());
				return performanceObject;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}
		#endregion

		#region EVENT_MANAGEMENT
		public List<GoHejaEvent> GetPastEvents()
		{
			try
			{
				var strPastEvents = mTrackSvc.getUserCalendarPast(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strPastEvents));
				return CastGoHejaEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<GoHejaEvent> GetTodayEvents()
		{
			try
			{
				var strTodayEvents = mTrackSvc.getUserCalendarToday(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strTodayEvents));
				return CastGoHejaEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<GoHejaEvent> GetFutureEvents()
		{
			try
			{
				var strFutureEvents = mTrackSvc.getUserCalendarFuture(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strFutureEvents));
				return CastGoHejaEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public GoHejaEvent GetEventDetail(string eventID)
		{
			try
			{
				var strEventDetail = mTrackSvc.getEventMob(eventID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strEventDetail.ToString()));
				return CastGoHejaEvents(eventsData)[0];
			}
			catch (Exception err)
			{
				//ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<GoHejaEvent> CastGoHejaEvents(JArray events)
		{

			var returnEvents = new List<GoHejaEvent>();

			if (events == null) return returnEvents;

			foreach (var eventJson in events)
			{
				GoHejaEvent goHejaEvent = JsonConvert.DeserializeObject<GoHejaEvent>(eventJson.ToString());
				returnEvents.Add(goHejaEvent);
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
				//ShowMessageBox(null, ex.Message);
				return null;
			}
			return eventTotal;
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
				//ShowMessageBox(null, ex.Message);
				return null;
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
				//ShowMessageBox(null, ex.Message);
				return null;
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
						returnTPoints.Add(tPoints);
					}
				}

			}
			catch (Exception ex)
			{
				//ShowMessageBox(null, ex.Message);
				return null;
			}
			return returnTPoints;
		}

		public Color GetRandomColor(int index)
		{
			return PATH_COLORS[index % 3];
		}

		public Comment GetComments(string eventID, string type = "1")
		{
			var comment = new Comment();
			try
			{
				var commentObject = mTrackSvc.getComments(eventID, "1", Constants.SPEC_GROUP_TYPE);
				comment = JsonConvert.DeserializeObject<Comment>(commentObject.ToString());
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return null;
			}
			return comment;
		}

		public object SetComment(string author, string authorId, string commentText, string eventId)
		{
			try
			{
				var response = mTrackSvc.setComments(author, authorId, commentText, eventId, Constants.SPEC_GROUP_TYPE);
				return response;
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return null;
			}
		}

		public void UpdateMemberNotes(string notes, string userID, string eventId, string username, string attended, string duration, string distance, string trainScore, string type)
		{
			try
			{
				var response = mTrackSvc.updateMeberNotes(notes, userID, eventId, username, attended, duration, distance, trainScore, type, Constants.SPEC_GROUP_TYPE);
				//return response;
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return;
			}
		}

		public void SaveUserImage(byte[] fileBytes)
		{
			try
			{
				var response = mTrackSvc.saveUserImage(AppSettings.UserID, fileBytes, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, "Save error\n" + err.Message);
			}
		}

		public void UpdateMomgoData(string name,
					string loc,
					System.DateTime time,
					bool timeSpecified,
					string deviceID,
					float speed,
					bool speedSpecified,
					string id,
					string country,
					float dist,
					bool distSpecified,
					float alt,
					bool altSpecified,
					float bearing,
					bool bearingSpecified,
					int recordType,
					bool recordTypeSpecified,
					string eventType,
					string specGroup)
		{
			mTrackSvc.updateMomgoDataAsync(name, loc, time, timeSpecified, deviceID, speed, speedSpecified, id, country, dist, distSpecified, alt, altSpecified, bearing, bearingSpecified, recordType, recordTypeSpecified, eventType, specGroup, null);
		}
		#endregion

		public string GetTypeStrFromID(string typeID)
		{
			return Constants.PRACTICE_TYPES[int.Parse(typeID) - 1];
		}
		public string GetTypeIDFromStr(string typeStr)
		{
			return (Array.IndexOf(Constants.PRACTICE_TYPES, typeStr) + 1).ToString();
		}

		public double DistanceAtoB(TPoint pA, TPoint pB)
		{
			Location pointA = new Location("");
			pointA.Latitude = pA.Latitude;
			pointA.Longitude = pA.Longitude;

			Location pointB = new Location("");
			pointB.Latitude = pB.Latitude;
			pointB.Longitude = pB.Longitude;

			float distance = pointA.DistanceTo(pointB);

			return distance;
		}

		public void CompareEventResult(float planned, float total, TextView lblPlanned, TextView lblTotal)
		{
			if (planned == total || planned == 0 || total == 0)
			{
				lblPlanned.SetTextColor(COLOR_ORANGE);
				lblTotal.SetTextColor(COLOR_ORANGE);
				return;
			}

			if (planned > total)
			{
				var delta = (planned - total) / total;
				if (delta < 0.15)
				{
					lblPlanned.SetTextColor(COLOR_ORANGE);
					lblTotal.SetTextColor(COLOR_ORANGE);
				}
				else {
					lblPlanned.SetTextColor(COLOR_BLUE);
					lblTotal.SetTextColor(COLOR_BLUE);
				}
			}
			else if (planned < total)
			{
				var delta = (total - planned) / planned;
				if (delta < 0.15)
				{
					lblPlanned.SetTextColor(COLOR_ORANGE);
					lblTotal.SetTextColor(COLOR_ORANGE);
				}
				else {
					lblPlanned.SetTextColor(COLOR_RED);
					lblTotal.SetTextColor(COLOR_RED);
				}
			}
		}

		public bool ValidateUserNickName(string nickName)
		{
			var validate = mTrackSvc.validateNickName(nickName, Constants.SPEC_GROUP_TYPE);
			if (validate != "1")
			{
				return true;
			}
			else {
				return false;
			}
		}

		private string FormatJsonType(string jsonData)
		{
			var returnString = jsonData.Replace(Constants.INVALID_JSONS1[0], "\"");
			returnString = returnString.Replace(Constants.INVALID_JSONS1[1], "\"");
			returnString = returnString.Replace(Constants.INVALID_JSONS1[2], "\"");

			return returnString;
		}

		public string FormatEventDescription(string rawString)
		{
			if (rawString == "") return rawString;

			int startIndex = rawString.IndexOf("<textarea");
			int endIndex = rawString.IndexOf(">");

			if (startIndex < 0 || endIndex < 0) return rawString;

			int count = endIndex - startIndex;

			var theString = new StringBuilder(rawString);
			theString.Remove(startIndex, count);

			var returnString = theString.ToString();
			returnString = returnString.Replace("</textarea><br/>", "");
			return returnString;
		}

		public int GetFormatedDurationAsMin(string strTime)
		{
			if (strTime == "") return 0;

			var arrTimes = strTime.Split(new char[] { ':' });

			var hrs = int.Parse(arrTimes[0]);
			var min = int.Parse(arrTimes[1]);

			return hrs * 60 + min;
		}

		public float TotalSecFromString(string strTime)
		{
			if (strTime == "") return 0;

			try
			{
				var arrTimes = strTime.Split(new char[] { ':' });

				var hrs = int.Parse(arrTimes[0]);
				var min = int.Parse(arrTimes[1]);
				var sec = int.Parse(arrTimes[2]);

				return hrs * 3600 + min * 60 + sec;
			}
			catch
			{
				return 0;
			}
		}

		public float ConvertToFloat(string value)
		{
			if (value == null || value == "")
				return 0;

			try
			{
				return float.Parse(value);
			}
			catch
			{
				return 0;
			}
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

		public void MarkAsInvalide(ImageView validEmail, LinearLayout errorEmail, bool isInvalid)
		{
			if (validEmail != null)
				validEmail.SetImageResource(isInvalid ? Resource.Drawable.icon_cross : Resource.Drawable.icon_check);

			if (errorEmail != null)
				errorEmail.Visibility = isInvalid ? ViewStates.Visible : ViewStates.Invisible;
		}

		public Bitmap GetPinIconByType(string pointType)
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

			return BitmapFactory.DecodeResource(Resources, strPinImg);
		}

		public static Bitmap ScaleDownImg(Bitmap realImage, float maxImageSize, bool filter)
		{
			float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Height);
			int width = (int)Math.Round((float)ratio * realImage.Width);
			int height = (int)Math.Round((float)ratio * realImage.Height);

			Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
			return newBitmap;
		}

		#endregion

		public float difAlt(float prev, float curr)
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
	}
}
