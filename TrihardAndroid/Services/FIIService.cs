using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Firebase.Iid;
using PortableLibrary;

namespace goheja.Services
{
    [Service(Label = "FIIService")]
	[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class FIIService : FirebaseInstanceIdService
	{
		public override async void OnTokenRefresh()
		{
			var refreshedToken = FirebaseInstanceId.Instance.Token;
			Debug.WriteLine("Refreshed token: " + refreshedToken);

			await SendRegistrationToServer(refreshedToken);
		}
		
		async Task SendRegistrationToServer(string token)
		{
            if (AppSettings.CurrentUser == null) return;

            var currentUser = AppSettings.CurrentUser;
            currentUser.fcmToken = token;
            AppSettings.CurrentUser = currentUser;

            await FirebaseService.RegisterFCMUser(currentUser);
		}
	}
}
