using Foundation;

namespace location2
{
	public static class AppSettings
	{
		private const string userIDKey = "userID";
		public static string UserID
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(userIDKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, userIDKey);
			}
		}

		private const string deviceIDKey = "deviceID";
		public static string DeviceID
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(deviceIDKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, deviceIDKey);
			}
		}

		private const string deviceUDIDKey = "deviceUDID";
		public static string DeviceUDID
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(deviceUDIDKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, deviceUDIDKey);
			}
		}

		private const string emailKey = "email";
		public static string Email
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(emailKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, emailKey);
			}
		}

		private const string passwordKey = "password";
		public static string Password
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(passwordKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, passwordKey);
			}
		}

		private const string usernameKey = "usernameKey";
		public static string Username
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey(usernameKey); }
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString(value, usernameKey);
			}
		}
	}
}
