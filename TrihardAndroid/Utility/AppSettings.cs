using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using PortableLibrary;

namespace goheja
{
	public static class AppSettings
	{
		private static ISharedPreferences _appSettings = Application.Context.GetSharedPreferences("App_settings", FileCreationMode.Private);

		public static BaseActivity baseVC;

		public static List<GoHejaEvent> currentEventsList;
		public static EventTotal currentEventTotal;

		public static GoHejaEvent selectedEvent;

		public static string currentEmail;

		private const string userIDKey = "userID";
		public static string UserID
		{
			get
			{
				return _appSettings.GetString(userIDKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(userIDKey, value);
				editor.Apply();
			}
		}

		private const string deviceIDKey = "deviceID";
		public static string DeviceID
		{
			get
			{
				return _appSettings.GetString(deviceIDKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(deviceIDKey, value);
				editor.Apply();
			}
		}

		private const string deviceUDIDKey = "deviceUDID";
		public static string DeviceUDID
		{
			get
			{
				return _appSettings.GetString(deviceUDIDKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(deviceUDIDKey, value);
				editor.Apply();
			}
		}

		private const string emailKey = "email";
		public static string Email
		{
			get
			{
				return _appSettings.GetString(emailKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(emailKey, value);
				editor.Apply();
			}
		}

		private const string passwordKey = "password";
		public static string Password
		{
			get
			{
				return _appSettings.GetString(passwordKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(passwordKey, value);
				editor.Apply();
			}
		}

		private const string usernameKey = "usernameKey";
		public static string Username
		{
			get
			{
				return _appSettings.GetString(usernameKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(usernameKey, value);
				editor.Apply();
			}
		}
	}
}
