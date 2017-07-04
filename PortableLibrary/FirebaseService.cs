﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using PortableLibrary.Model;

namespace PortableLibrary
{
    public static class FirebaseService
    {
        public static HttpClient _httpClient;

        public static FirebaseClient _firebase = new FirebaseClient(Constants.URL_FBDB_BASE);

        public static HttpClient GetHttpClientInstance()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + Constants.FCM_SERVER_KEY);
            }

            return _httpClient;
        }

        public static async Task SendNotification(FCMDataNotification nContent, List<string> recipientIDs)
        {
            var httpClient = GetHttpClientInstance();

            try
            {
                var registration_ids = await GetFCMUserTokens(recipientIDs);

                if (registration_ids.Count == 0) return;

                var objNotification = new FBNotification();
                objNotification.data = nContent;

                if (registration_ids["iOSTokens"].Count > 0)
                {
                    objNotification.notification = new FCMDisplayNotification(nContent);
                    objNotification.registration_ids = registration_ids["iOSTokens"];
                    var jsonNotification = JsonConvert.SerializeObject(objNotification);

                    StringContent content = new StringContent(jsonNotification, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(Constants.URL_FCM_BASE, content);
                    var result = response.Content.ReadAsStringAsync().Result;
                }

                if (registration_ids["AndroidTokens"].Count > 0)
                {
                    objNotification.notification = null;
                    objNotification.registration_ids = registration_ids["AndroidTokens"];
                    var jsonNotification = JsonConvert.SerializeObject(objNotification);

                    StringContent content = new StringContent(jsonNotification, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(Constants.URL_FCM_BASE, content);
                    var result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static async Task<bool> RegisterFCMUser(LoginUser user, bool isFCMUpdate = false)
        {
            try
            {
                if (user.fcmToken == null || user.userId == null) return false;

                var fcmUsers = await _firebase.Child("FCMUsers").OrderByKey().OnceAsync<LoginUser>();

				foreach (var fcmUser in fcmUsers)
				{
                    if (fcmUser.Object.fcmToken == user.fcmToken || 
                        (fcmUser.Object.userId == user.userId && fcmUser.Object.osType == user.osType))
					{
                        if (!isFCMUpdate) user.isFcmOn = fcmUser.Object.isFcmOn;

						await _firebase.Child("FCMUsers").Child(fcmUser.Key).PutAsync(user);
						Debug.WriteLine("FCMUser Updated.");
                        return user.isFcmOn;
                    }
                }

                await _firebase.Child("FCMUsers").PostAsync(user);
                Debug.WriteLine("new FCMUser added.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
			return true;
        }

		public static async Task RemoveFCMUser(LoginUser user)
		{
			try
			{
				if (user.fcmToken == null || user.userId == null) return;

				var fcmUsers = await _firebase.Child("FCMUsers").OrderByKey().OnceAsync<LoginUser>();

				foreach (var fcmUser in fcmUsers)
				{
					if (fcmUser.Object.fcmToken == user.fcmToken ||
						(fcmUser.Object.userId == user.userId && fcmUser.Object.osType == user.osType))
					{
                        await _firebase.Child("FCMUsers").Child(fcmUser.Key).DeleteAsync();
						Debug.WriteLine("FCMUser Removed.");
						return;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return;
		}

        public static async Task<Dictionary<string, List<string>>> GetFCMUserTokens(List<string> recipientIDs)
        {
            var fcmiOSTokens = new List<string>();
            var fcmAndroidTokens = new List<string>();

            try
            {
                var users = await _firebase.Child("FCMUsers").OrderByKey().OnceAsync<LoginUser>();

                foreach (var redipientID in recipientIDs)
                {
                    foreach (var objUser in users)
                    {
                        var user = objUser.Object;
						if (user.userId == redipientID && user.isFcmOn)
							switch (user.osType)
                            {
                                case Constants.OS_TYPE.iOS:
                                    fcmiOSTokens.Add(user.fcmToken);
                                    break;
                                case Constants.OS_TYPE.Android:
                                    fcmAndroidTokens.Add(user.fcmToken);
                                    break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return new Dictionary<string, List<string>>
            {
                {"iOSTokens", fcmiOSTokens},
                {"AndroidTokens", fcmAndroidTokens}
            };
        }


		public static async Task<bool> GetNotificationSetting(LoginUser currentUser)
		{
			try
			{
				var users = await _firebase.Child("FCMUsers").OrderByKey().OnceAsync<LoginUser>();

				foreach (var objUser in users)
				{
					var user = objUser.Object;
					if (currentUser.userId == user.userId && currentUser.osType == user.osType)
					{
						return user.isFcmOn;
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return false;
		}

	}
}
