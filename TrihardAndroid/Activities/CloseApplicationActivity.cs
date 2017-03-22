using Android.App;
using Android.Content.PM;
using Android.OS;

namespace goheja
{
    [Activity(Label = "Closing Application", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CloseApplicationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CancelAll();
            FinishAffinity();
            Finish();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Process.KillProcess(Process.MyPid());
        }
    }
}