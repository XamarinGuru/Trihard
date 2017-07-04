using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Firebase.Messaging;
using PortableLibrary;

namespace goheja.Services
{
	[Service(Label = "FCMService")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class FCMService : FirebaseMessagingService
	{
		public override void OnMessageReceived(RemoteMessage message)
		{
            var mData = message.Data;
            Debug.WriteLine("senderId: " + mData["senderId"]);
			Debug.WriteLine("senderName: " + mData["senderName"]);
            Debug.WriteLine("practiceId: " + mData["practiceId"]);
            Debug.WriteLine("commentId: " + mData["commentId"]);
			Debug.WriteLine("practiceType: " + mData["practiceType"]);
            Debug.WriteLine("practiceName: " + mData["practiceName"]);
            Debug.WriteLine("practiceDate: " + mData["practiceDate"]);
            Debug.WriteLine("description: " + mData["description"]);
            Debug.WriteLine("osType: " + mData["osType"]);

            SendNotification(mData);
		}

        void SendNotification(IDictionary<string, string> mData)
        {
            var msg = mData["description"].Length > 100 ? mData["description"].Substring(0, 100) : mData["description"];

            var textStyle = new NotificationCompat.InboxStyle();
            textStyle.SetBigContentTitle("Notification from " + mData["senderName"]);
            textStyle.AddLine("Event Type : " + mData["practiceType"]);
            textStyle.AddLine("Event Name : " + mData["practiceName"]);
            textStyle.AddLine("Event Date : " + String.Format("{0:t}", mData["practiceDate"]));
            textStyle.AddLine(" ");
            textStyle.AddLine(msg);
            textStyle.SetSummaryText("Tap to open");

            var intent = new Intent(this, typeof(EventInstructionActivity));
            intent.PutExtra("FromWhere", "RemoteNotification");
            intent.PutExtra("SelectedEventID", mData["practiceId"]);
            intent.PutExtra("senderId", mData["senderId"]);
            intent.PutExtra("senderName", mData["senderName"]);
            intent.PutExtra("commentId", mData["commentId"]);
            intent.AddFlags(ActivityFlags.ClearTop);

            Android.App.TaskStackBuilder stackBuilder = Android.App.TaskStackBuilder.Create(this);

            if (AppSettings.CurrentUser.userType == Constants.USER_TYPE.ATHLETE)
                stackBuilder.AddNextIntentWithParentStack(new Intent(this, typeof(SwipeTabActivity)));
            else
                stackBuilder.AddNextIntentWithParentStack(new Intent(this, typeof(CoachHomeActivity)));

            stackBuilder.AddNextIntent(intent);

            var id = DateTime.Now.Millisecond;

            PendingIntent pendingIntent = stackBuilder.GetPendingIntent(id, PendingIntentFlags.UpdateCurrent);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var notificationBuilder = new NotificationCompat.Builder(this)
                                                            .SetContentTitle("Notification from " + mData["senderName"])
                                                            .SetContentText("Drop down to get detail")
                                                            .SetSmallIcon(Resource.Drawable.icon_remote_notification)
                                                            .SetStyle(textStyle)
                                                            .SetAutoCancel(true)
                                                            .SetSound(defaultSoundUri)
                                                            .SetContentIntent(pendingIntent);
            
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(id, notificationBuilder.Build());
        }
	}
}
