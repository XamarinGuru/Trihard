
using Android.App;
using Android.OS;

namespace goheja
{
    [Activity(Label = "NotificationActivity")]
    public class NotificationActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBackCancel();
        }
    }
}
