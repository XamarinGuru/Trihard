using System.Collections.Generic;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using PortableLibrary;
using PortableLibrary.Model;

namespace goheja
{
	public static class AppSettings
	{
		static ISharedPreferences _appSettings = Application.Context.GetSharedPreferences("App_settings", FileCreationMode.Private);

		public static BaseActivity baseVC;

		public static List<GoHejaEvent> currentEventsList;
        public static ReportData currentEventReport;
		public static GoHejaEvent selectedEvent;

		public static List<AthleteInSubGroup> selectedAthletesInSubGroup;

		public static bool isFakeUser;
        public static string fakeUserName;

		private const string userKey = "userKey";
		public static LoginUser CurrentUser
		{
			get
			{
				try
				{
					var strCurrentUser = _appSettings.GetString(userKey, "");
					return JsonConvert.DeserializeObject<LoginUser>(strCurrentUser);
				}
				catch
				{
					return null;
				}
			}
			set
			{
				var strUser = JsonConvert.SerializeObject(value);
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(userKey, strUser);
				editor.Apply();
			}
		}
	}
}
