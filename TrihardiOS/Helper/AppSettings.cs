using Foundation;
using Newtonsoft.Json;
using PortableLibrary;

namespace location2
{
	public static class AppSettings
	{
		public static bool isFakeUser;
		public static string fakeUserName;

		private const string userKey = "userKey";
		public static LoginUser CurrentUser
		{
			get
			{
				try
				{
					var strCurrentUser = NSUserDefaults.StandardUserDefaults.StringForKey(userKey);
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
				NSUserDefaults.StandardUserDefaults.SetString(strUser, userKey);
			}
		}
	}
}
