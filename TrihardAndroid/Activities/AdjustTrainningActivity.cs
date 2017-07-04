
using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ActionAdjustTrainning", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AdjustTrainningActivity : BaseActivity
	{
		RootMemberModel MemberModel = new RootMemberModel();

        Constants.EVENT_TYPE _pType;

		TextView lblTime, lblDistance, lblTSS;
		SeekBar seekTime, seekDistance, seekTSS;
		CheckBox attended;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AdjustTrainningActivity);

			InitUISettings();

			if (!IsNetEnable()) return;

			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});
		}

		protected override void OnResume()
		{
			base.OnResume();
			SetPType();
		}

		void InitUISettings()
		{
			FindViewById<LinearLayout>(Resource.Id.bgType).SetBackgroundColor(GROUP_COLOR);

			attended = FindViewById<CheckBox>(Resource.Id.checkAttended);

			lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);

			seekTime = FindViewById<SeekBar>(Resource.Id.ActionTimeChanged);
			seekDistance = FindViewById<SeekBar>(Resource.Id.ActionDistanceChanged);
			seekTSS = FindViewById<SeekBar>(Resource.Id.ActionTSSChanged);

			seekTime.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);
			seekDistance.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);
			seekTSS.ProgressDrawable.SetColorFilter(GROUP_COLOR, Android.Graphics.PorterDuff.Mode.Multiply);

			seekTime.ProgressChanged += (sender, e) =>
			{
				lblTime.Text = (((SeekBar)sender).Progress / 10).ToString();
			};
			seekDistance.ProgressChanged += (sender, e) =>
			{
				lblDistance.Text = (((SeekBar)sender).Progress / 10.0f).ToString();
			};
			seekTSS.ProgressChanged += (sender, e) =>
			{
				lblTSS.Text = (((SeekBar)sender).Progress / 10.0f).ToString();
			};

			FindViewById(Resource.Id.ActionSwitchType).Click += delegate (object sender, EventArgs e)
			{
				var activity = new Intent(this, typeof(SelectPTypeActivity));
				StartActivityForResult(activity, 1);
			};
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += ActionAdjustTrainning;
			FindViewById(Resource.Id.ActionAdjustTrainning).SetBackgroundColor(GROUP_COLOR);

            SetupAdjustPicker(lblTime, seekTime, 360);
            SetupAdjustPicker(lblTSS, seekTSS, 400);
		}
		void SetPType()
		{
			var imgType = FindViewById<ImageView>(Resource.Id.imgType);
			var strType = FindViewById<TextView>(Resource.Id.strType);

			strType.Text = Constants.PRACTICE_TYPES[int.Parse(AppSettings.selectedEvent.type) - 1];

			_pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(AppSettings.selectedEvent.type));

			switch (_pType)
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

			InitBindingEventTotal();
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

		void SetupDistanceAdjustPicker(TextView textView, SeekBar seekBar, int maxValue)
		{
			textView.Touch -= SetDistanceAdjustDialog;
			textView.Touch += SetDistanceAdjustDialog;
		}

		void SetDistanceAdjustDialog(object sender, View.TouchEventArgs e)
		{
			if (e.Event.Action == MotionEventActions.Down)
			{
                AdjustDialog myDiag = AdjustDialog.newInstance((TextView)sender, seekDistance, _pType == Constants.EVENT_TYPE.SWIM ? 10 : 250);
				myDiag.Show(FragmentManager, "Diag");
			}
		}

		void InitBindingEventTotal()
		{
			try
			{
                var reportData = AppSettings.currentEventReport;

				SetupDistanceAdjustPicker(lblDistance, seekDistance, _pType == Constants.EVENT_TYPE.SWIM ? 10 : 250);

				attended.Checked = AppSettings.selectedEvent.attended == "1" ? true : false;

				seekDistance.Max = _pType == Constants.EVENT_TYPE.SWIM ? 100 : 2500;

                if (reportData == null || reportData.data == null) return;

                var strEt = GetFormatedDurationAsMin(reportData.GetTotalValue(Constants.TOTALS_ES_TIME));
				var strTd = reportData.GetTotalValue(Constants.TOTALS_DISTANCE);
				var strTss = reportData.GetTotalValue(Constants.TOTALS_LOAD);

				lblTime.Text = strEt.ToString();
				lblTSS.Text = float.Parse(strTss).ToString("F1");

				seekTime.Progress = strEt * 10;
				seekTSS.Progress = (int)(float.Parse(strTss) * 10);

				var valDistance = float.Parse(strTd);
				if (_pType == Constants.EVENT_TYPE.SWIM)
				{
					if (valDistance > 10)
					{
						lblDistance.Text = "10";
						seekDistance.Progress = 100;
					}
					else
					{
						lblDistance.Text = valDistance.ToString("F1");
						seekDistance.Progress = int.Parse((valDistance * 10).ToString("F0"));
					}
				}
				else
				{
					lblDistance.Text = valDistance.ToString("F1");
					seekDistance.Progress = int.Parse((valDistance * 10).ToString("F0"));
				}
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		void ActionAdjustTrainning(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

				var authorID = AppSettings.CurrentUser.userId;

                UpdateMemberNotes(string.Empty, authorID, AppSettings.selectedEvent._id, MemberModel.username, attended.Checked ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, AppSettings.selectedEvent.type);

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
