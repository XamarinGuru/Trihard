
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ActionAdjustTrainning", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AdjustTrainningActivity : BaseActivity
	{
		RootMemberModel MemberModel = new RootMemberModel();

		TextView lblTime, lblDistance, lblTSS;
		EditText txtComment;
		SeekBar seekTime, seekDistance, seekTSS;
		CheckBox attended;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AdjustTrainningActivity);

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			InitBindingEventTotal();
		}

		protected override void OnResume()
		{
			base.OnResume();
			SetPType();
		}

		void InitUISettings()
		{
			SetPType();

			FindViewById<LinearLayout>(Resource.Id.bgType).SetBackgroundColor(GROUP_COLOR);

			attended = FindViewById<CheckBox>(Resource.Id.checkAttended);

			lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);

			seekTime = FindViewById<SeekBar>(Resource.Id.ActionTimeChanged);
			seekDistance = FindViewById<SeekBar>(Resource.Id.ActionDistanceChanged);
			seekTSS = FindViewById<SeekBar>(Resource.Id.ActionTSSChanged);

			seekTime.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);
			seekDistance.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);
			seekTSS.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);

			seekTime.ProgressChanged += (sender, e) => { lblTime.Text = ((SeekBar)sender).Progress.ToString(); };
			seekDistance.ProgressChanged += (sender, e) => { lblDistance.Text = ((SeekBar)sender).Progress.ToString(); };
			seekTSS.ProgressChanged += (sender, e) => { lblTSS.Text = ((SeekBar)sender).Progress.ToString(); };

			FindViewById(Resource.Id.ActionSwitchType).Click += delegate (object sender, EventArgs e)
			{
				var activity = new Intent(this, typeof(SelectPTypeActivity));
				StartActivityForResult(activity, 1);
			};
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += ActionAdjustTrainning;
			FindViewById(Resource.Id.ActionAdjustTrainning).SetBackgroundColor(GROUP_COLOR);

			try
			{
				SetupAdjustPicker(lblTime, seekTime, 360);
				SetupAdjustPicker(lblDistance, seekDistance, 250);
				SetupAdjustPicker(lblTSS, seekTSS, 400);
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}
		void SetPType()
		{
			var imgType = FindViewById<ImageView>(Resource.Id.imgType);
			var strType = FindViewById<TextView>(Resource.Id.strType);

			strType.Text = Constants.PRACTICE_TYPES[int.Parse(AppSettings.selectedEvent.type) - 1];

			switch (AppSettings.selectedEvent.type)
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
		void SetupAdjustPicker(TextView textView, SeekBar seekBar, int maxValue)
		{
			textView.Touch += (object sender, View.TouchEventArgs e) =>
			{
				if (e.Event.Action == MotionEventActions.Down)
				{
					AdjustDialog myDiag = AdjustDialog.newInstance((TextView)sender, seekBar, maxValue);
					myDiag.Show(FragmentManager, "Diag");
				}
			};
		}

		void InitBindingEventTotal()
		{
			try
			{
				var eventTotal = AppSettings.currentEventTotal;

				attended.Checked = AppSettings.selectedEvent.attended == "1" ? true : false;

				if (eventTotal == null || eventTotal.totals == null) return;

				var strEt = GetFormatedDurationAsMin(eventTotal.GetValue(Constants.TOTALS_ES_TIME));
				var strTd = eventTotal.GetValue(Constants.TOTALS_DISTANCE);
				var strTss = eventTotal.GetValue(Constants.TOTALS_LOAD);

				lblTime.Text = strEt.ToString();
				lblDistance.Text = float.Parse(strTd).ToString("F0");
				lblTSS.Text = float.Parse(strTss).ToString("F0");

				seekTime.Progress = strEt;
				seekDistance.Progress = (int)float.Parse(strTd);
				seekTSS.Progress = (int)float.Parse(strTss);
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		void ActionAdjustTrainning(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

				UpdateMemberNotes(txtComment.Text, AppSettings.UserID, AppSettings.selectedEvent._id, MemberModel.username, attended.Checked ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, AppSettings.selectedEvent.type);

				HideLoadingView();

				ActionBackCancel();
			});
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
