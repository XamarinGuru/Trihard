using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using System.Threading.Tasks;
using Android.Content.PM;
using PortableLibrary;

namespace goheja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]

    public class SplashActivity : BaseActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Task.Delay(500);  
            });

            startupWork.ContinueWith(t =>
            {
                initiatAth();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

        public void initiatAth()
        {
			if (!IsNetEnable()) return;

            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, CreateNotification());

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				var currentUser = AppSettings.CurrentUser;

				Intent nextIntent = new Intent(this, typeof(InitActivity));
				if (currentUser != null)
				{
					if (currentUser.userType == (int)Constants.USER_TYPE.ATHLETE)
					{
						nextIntent = new Intent(this, typeof(SwipeTabActivity));
					}
					else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
					{
						nextIntent = new Intent(this, typeof(CoachHomeActivity));
					}
				}

				StartActivityForResult(nextIntent, 0);
				Finish();
			});

        }

        public Notification CreateNotification()
        {
			var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(SplashActivity)), PendingIntentFlags.UpdateCurrent);
			var builder = new NotificationCompat.Builder(this)
			                                    .SetContentTitle(ApplicationInfo.LoadLabel(PackageManager) + " on the go")
			                                    .SetSmallIcon(Resource.Drawable.icon_notification).SetPriority(1)
			                                    .SetContentIntent(contentIntent)
			                                    .SetCategory("tst")
			                                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(Html.FromHtml("Tap to Open")))
			                                    .SetContentText(Html.FromHtml("Tap to Open"));
			
            var clossIntent = new Intent(this, typeof(CloseApplicationActivity));
            clossIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);
            var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
            var action = new NotificationCompat.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
            builder.AddAction(action);

            var n = builder.Build();
            n.Flags |= NotificationFlags.NoClear;
            return n;
        }
    }
}

