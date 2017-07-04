
using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.GrapeCity.Xuni.Core;
using goheja.Adapter;
using PortableLibrary;
using PortableLibrary.Model;
using UniversalImageLoader.Core;

namespace goheja
{
	[Activity(Label = "EventInstructionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EventInstructionActivity : BaseActivity
	{
        ScrollView scrollView;
        TextView btnEdit;
        TextView lblPlannedDistance, lblPlannedDuration, lblPlannedLoad, lblPlannedAvgSpeed, lblPlannedAcent, lblPlannedAvgHr, lblPlannedCalories, lblPlannedAvgPower, lblPlannedLeveledPower;
        EditText editPerformedDistance, editPerformedDuration, editPerformedLoad;
        TextView lblPerformedAvgSpeed, lblPerformedAcent, lblPerformedAvgHR, lblPerformedCalories, lblPerformedAvgPower, lblPerformedLeveledPower;

        ListView listLaps;
        TextView btnTotals, btnLaps;

		float fDistance = 0;
		float fDuration = 0;
		float fLoad = 0;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventInstructionActivity);

			var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
			ImageLoader.Instance.Init(config);

            LicenseManager.Key = License.Key;

			if (!IsNetEnable()) return;

            InitUISettings();
        }

        protected override void OnResume()
        {
            base.OnResume();

            ResetUISettings();
		}

        void ResetUISettings()
        {
            var selectedEventID = Intent.GetStringExtra("SelectedEventID");
			try
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

					var selectedEvent = GetEventDetail(selectedEventID);

					var reportData = GetEventReport(selectedEventID);
					var eventComment = GetComments(selectedEventID);

					AppSettings.selectedEvent = selectedEvent;
					AppSettings.selectedEvent._id = selectedEventID;
					AppSettings.currentEventReport = reportData;

					RunOnUiThread(() =>
					{
                        SetAdjustEnable(selectedEvent);
						InitBindingEventPlanned(selectedEvent);
						InitBindingEventReport(reportData);
						InitBindingEventComments(eventComment);
					});

					HideLoadingView();
				});
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
        }

		void InitUISettings()
		{
            scrollView = FindViewById<ScrollView>(Resource.Id.scrollView);

			var fromWhere = Intent.GetStringExtra("FromWhere");

			var currentUser = AppSettings.CurrentUser;
            if (!string.IsNullOrEmpty(fromWhere) && fromWhere.Equals("RemoteNotification")
                && currentUser.userType == (int)Constants.USER_TYPE.COACH)
            {

                currentUser.athleteId = Intent.GetStringExtra("senderId");
                AppSettings.isFakeUser = true;
                AppSettings.fakeUserName = Intent.GetStringExtra("senderName");

                AppSettings.CurrentUser = currentUser;
            }

			var txtFakeUserName = FindViewById<TextView>(Resource.Id.txtFakeUserName);
			txtFakeUserName.Visibility = AppSettings.isFakeUser ? ViewStates.Visible : ViewStates.Gone;
			txtFakeUserName.Text = AppSettings.fakeUserName;

            btnEdit = FindViewById<TextView>(Resource.Id.ActionEdit);
            btnEdit.SetTextColor(GROUP_COLOR);
            var imgEdit = FindViewById<ImageView>(Resource.Id.imgEdit);

            listLaps = FindViewById<ListView>(Resource.Id.listLaps);
            listLaps.SetOnTouchListener(new MyTouchListener());

            btnTotals = FindViewById<TextView>(Resource.Id.btnTotals);
            btnLaps = FindViewById<TextView>(Resource.Id.btnLaps);

            SetActiveTab(0);

            //plan
			lblPlannedDistance = FindViewById<TextView>(Resource.Id.lblPlannedDistance);
			lblPlannedDuration = FindViewById<TextView>(Resource.Id.lblPlannedDuration);
			lblPlannedLoad = FindViewById<TextView>(Resource.Id.lblPlannedLoad);
			lblPlannedAvgSpeed = FindViewById<TextView>(Resource.Id.lblPlannedAvgSpeed);
			lblPlannedAcent = FindViewById<TextView>(Resource.Id.lblPlannedAcent);
			lblPlannedAvgHr = FindViewById<TextView>(Resource.Id.lblPlannedAvgHr);
			lblPlannedCalories = FindViewById<TextView>(Resource.Id.lblPlannedCalories);
			lblPlannedAvgPower = FindViewById<TextView>(Resource.Id.lblPlannedAvgPower);
			lblPlannedLeveledPower = FindViewById<TextView>(Resource.Id.lblPlannedLeveledPower);

            //perform
            editPerformedDistance = FindViewById<EditText>(Resource.Id.editPerformedDistance);
			editPerformedDuration = FindViewById<EditText>(Resource.Id.editPerformedDuration);
			editPerformedLoad = FindViewById<EditText>(Resource.Id.editPerformedLoad);

            lblPerformedAvgSpeed = FindViewById<TextView>(Resource.Id.lblPerformedAvgSpeed);
            lblPerformedAcent = FindViewById<TextView>(Resource.Id.lblPerformedAcent);
            lblPerformedAvgHR = FindViewById<TextView>(Resource.Id.lblPerformedAvgHR);
            lblPerformedCalories = FindViewById<TextView>(Resource.Id.lblPerformedCalories);
            lblPerformedAvgPower = FindViewById<TextView>(Resource.Id.lblPerformedAvgPower);
            lblPerformedLeveledPower = FindViewById<TextView>(Resource.Id.lblPerformedLeveledPower);

            SetEditPerformField();

			btnEdit.Click += delegate (object sender, System.EventArgs e)
			{
                btnEdit.Text = btnEdit.Text == "Edit" ? "Done" : "Edit";
                imgEdit.SetImageResource(btnEdit.Text == "Edit" ? Resource.Drawable.icon_pencil : Resource.Drawable.icon_check);

                SetEditPerformField();

                if (btnEdit.Text == "Edit")
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

                        var authorID = AppSettings.CurrentUser.userId;

                        var pDuration = ConvertToFloat(editPerformedDuration.Text);
                        var pDistance = ConvertToFloat(editPerformedDistance.Text);
                        var pLoad = ConvertToFloat(editPerformedLoad.Text);

                        UpdateMemberNotes(string.Empty, authorID, AppSettings.selectedEvent._id, string.Empty, AppSettings.selectedEvent.attended, pDuration.ToString(), pDistance.ToString(), pLoad.ToString(), AppSettings.selectedEvent.type);

                        HideLoadingView();

                        ResetUISettings();
                    });
                }
			};

			btnTotals.Click += delegate (object sender, System.EventArgs e)
			{
                SetActiveTab(0);
			};
			btnLaps.Click += delegate (object sender, System.EventArgs e)
			{
				SetActiveTab(1);
			};

			FindViewById(Resource.Id.ActionLocation).Click += delegate (object sender, System.EventArgs e)
			{
				var activity = new Intent(this, typeof(LocationActivity));
				StartActivityForResult(activity, 1);
			};

			var ActionAdjustTrainning = FindViewById(Resource.Id.ActionAdjustTrainning);
			ActionAdjustTrainning.Click += delegate (object sender, System.EventArgs e)
			{
				var activity = new Intent(this, typeof(AdjustTrainningActivity));
				StartActivityForResult(activity, 1);
			};
			ActionAdjustTrainning.SetBackgroundColor(GROUP_COLOR);

			FindViewById(Resource.Id.ActionAddComment).Click += delegate (object sender, System.EventArgs e) { 
				var activity = new Intent(this, typeof(AddCommentActivity));
				StartActivityForResult(activity, 1);
			};
		}

		void SetAdjustEnable(GoHejaEvent selectedEvent)
		{
			if (DateTime.Compare(selectedEvent.StartDateTime(), DateTime.Now) > 0 || AppSettings.isFakeUser)
			{
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Gone;
				FindViewById(Resource.Id.contentEditBtn).Visibility = ViewStates.Gone;
			}
			else
			{
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Visible;
				FindViewById(Resource.Id.contentEditBtn).Visibility = ViewStates.Visible;
			}
		}

		void InitBindingEventPlanned(GoHejaEvent selectedEvent)
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

                lblPlannedDistance.Text = formattedDistance + " KM";

				var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
				var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
				var pHrs = durMin / 60;
				fDuration = (durHrs * 60 + durMin) * 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;
				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				fLoad = selectedEvent.tss == "" ? 0 : float.Parse(selectedEvent.tss);

                lblPlannedDuration.Text = strDuration;
                lblPlannedLoad.Text = selectedEvent.tss;
                lblPlannedAvgHr.Text = selectedEvent.hb;

				var imgType = FindViewById<ImageView>(Resource.Id.imgType);
                var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(selectedEvent.type));
				switch (pType)
				{
                    case Constants.EVENT_TYPE.OTHER:
						imgType.SetImageResource(Resource.Drawable.icon_other);
						break;
					case Constants.EVENT_TYPE.BIKE:
						imgType.SetImageResource(Resource.Drawable.icon_bike);
						break;
                    case Constants.EVENT_TYPE.RUN:
						imgType.SetImageResource(Resource.Drawable.icon_run);
						break;
                    case Constants.EVENT_TYPE.SWIM:
						imgType.SetImageResource(Resource.Drawable.icon_swim);
						break;
                    case Constants.EVENT_TYPE.TRIATHLON:
						imgType.SetImageResource(Resource.Drawable.icon_triathlon);
						break;
                    case Constants.EVENT_TYPE.ANOTHER:
						imgType.SetImageResource(Resource.Drawable.icon_other);
						break;
				}
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

        void InitBindingEventReport(ReportData report)
        {
            if (report != null)
			{
				if (report.data != null)
					InitBindingEventPerformed(report.data);

				if (report.lapData != null)
					InitBindingEventLaps(report.lapData, report.type);
			}
        }

        void InitBindingEventPerformed(List<Item> eventTotal)
		{
			try
			{
				lblPerformedAvgSpeed.Text = FormatNumber(eventTotal[0].value);
                editPerformedDistance.Text = FormatNumber(eventTotal[1].value);

                var strEt = GetFormatedDurationAsMin(eventTotal[2].value);
                editPerformedDuration.Text = strEt.ToString();

                lblPerformedAcent.Text = FormatNumber(eventTotal[3].value);
                lblPerformedAvgHR.Text = FormatNumber(eventTotal[4].value);
                lblPerformedCalories.Text = FormatNumber(eventTotal[5].value);
                lblPerformedAvgPower.Text = FormatNumber(eventTotal[6].value);
				editPerformedLoad.Text = FormatNumber(eventTotal[7].value);
                lblPerformedLeveledPower.Text = FormatNumber(eventTotal[8].value);

				CompareEventResult(fDistance, ConvertToFloat(eventTotal[1].value), lblPlannedDistance, editPerformedDistance);
				CompareEventResult(fDuration, TotalSecFromString(eventTotal[2].value), lblPlannedDuration, editPerformedDuration);
				CompareEventResult(fLoad, ConvertToFloat(eventTotal[7].value), lblPlannedLoad, editPerformedLoad);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		void InitBindingEventLaps(List<Lap> laps, int type)
		{
            var lapHeaderForBikeOrRun = FindViewById<LinearLayout>(Resource.Id.lapHeaderForBikeOrRun);
            var lapHeaderForSwim = FindViewById<LinearLayout>(Resource.Id.lapHeaderForSwim);
            var lapHeaderForTriathlon = FindViewById<LinearLayout>(Resource.Id.lapHeaderForTriathlon);
            var lapHeaderForOther = FindViewById<LinearLayout>(Resource.Id.lapHeaderForOther);

            lapHeaderForBikeOrRun.Visibility = ViewStates.Gone;
            lapHeaderForSwim.Visibility = ViewStates.Gone;
            lapHeaderForTriathlon.Visibility = ViewStates.Gone;
            lapHeaderForOther.Visibility = ViewStates.Gone;

			var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), type);
			switch (pType)
			{
				case Constants.EVENT_TYPE.BIKE:
				case Constants.EVENT_TYPE.RUN:
                    lapHeaderForBikeOrRun.Visibility = ViewStates.Visible;
					break;
				case Constants.EVENT_TYPE.SWIM:
					lapHeaderForSwim.Visibility = ViewStates.Visible;
					break;
				case Constants.EVENT_TYPE.TRIATHLON:
					lapHeaderForTriathlon.Visibility = ViewStates.Visible;
					break;
				case Constants.EVENT_TYPE.ANOTHER:
				case Constants.EVENT_TYPE.OTHER:
					lapHeaderForOther.Visibility = ViewStates.Visible;
					break;
			}

			var adapter = new LapsAdapter(laps, type, this);
			listLaps.Adapter = adapter;
			adapter.NotifyDataSetChanged();
		}

		void InitBindingEventComments(Comments comments)
		{
			var contentComment = FindViewById<LinearLayout>(Resource.Id.contentComment);
			contentComment.RemoveAllViews();

			if (comments == null || comments.comments.Count == 0) return;

			var commentId = Intent.GetStringExtra("commentId");

			try
			{
				var commentTitle = comments.comments.Count > 1 ? "COMMENTS" + " (" + comments.comments.Count + ")" : "COMMENT" + " (" + comments.comments.Count + ")";
				FindViewById<TextView>(Resource.Id.lblCommentTitle).Text = commentTitle;

                foreach (var comment in comments.comments)
                {
                    var commentView = LayoutInflater.From(this).Inflate(Resource.Layout.item_Comment, null);

                    var deltaSecs = float.Parse(comment.date) / 1000;
                    var commentDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(deltaSecs).ToLocalTime();

					commentView.FindViewById<TextView>(Resource.Id.lblAuthorName).Text = comment.author;
					commentView.FindViewById<TextView>(Resource.Id.lblCommentDate).Text = String.Format("{0:t}", commentDate) + " | " + String.Format("{0:d}", commentDate);
					commentView.FindViewById<TextView>(Resource.Id.lblComment).Text = comment.commentText;
                    commentView.FindViewById<TextView>(Resource.Id.lblCommentDate).SetTextColor(Color.White);
                    commentView.FindViewById<ImageView>(Resource.Id.imgNewSymbol).Visibility = ViewStates.Gone;
					if (!string.IsNullOrEmpty(comment.authorUrl))
					{
						var imgIcon = commentView.FindViewById<ImageView>(Resource.Id.imgProfile);
						ImageLoader imageLoader = ImageLoader.Instance;
						imageLoader.DisplayImage(comment.authorUrl, imgIcon);
					}

					contentComment.AddView(commentView);

                    if (!string.IsNullOrEmpty(commentId) && commentId.Equals(comment.commentId))
                    {
                        commentView.FindViewById<TextView>(Resource.Id.lblCommentDate).SetTextColor(Color.ParseColor("#" + Constants.COLOR_NEW_NOTIFICATION));
                        commentView.FindViewById<ImageView>(Resource.Id.imgNewSymbol).Visibility = ViewStates.Visible;
						commentView.FindViewById<LinearLayout>(Resource.Id.footerView).RequestFocus();

                        Intent.RemoveExtra("commentId");
                    }
				}
			}
			catch (Exception ex)
			{
				//ShowTrackMessageBox(ex.Message);
			}
		}

        void SetEditPerformField()
        {
			var isEditable = !(btnEdit.Text == "Edit");
			editPerformedDistance.Enabled = isEditable;
			editPerformedDuration.Enabled = isEditable;
			editPerformedLoad.Enabled = isEditable;

            var bgColor = isEditable ? Color.Gray : Color.Transparent;
			editPerformedDistance.SetBackgroundColor(bgColor);
            editPerformedDuration.SetBackgroundColor(bgColor);
            editPerformedLoad.SetBackgroundColor(bgColor);
		}

        void SetActiveTab(int tabType)
        {
            if(tabType == 0)
            {
				btnTotals.SetTextColor(GROUP_COLOR);
				btnLaps.SetTextColor(Color.White);
				FindViewById<LinearLayout>(Resource.Id.contentTotals).Visibility = ViewStates.Visible;
				FindViewById<LinearLayout>(Resource.Id.tabTotalsBorder).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.contentLaps).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.tabLapsBorder).Visibility = ViewStates.Visible;
            }
            else
            {
				btnTotals.SetTextColor(Color.White);
				btnLaps.SetTextColor(GROUP_COLOR);
				FindViewById<LinearLayout>(Resource.Id.contentTotals).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.tabTotalsBorder).Visibility = ViewStates.Visible;
				FindViewById<LinearLayout>(Resource.Id.contentLaps).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.tabLapsBorder).Visibility = ViewStates.Gone;
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

        //enable scrolling of listview on scrollview
		public class MyTouchListener: Java.Lang.Object, View.IOnTouchListener
		{
            public bool OnTouch(View v, MotionEvent e)
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        v.Parent.RequestDisallowInterceptTouchEvent(true);
                        break;
                    case MotionEventActions.Up:
                        v.Parent.RequestDisallowInterceptTouchEvent(false);
                        break;
                }

                v.OnTouchEvent(e);
                return true;
            }
		}
	}
}
