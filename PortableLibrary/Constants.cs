
namespace PortableLibrary
{
	public static class Constants
	{
		public const string GOOGLE_MAP_API_KEY = "AIzaSyAiBwRUm_KZDv_sp3eI7F8hxkePqDTvY20";
		public const int MAP_ZOOM_LEVEL = 15;

		#region specification for each group
		public const string SPEC_GROUP_TYPE = "trihard" ;
		public const string DEVICE_CALENDAR_TITLE = "Trihard Calendar";
		public const string GROUP_COLOR = "AD3290";
		public const string PATH_USER_IMAGE = "data/goheja.trihard.com/files/me.png";
		public const string MSG_SIGNUP_FAIL = "You are not registered to Trihard services.";
		#endregion

		public const string GOHEJA_BASEPATH = "http://go-heja.com/Service1.svc?wsdl";

		//URLs
		public const string URL_TERMS = "http://go-heja.com/nitro/terms.php/";
		public const string URL_EVENT_MAP = "http://go-heja.com/nitro/calenPage.php?name={0}&startdate={1}&user={2}";
		public const string URL_ANALYTICS_MAP = "http://go-heja.com:8080/nitro/mobongoing.php?txt={0}";
		public const string URL_CALENDAR = "http://go-heja.com/nitro/mobda.php?userNickName={0}&userId={1}";
		public const string URL_WATCH = "http://go-heja.com:8080/gh/mob/sync.php?userId={0}&mog={1}&url=uurrll";
		public const string URL_GAUGE = "http://go-heja.com:8080/innovi/mobGraph/totalGauge.php?userId={0}";
		public const string URL_GOOGLE = "www.google.com";

		public static double[] LOCATION_ISURAEL = { 31.0461, 34.8516 };

		public const string MSG_LOGIN = "Login...";
		public const string MSG_LOGIN_FAIL = "Login failed.";
		public const string MSG_SIGNUP = "Sign Up...";
		public const string MSG_LOADING_DATA = "Loading data...";
		public const string MSG_LOADING_USER_DATA = "Loading user data...";
		public const string MSG_LOADING_EVENTS = "Loading events...";
		public const string MSG_LOADING_EVENT_DETAIL = "Loading event details...";
		public const string MSG_LOADING_ALL_MARKERS = "Loading event all markers...";
		public const string MSG_TYPE_COMMENT = "Type your comment...";
		public const string MSG_SAVE_COMMENT = "Saving your comment...";
		public const string MSG_ADJUST_TRAINING = "Adjusting Trainning...";
		public const string MSG_NO_INTERNET = "No internet connection!";
		public const string MSG_CHANGE_PASSWORD = "Requesting change password...";
		public const string MSG_CHANGE_PW_SUC = "Password updated successfully";
		public const string MSG_CHANGE_PW_FAIL = "Passwords don’t match";
		public const string MSG_CHANGE_PW_EMAIL_FAIL = "Email do not exists in the system , please try again or signup";
		public const string MSG_FORGOT_PASSWORD = "Requesting forgot password...";
		public const string MSG_FORGOT_PW_SUC = "A mail was sent to you with a temporary code.";
		public const string MSG_FORGOT_PW_EMAIL_FAIL = "Email do not exists in the system , please try again or signup.";
		public const string MSG_GPS_DISABLED = "GPS settings disabled.";
		public const string MSG_COMFIRM_STOP_SPORT_COMP = "You sure you want to stop practice?";

		//TYPES
		public const string TOTALS_ES_TIME = "Elapsed time";
		public const string TOTALS_DISTANCE = "Total Distance";
		public const string TOTALS_LOAD = "Load";
		public const string TOTALS_TYPE = "Type";

		public const string PICKER_TIME = "time";
		public const string PICKER_RANKING = "ranking";
		public const string PICKER_HR = "hr";
		public const string PICKER_PACE = "pace";

		public const string PICKER_ATIME = "adjustTime";
		public const string PICKER_ADISTANCE = "adjustDistance";
		public const string PICKER_ATSS = "adjustTSS";

		public const string PICKER_PTYPE = "type";

		public static string[] PRACTICE_TYPES = {
			"Cycling",
			"Running",
			"Swimming",
			"TRI",
			"Other"
		};

		public const string UNIT_SWIM = "minutes per 100 meter";
		public const string UNIT_RUN = "minutes per kilometer";

		public static string[] INVALID_JSONS1 = {	"ObjectId(\"", 
													" ISODate(\"", 
													"\")" 
												};

		public static string[] INVALID_JSONS2 = { 	"<textarea id =\"genData\" class=\"generalData\" name=\"pDesc\"  placeholder=\"Right here coach\" maxlength=\"1000\">",
													"<textarea  dir=\"rtl\" lang=\"ar\"id =\"genData\" class=\"pGenData\" name=\"pDesc\"  placeholder=\"Practice details\" maxlength=\"1000\">",
													"</textarea><br/>"
												};

		public const int TAG_iOS_COLLEPS_PHYSICAL = 130;
		public const int TAG_iOS_COLLEPS_GOALS = 100;
		public const int TAG_iOS_COLLEPS_BEST_RESULTS = 220;
		public const int TAG_iOS_COLLEPS_SELF_RANKING = 1000;

		public const int TAG_ANDROID_COLLEPS_PHYSICAL = 101;
		public const int TAG_ANDROID_COLLEPS_GOALS = 102;
		public const int TAG_ANDROID_COLLEPS_BEST_RESULTS = 103;
		public const int TAG_ANDROID_COLLEPS_SELF_RANKING = 104;

		public const int TAG_EDIT_PHYSICAL = 1001;
		public const int TAG_EDIT_GOALS = 1002;
		public const int TAG_EDIT_BEST_RESULTS = 1003;
		public const int TAG_EDIT_SELF_RANKING = 1004;
		public const int TAG_EDIT_SWIM = 1005;
		public const int TAG_EDIT_RUN = 1006;
		public const int TAG_EDIT_BIKE = 1007;


		public enum PICKER_TYPE
		{
			TIME,
			DATE
		}

		public const int AVAILABLE_DISTANCE_MAP = 200;
	}
}
