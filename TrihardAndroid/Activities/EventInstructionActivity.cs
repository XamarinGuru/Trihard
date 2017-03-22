
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventInstructionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EventInstructionActivity : BaseActivity
	{
		TextView lblPDistance, lblPDuration, lblPLoad, lblPHB;
		TextView lblTDistance, lblTDuration, lblTload;

		float fDistance = 0;
		float fDuration = 0;
		float fLoad = 0;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventInstructionActivity);

			var selectedEvent = AppSettings.selectedEvent;

			InitUISettings(selectedEvent);

			if (!IsNetEnable()) return;

			InitBindingEventData(selectedEvent);
		}

		protected override void OnResume()
		{
			base.OnResume();

			var selectedEvent = AppSettings.selectedEvent;

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				var eventDetail = GetEventDetail(selectedEvent._id);
				var eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				AppSettings.selectedEvent = eventDetail;
				AppSettings.selectedEvent._id = selectedEvent._id;
				AppSettings.currentEventTotal = eventTotal;

				RunOnUiThread(() =>
				{
					InitBindingEventData(eventDetail);
					InitBindingEventTotal(eventTotal);
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitUISettings(GoHejaEvent selectedEvent)
		{
			lblPDistance = FindViewById<TextView>(Resource.Id.lblPDistance);
			lblPDuration = FindViewById<TextView>(Resource.Id.lblPDuration);
			lblPLoad = FindViewById<TextView>(Resource.Id.lblPLoad);
			lblPHB = FindViewById<TextView>(Resource.Id.lblPHB);

			lblPDistance.SetTextColor(GROUP_COLOR);
			lblPDuration.SetTextColor(GROUP_COLOR);
			lblPLoad.SetTextColor(GROUP_COLOR);
			lblPHB.SetTextColor(GROUP_COLOR);

			FindViewById<TextView>(Resource.Id.lblAvgSpeed).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblTotalDistance).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblElapsedTime).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblTotalAcent).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblAvgHR).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblTotalCalories).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblAvgPower).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblLoad).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.lblLeveledPower).SetTextColor(GROUP_COLOR);

			lblTDistance = FindViewById<TextView>(Resource.Id.lblTotalDistance);
			lblTDuration = FindViewById<TextView>(Resource.Id.lblElapsedTime);
			lblTload = FindViewById<TextView>(Resource.Id.lblLoad);

			FindViewById(Resource.Id.btnBack).Click += delegate (object sender, EventArgs e) {
				var activity = new Intent();
				SetResult(Result.Canceled, activity);
				Finish();
			};
			FindViewById(Resource.Id.ActionLocation).Click += delegate (object sender, EventArgs e)
			{
				var activity = new Intent(this, typeof(LocationActivity));
				StartActivityForResult(activity, 1);
			};
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += delegate (object sender, EventArgs e)
			{
				var activity = new Intent(this, typeof(AdjustTrainningActivity));
				StartActivityForResult(activity, 1);
			};
			FindViewById(Resource.Id.ActionAdjustTrainning).SetBackgroundColor(GROUP_COLOR);

			FindViewById(Resource.Id.ActionAddComment).Click += delegate (object sender, EventArgs e) { 
				var activity = new Intent(this, typeof(AddCommentActivity));
				StartActivityForResult(activity, 1);
			};
			FindViewById(Resource.Id.ActionAddComment).SetBackgroundColor(GROUP_COLOR);

			if (DateTime.Compare(selectedEvent.StartDateTime(), DateTime.Now) > 0)
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Gone;
			else
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Visible;
		}

		void InitBindingEventData(GoHejaEvent selectedEvent)
		{
			try
			{
				var startDateFormats = String.Format("{0:f}", selectedEvent.StartDateTime());//selectedEvent.StartDateTime().GetDateTimeFormats();
				FindViewById<TextView>(Resource.Id.lblTitle).Text = selectedEvent.title;
				FindViewById<TextView>(Resource.Id.lblStartDate).Text = startDateFormats;
				FindViewById<TextView>(Resource.Id.lblData).Text = selectedEvent.eventData;

				var strDistance = selectedEvent.distance;
				fDistance = strDistance == "" || strDistance == null ? 0 : float.Parse(strDistance);
				var b = Math.Truncate(fDistance * 100);
				var c = b / 100;
				var formattedDistance = c.ToString("F2");

				lblPDistance.Text = formattedDistance + " KM";

				var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
				var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
				var pHrs = durMin / 60;
				fDuration = (durHrs * 60 + durMin) * 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;
				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				fLoad = selectedEvent.tss == "" ? 0 : float.Parse(selectedEvent.tss);

				lblPDuration.Text = strDuration;
				lblPLoad.Text = selectedEvent.tss;
				lblPHB.Text = selectedEvent.hb;

				var imgType = FindViewById<ImageView>(Resource.Id.imgType);
				switch (selectedEvent.type)
				{
					case "0":
						imgType.SetImageResource(Resource.Drawable.icon_triathlon);
						break;
					case "1":
						imgType.SetImageResource(Resource.Drawable.icon_bike);
						break;
					case "2":
						imgType.SetImageResource(Resource.Drawable.icon_run);
						break;
					case "3":
						imgType.SetImageResource(Resource.Drawable.icon_swim);
						break;
					case "4":
						imgType.SetImageResource(Resource.Drawable.icon_triathlon);
						break;
					case "5":
						imgType.SetImageResource(Resource.Drawable.icon_other);
						break;
				}
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		void InitBindingEventTotal(EventTotal eventTotal)
		{
			if (eventTotal == null || eventTotal.totals == null)
			{
				FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Gone;
				return;
			}

			FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Visible;

			try
			{
				FindViewById<TextView>(Resource.Id.lblAvgSpeed).Text = FormatNumber(eventTotal.totals[0].value);
				FindViewById<TextView>(Resource.Id.lblTotalDistance).Text = FormatNumber(eventTotal.totals[1].value);
				FindViewById<TextView>(Resource.Id.lblElapsedTime).Text = FormatNumber(eventTotal.totals[2].value);
				FindViewById<TextView>(Resource.Id.lblTotalAcent).Text = FormatNumber(eventTotal.totals[3].value);
				FindViewById<TextView>(Resource.Id.lblAvgHR).Text = FormatNumber(eventTotal.totals[4].value);
				FindViewById<TextView>(Resource.Id.lblTotalCalories).Text = FormatNumber(eventTotal.totals[5].value);
				FindViewById<TextView>(Resource.Id.lblAvgPower).Text = FormatNumber(eventTotal.totals[6].value);
				FindViewById<TextView>(Resource.Id.lblLoad).Text = FormatNumber(eventTotal.totals[7].value);
				FindViewById<TextView>(Resource.Id.lblLeveledPower).Text = FormatNumber(eventTotal.totals[8].value);

				CompareEventResult(fDistance, ConvertToFloat(eventTotal.totals[1].value), lblPDistance, lblTDistance);
				CompareEventResult(fDuration, TotalSecFromString(eventTotal.totals[2].value), lblPDuration, lblTDuration);
				CompareEventResult(fLoad, ConvertToFloat(eventTotal.totals[7].value), lblPLoad, lblTload);
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		void InitBindingEventComments(Comment comments)
		{
			if (comments == null) return;

			try
			{
				var contentComment = FindViewById<LinearLayout>(Resource.Id.contentComment);
				contentComment.RemoveAllViews();
				foreach (var comment in comments.comments)
				{
					var commentView = LayoutInflater.From(this).Inflate(Resource.Layout.item_Comment, null);

					var deltaSecs = float.Parse(comment.date) / 1000;
					var commentDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(deltaSecs).ToLocalTime();

					FindViewById<TextView>(Resource.Id.lblCommentTitle).Text = "COMMENT" + " (" + comments.comments.Count + ")";
					commentView.FindViewById<TextView>(Resource.Id.lblAuthorName).Text = comment.author;
					commentView.FindViewById<TextView>(Resource.Id.lblCommentDate).Text = String.Format("{0:t}", commentDate) + " | " + String.Format("{0:d}", commentDate);
					commentView.FindViewById<TextView>(Resource.Id.lblComment).Text = comment.commentText;
					contentComment.AddView(commentView);
				}
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				ActionBackCancel();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
