using System;
using UIKit;
using BigTed;
using Foundation;
using System.Drawing;
using Newtonsoft.Json;
using PortableLibrary;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using CoreLocation;
using CoreGraphics;
using System.Runtime.CompilerServices;

namespace location2
{
	public partial class BaseViewController : UIViewController
	{
		public UIColor GROUP_COLOR;
		UIColor[] PATH_COLORS = { UIColor.Red, new UIColor(38 / 255f, 127 / 255f, 0, 1.0f), UIColor.Blue };
		UIColor COLOR_ORANGE = new UIColor(229 / 255f, 161 / 255f, 9 / 225f, 1.0f);
		UIColor COLOR_RED = new UIColor(179 / 255f, 66 / 255f, 17 / 225f, 1.0f);
		UIColor COLOR_BLUE = new UIColor(21 / 255f, 181 / 255f, 98 / 225f, 1.0f);

		protected float scroll_amount = 0.0f;
		protected bool moveViewUp = false;

		public MainPageViewController rootVC;
		public int pageIndex = 0;
		public string titleText;
		public string imageFile;

		trackSvc.Service1 mTrackSvc = new trackSvc.Service1();
		Reachability.Reachability mConnection = new Reachability.Reachability();

		protected RootMemberModel MemberModel = new RootMemberModel();

		public BaseViewController() : base()
		{
		}
		public BaseViewController(IntPtr handle) : base(handle)
		{
			SetGroupColor();
		}

		void SetGroupColor()
		{
			var red = Convert.ToInt32(Constants.GROUP_COLOR.Substring(0, 2), 16) / 255f;
			var green = Convert.ToInt32(Constants.GROUP_COLOR.Substring(2, 2), 16) / 255f;
			var blue = Convert.ToInt32(Constants.GROUP_COLOR.Substring(4, 2), 16) / 255f;
			GROUP_COLOR = UIColor.FromRGB(red, green, blue);
		}

		protected void ShowLoadingView(string title)
		{
			InvokeOnMainThread(() => { BTProgressHUD.Show(title, -1, ProgressHUD.MaskType.Black); });
		}

		protected void HideLoadingView()
		{
			InvokeOnMainThread(() => { BTProgressHUD.Dismiss(); });
		}

		// Show the alert view
		protected void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			var alertView = new UIAlertView(title, message, null, cancelButton, otherButtons);
			alertView.Clicked += (sender, e) =>
			{
				if (e.ButtonIndex == 0)
				{
					return;
				}
				if (successHandler != null)
				{
					successHandler();
				}
			};
			alertView.Show();
		}

		protected void ShowMessageBox1(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			InvokeOnMainThread(() =>
			{
				var alertView = new UIAlertView(title, message, null, cancelButton, otherButtons);
				alertView.Clicked += (sender, e) =>
				{
					successHandler();
				};
				alertView.Show();

			});
		}

		protected void ShowTrackMessageBox(string trackMsg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
		{
			InvokeOnMainThread(() =>
			{
				var alertView = new UIAlertView("", Constants.MSG_COMMON, null, "Ok", null);
				alertView.Clicked += (sender, e) =>
				{
					TrackErrorIntoServer(trackMsg, filePath, lineNumber, caller);
				};
				alertView.Show();
				//ShowMessageBox(title, message, "Ok", null, null); 
			});
		}

		protected void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			InvokeOnMainThread(() =>
			{
				var alertView = new UIAlertView(title, message, null, "Ok", null);
				alertView.Clicked += (sender, e) =>
				{
					if (isFinish)
						CloseApplication();
				};
				alertView.Show();
				//ShowMessageBox(title, message, "Ok", null, null); 
			});
		}

		protected bool TextFieldShouldReturn(UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}

		#region error handling
		public bool IsNetEnable()
		{
			bool isOnline = mConnection.IsHostReachable(Constants.URL_GOOGLE) ? true : false;
			if (!isOnline)
			{
				ShowMessageBox(null, Constants.MSG_NO_INTERNET, true);
				return false;
			}
			return true;
		}

		void TrackErrorIntoServer(string msgError, string filePath, int lineNumber, string caller)
		{
			var dateTime = DateTime.Now.ToString();
			var userId = GetUserID();
			var deviceModel = GetDeviceModel();
			var errorDetail = string.Format(Constants.MSG_TRACK_ERROR_DETAIL, msgError, filePath, lineNumber, caller);
			var msg = string.Format(Constants.MSG_TRACK_ERROR, dateTime, userId, deviceModel, Constants.SPEC_GROUP_TYPE, errorDetail);

			ShowLoadingView(Constants.MSG_TRAKING_ERROR);

			mTrackSvc.specLog(msg);

			HideLoadingView();

			CloseApplication();
			//ShowMessageBox(null, msg);
		}

		public string GetDeviceModel()
		{
			return new iOSHardware().GetModel(DeviceHardware.Version);
		}

		void CloseApplication()
		{
			System.Threading.Thread.CurrentThread.Abort();
		}
		#endregion

		#region integrate with web reference

		#region USER_MANAGEMENT
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			string result = "";

			try
			{
				result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
			return result;
		}

		public bool LoginUser(string email, string password)
		{
			string userID = "0";

			try
			{
				userID = mTrackSvc.getListedDeviceId(email, password, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			if (userID == "0")
			{
				return false;
			}
			else
			{
				AppSettings.UserID = userID;
				return true;
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
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return userID;
		}

		public RootMember GetUserObject()
		{
			RootMember result = new RootMember();

			var userID = GetUserID();

			try
			{
				var objUser = mTrackSvc.getUsrObject(userID, Constants.SPEC_GROUP_TYPE);
				var jsonUser = FormatJsonType(objUser.ToString());
				result = JsonConvert.DeserializeObject<RootMember>(jsonUser);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public string UpdateUserDataJson(RootMember updatedUserObject, string updatedById = null)
		{
			string result = "";

			try
			{
				var userID = GetUserID();
				var jsonUser = JsonConvert.SerializeObject(updatedUserObject);
				Console.WriteLine(jsonUser);
				updatedById = userID;
				result = mTrackSvc.updateUserDataJson(userID, jsonUser, updatedById, Constants.SPEC_GROUP_TYPE);
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

			var userID = GetUserID();

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

			var userID = GetUserID();

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

			var userID = GetUserID();

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
		public List<GoHejaEvent> GetPastEvents()
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

			try
			{
				var strPastEvents = mTrackSvc.getUserCalendarPast(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strPastEvents));
				result = CastGoHejaEvents(eventsData);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public List<GoHejaEvent> GetTodayEvents()
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

			try
			{
				var strTodayEvents = mTrackSvc.getUserCalendarToday(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
				var eventsData = JArray.Parse(FormatJsonType(strTodayEvents));
				result = CastGoHejaEvents(eventsData);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public List<GoHejaEvent> GetFutureEvents()
		{
			List<GoHejaEvent> result = new List<GoHejaEvent>();

			try
			{
				var strFutureEvents = mTrackSvc.getUserCalendarFuture(AppSettings.UserID, Constants.SPEC_GROUP_TYPE);
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
				ShowTrackMessageBox(ex.Message);
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
				//ShowTrackMessageBox(ex.Message);
				return null;
			}
			return returnTPoints;
		}
		public UIColor GetRandomColor(int index)
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
				//ShowTrackMessageBox(ex.Message);
				return null;
			}
			return comment;
		}

		public object SetComment(string author, string authorId, string commentText, string eventId)
		{
			object result = new object();

			try
			{
				result = mTrackSvc.setComments(author, authorId, commentText, eventId, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
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

		public void UpdateMomgoData(string name,
					string loc,
					DateTime time,
					bool timeSpecified,
					string deviceID,
					float speed,
					bool speedSpecified,
					string athId,
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
			try
			{
				mTrackSvc.updateMomgoData(name, loc, time, true, AppSettings.DeviceUDID, speed, true, athId, country, dist, true, alt, true, bearing, true, 1, true, eventType, specGroup);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
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
				CLLocation pointA = new CLLocation(pA.Latitude, pA.Longitude);
				CLLocation pointB = new CLLocation(pB.Latitude, pB.Longitude);
				result = pointA.DistanceFrom(pointB);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}

		public void CompareEventResult(float planned, float total, UILabel lblPlanned, UILabel lblTotal)
		{
			try
			{
				if (planned == total || planned == 0 || total == 0)
				{
					lblPlanned.TextColor = COLOR_ORANGE;
					lblTotal.TextColor = COLOR_ORANGE;
					return;
				}

				if (planned > total)
				{
					var delta = (planned - total) / total;
					if (delta < 0.15)
					{
						lblPlanned.TextColor = COLOR_ORANGE;
						lblTotal.TextColor = COLOR_ORANGE;
					}
					else
					{
						lblPlanned.TextColor = COLOR_BLUE;
						lblTotal.TextColor = COLOR_BLUE;
					}
				}
				else if (planned < total)
				{
					var delta = (total - planned) / planned;
					if (delta < 0.15)
					{
						lblPlanned.TextColor = COLOR_ORANGE;
						lblTotal.TextColor = COLOR_ORANGE;
					}
					else
					{
						lblPlanned.TextColor = UIColor.Red;
						lblTotal.TextColor = UIColor.Red;
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

		public string FormatEventDescription(string eventJson)
		{
			var returnString = eventJson.Replace(Constants.INVALID_JSONS2[0], "");
			returnString = returnString.Replace(Constants.INVALID_JSONS2[1], "");
			returnString = returnString.Replace(Constants.INVALID_JSONS2[2], "");

			return returnString;
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
			catch (Exception ex)
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

		public void SaveUserImage(UIImage img)
		{
			try
			{
				var scaledImage = MaxResizeImage(img, 100, 100);
				NSData imgData = scaledImage.AsPNG();
				//save to local
				var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");

				NSError err = null;
				if (!imgData.Save(jpgFilename, false, out err))
				{
					ShowMessageBox(null, "NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
				}

				//save to server

				var fileBytes = new Byte[imgData.Length];
				System.Runtime.InteropServices.Marshal.Copy(imgData.Bytes, fileBytes, 0, Convert.ToInt32(imgData.Length));

				var response = mTrackSvc.saveUserImage(AppSettings.UserID, fileBytes, Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		public void MarkAsInvalide(UIButton validEmail, UIView errorEmail, bool isInvalid)
		{
			InvokeOnMainThread(() =>
			{
				if (validEmail != null)
					validEmail.Selected = isInvalid;

				if (errorEmail != null)
					errorEmail.Hidden = !isInvalid;
			});
		}

		public UIImage GetPinIconByType(string pointType)
		{
			UIImage result = null;

			try
			{
				var strPinImg = "";
				switch (pointType)
				{
					case "1":
					case "START":
						strPinImg = "pin_start.png";
						break;
					case "2":
					case "FINISH":
						strPinImg = "pin_finish.png";
						break;
					case "3":
					case "CHECK_POINT":
						strPinImg = "pin_check_mark.png";
						break;
					case "4":
					case "CAMERA":
						strPinImg = "pin_camera.png";
						break;
					case "5":
					case "NORTH":
						strPinImg = "pin_north.png";
						break;
					case "6":
					case "EAST":
						strPinImg = "pin_east.png";
						break;
					case "7":
					case "SOUTH":
						strPinImg = "pin_south.png";
						break;
					case "8":
					case "WEST":
						strPinImg = "pin_west.png";
						break;
					case "9":
					case "T1":
						strPinImg = "pin_T1.png";
						break;
					case "10":
					case "T2":
						strPinImg = "pin_T2.png";
						break;
					case "pSTART":
						strPinImg = "pin_pstart.png";
						break;
					case "pFINISH":
						strPinImg = "pin_pfinish.png";
						break;
				}
				result = UIImage.FromFile(strPinImg);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return result;
		}





		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			try
			{
				var sourceSize = sourceImage.Size;
				var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
				if (maxResizeFactor > 1) return sourceImage;
				var width = maxResizeFactor * sourceSize.Width;
				var height = maxResizeFactor * sourceSize.Height;
				UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
				sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
				var resultImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
				return resultImage;
			}
			catch
			{
				return null;
			}
		}

		public UIImage GetPictureFromLocal()
		{
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");

			UIImage currentImage = UIImage.FromFile(jpgFilename);
			return currentImage;
		}



		protected void SetupDatePicker(UITextField field)
		{
			UIDatePicker picker = new UIDatePicker();
			picker.Mode = UIDatePickerMode.Date;
			picker.MinimumDate = NSDate.Now;
			picker.MaximumDate = NSDate.Now.AddSeconds(60 * 60 * 24 * 365 * 3);

			var format = "{0: MM-dd-yyyy}";
			picker.ValueChanged += (object s, EventArgs e) =>
			{
				//if (changeOnEdit)
				{
					updateSetupDateTimePicker(field, picker.Date, format, s, e);
				}
			};

			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				updateSetupDateTimePicker(field, picker.Date, format, s, e, true);
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			field.InputView = picker;
			field.InputAccessoryView = toolbar;

			field.ShouldChangeCharacters = new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
			{
				return false;
			});
		}

		private void updateSetupDateTimePicker(UITextField field, NSDate date, String format, object sender, EventArgs e, bool done = false)
		{
			var newDate = NSDateToDateTime(date);
			var str = String.Format(format, newDate);

			field.Text = str;
			field.SendActionForControlEvents(UIControlEvent.ValueChanged);
			if (done)
			{
				field.ResignFirstResponder();
			}
		}

		protected void SetupPicker(UITextField field, string type)
		{
			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				field.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			UIPickerViewModel picker_model = new UIPickerViewModel();
			if (type == "time")
				picker_model = new TimePickerViewModel(field);
			else if (type == "ranking")
				picker_model = new RankingPickerViewModel(field);
			else if (type == "hr")
				picker_model = new HRPickerViewModel(field);
			else if (type == "pace")
				picker_model = new PacePickerViewModel(field);
			else if (type == "type")
				picker_model = new PTypePickerViewModel(field);

			UIPickerView picker = new UIPickerView();
			picker.BackgroundColor = UIColor.White;
			picker.Model = picker_model;
			picker.ShowSelectionIndicator = true;

			field.InputView = picker;
			field.InputAccessoryView = toolbar;

			field.ShouldChangeCharacters = new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
			{
				return false;
			});
		}

		#region CLASS RANKING_PICKER
		public class RankingPickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public RankingPickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 5;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return ((int)row + 1).ToString();
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				textField.Text = (pickerView.SelectedRowInComponent(0) + 1).ToString();
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS PRACTICE_TYPE_PICKER
		public class PTypePickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public PTypePickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 5;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return Constants.PRACTICE_TYPES[(int)row];
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				textField.Text = Constants.PRACTICE_TYPES[pickerView.SelectedRowInComponent(0)];
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS RANKING_PICKER
		public class HRPickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public HRPickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 2;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 20;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return (60 + row * 10).ToString();
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var mm = 60 + pickerView.SelectedRowInComponent(0) * 10;
				var ss = 60 + pickerView.SelectedRowInComponent(1) * 10;

				textField.Text = mm + "-" + ss;
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS PACE_PICKER HH:MM:SS
		public class PacePickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public PacePickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 2;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				switch (component)
				{
					case 0:
						return 100;
					case 1:
						return 60;
					default:
						break;
				}
				return 0;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				string title = string.Format("{0:00}", (int)row);
				return title;
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var mm = string.Format("{0:00}", pickerView.SelectedRowInComponent(0));
				var ss = string.Format("{0:00}", pickerView.SelectedRowInComponent(1));

				textField.Text = mm + ":" + ss;
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS TIME_PICKER HH:MM:SS
		public class TimePickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public TimePickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 3;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				switch (component)
				{
					case 0:
						return 100;
					case 1:
						return 60;
					case 2:
						return 60;
					default:
						break;
				}
				return 0;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				string title = string.Format("{0:00}", (int)row);
				return title;
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var hh = string.Format("{0:00}", pickerView.SelectedRowInComponent(0));
				var mm = string.Format("{0:00}", pickerView.SelectedRowInComponent(1));
				var ss = string.Format("{0:00}", pickerView.SelectedRowInComponent(2));

				textField.Text = hh + ":" + mm + ":" + ss;
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		public UIColor FromHexString(string hexValue, float alpha = 1.0f)
		{
			var colorString = hexValue.Replace("#", "");
			if (alpha > 1.0f)
			{
				alpha = 1.0f;
			}
			else if (alpha < 0.0f)
			{
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length)
			{
				case 3: // #RGB
					{
						red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
						green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
						blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
						return UIColor.FromRGBA(red, green, blue, alpha);
					}
				case 6: // #RRGGBB
					{
						red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
						green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
						blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
						return UIColor.FromRGBA(red, green, blue, alpha);
					}

				default:
					throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

			}
		}

		public void SetTableViewHeightBasedOnChildren(UITableView tableView, List<GoHejaEvent> children, NSLayoutConstraint conHeight)
		{
			nfloat height = children.Count * 60;

			CGRect tableFrame = tableView.Frame;
			tableFrame.Height = height;
			tableView.Frame = tableFrame;

			conHeight.Constant = children.Count == 0 ? 80 : height;

			View.LayoutIfNeeded();
		}
		#endregion


		#region NSDate vs DateTime
		public DateTime ConvertUTCToLocalTimeZone(DateTime dateTimeUtc)
		{
			NSTimeZone sourceTimeZone = new NSTimeZone("UTC");
			NSTimeZone destinationTimeZone = NSTimeZone.LocalTimeZone;
			NSDate sourceDate = DateTimeToNativeDate(dateTimeUtc);

			int sourceGMTOffset = (int)sourceTimeZone.SecondsFromGMT(sourceDate);
			int destinationGMTOffset = (int)destinationTimeZone.SecondsFromGMT(sourceDate);
			int interval = sourceGMTOffset - destinationGMTOffset;

			var destinationDate = dateTimeUtc.AddSeconds(interval);
			//var destinationDate = sourceDate.AddSeconds(interval);
			//var dateTime = NativeDateToDateTime(destinationDate);
			return destinationDate;
		}

		public static NSDate DateTimeToNativeDate(DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - reference).TotalSeconds);
		}
		public NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));

			TimeZoneInfo localZone = TimeZoneInfo.Local;

			bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(date);

			if (isDayLight)
				return NSDate.FromTimeIntervalSinceReferenceDate((date - newDate).TotalSeconds - 3600);
			else
				return NSDate.FromTimeIntervalSinceReferenceDate((date - newDate).TotalSeconds);
		}

		protected static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
			reference = reference.AddSeconds(date.SecondsSinceReferenceDate);
			if (reference.IsDaylightSavingTime())
			{
				reference = reference.AddHours(1);
			}
			return reference;
		}
		#endregion
	}
}

