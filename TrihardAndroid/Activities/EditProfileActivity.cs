
using System;
using System.Collections.Generic;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EditProfileActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EditProfileActivity : BaseActivity
	{
		RootMemberModel MemberModel { get; set; }

		TextView lblFirstname, lblLastname, lblCountry, lblAddress, lblBib, lblAge, lblGender, lblBirth, lblEmail, lblPhone;
		EditText txtWeight, txtHeight, txtBMI, txtFatPercentage;
		EditText txtGoalDate;
		EditText txtGoalName, txtGoalLoad;
		EditText txtSprint, txtOlympic, txtHDistance, txtDistance, txtMarathon, txtHMarathon, txt10KRun;
		EditText txtRankSwim, txtRankRun, txtRankBike;
		EditText txtSZone1HR, txtSZone2HR, txtSZone3HR, txtSZone4HR, txtSZone5HR;
		EditText txtSZone1PACE, txtSZone2PACE, txtSZone3PACE, txtSZone4PACE, txtSZone5PACE;
		EditText txtSFTPace, txtSFTPHB;
		EditText txtRZone1HR, txtRZone2HR, txtRZone3HR, txtRZone4HR, txtRZone5HR;
		EditText txtRZone1PACE, txtRZone2PACE, txtRZone3PACE, txtRZone4PACE, txtRZone5PACE;
		EditText txtRZone1Power, txtRZone2Power, txtRZone3Power, txtRZone4Power, txtRZone5Power;
		EditText txtRFTPace, txtRFTPHB, txtRFTPower;
		EditText txtBZone1HR, txtBZone2HR, txtBZone3HR, txtBZone4HR, txtBZone5HR;
		EditText txtBZone1POWER, txtBZone2POWER, txtBZone3POWER, txtBZone4POWER, txtBZone5POWER;
		EditText txtBFTPHB, txtBFTPower;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EditProfileActivity);

			Window.SetBackgroundDrawableResource(Resource.Drawable.bg_new);

			MemberModel = new RootMemberModel();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_USER_DATA);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();

				RunOnUiThread(() =>
				{
					SetInputBinding();
				});
			});

			SetUIVariablesAndActions();
			SetInputValidation();
		}

		private void SetUIVariablesAndActions()
		{
			#region UI Variables
			lblFirstname = FindViewById<TextView>(Resource.Id.lblFirstname);
			lblLastname = FindViewById<TextView>(Resource.Id.lblLastname);
			lblCountry = FindViewById<TextView>(Resource.Id.lblCountry);
			lblAddress = FindViewById<TextView>(Resource.Id.lblAddress);
			lblBib = FindViewById<TextView>(Resource.Id.lblBib);
			lblAge = FindViewById<TextView>(Resource.Id.lblAge);
			lblGender = FindViewById<TextView>(Resource.Id.lblGender);
			lblBirth = FindViewById<TextView>(Resource.Id.lblBirth);
			lblEmail = FindViewById<TextView>(Resource.Id.lblEmail);
			lblPhone = FindViewById<TextView>(Resource.Id.lblPhone);

			lblFirstname.SetTextColor(GROUP_COLOR);
			lblLastname.SetTextColor(GROUP_COLOR);
			lblCountry.SetTextColor(GROUP_COLOR);
			lblAddress.SetTextColor(GROUP_COLOR);
			lblBib.SetTextColor(GROUP_COLOR);
			lblAge.SetTextColor(GROUP_COLOR);
			lblGender.SetTextColor(GROUP_COLOR);
			lblBirth.SetTextColor(GROUP_COLOR);
			lblEmail.SetTextColor(GROUP_COLOR);
			lblPhone.SetTextColor(GROUP_COLOR);

			txtWeight = FindViewById<EditText>(Resource.Id.txtWeight);
			txtHeight = FindViewById<EditText>(Resource.Id.txtHeight);
			txtBMI = FindViewById<EditText>(Resource.Id.txtBMI);
			txtFatPercentage = FindViewById<EditText>(Resource.Id.txtFatPercentage);

			txtGoalDate = FindViewById<EditText>(Resource.Id.txtGoalDate);
			txtGoalDate.Focusable = false;
			txtGoalName = FindViewById<EditText>(Resource.Id.txtGoalName);
			txtGoalLoad = FindViewById<EditText>(Resource.Id.txtGoalLoad);

			txtSprint = FindViewById<EditText>(Resource.Id.txtSprint);
			txtOlympic = FindViewById<EditText>(Resource.Id.txtOlympic);
			txtHDistance = FindViewById<EditText>(Resource.Id.txtHDistance);
			txtDistance = FindViewById<EditText>(Resource.Id.txtDistance);
			txtMarathon = FindViewById<EditText>(Resource.Id.txtMarathon);
			txtHMarathon = FindViewById<EditText>(Resource.Id.txtHMarathon);
			txt10KRun = FindViewById<EditText>(Resource.Id.txt10KRun);

			txtRankSwim = FindViewById<EditText>(Resource.Id.txtRankSwim);
			txtRankRun = FindViewById<EditText>(Resource.Id.txtRankRun);
			txtRankBike = FindViewById<EditText>(Resource.Id.txtRankBike);

			txtSZone1HR = FindViewById<EditText>(Resource.Id.txtSZone1HR);
			txtSZone2HR = FindViewById<EditText>(Resource.Id.txtSZone2HR);
			txtSZone3HR = FindViewById<EditText>(Resource.Id.txtSZone3HR);
			txtSZone4HR = FindViewById<EditText>(Resource.Id.txtSZone4HR);
			txtSZone5HR = FindViewById<EditText>(Resource.Id.txtSZone5HR);

			txtSZone1PACE = FindViewById<EditText>(Resource.Id.txtSZone1PACE);
			txtSZone2PACE = FindViewById<EditText>(Resource.Id.txtSZone2PACE);
			txtSZone3PACE = FindViewById<EditText>(Resource.Id.txtSZone3PACE);
			txtSZone4PACE = FindViewById<EditText>(Resource.Id.txtSZone4PACE);
			txtSZone5PACE = FindViewById<EditText>(Resource.Id.txtSZone5PACE);

			txtSFTPace = FindViewById<EditText>(Resource.Id.txtSFTPace);
			txtSFTPHB = FindViewById<EditText>(Resource.Id.txtSFTPHB);

			txtRZone1HR = FindViewById<EditText>(Resource.Id.txtRZone1HR);
			txtRZone2HR = FindViewById<EditText>(Resource.Id.txtRZone2HR);
			txtRZone3HR = FindViewById<EditText>(Resource.Id.txtRZone3HR);
			txtRZone4HR = FindViewById<EditText>(Resource.Id.txtRZone4HR);
			txtRZone5HR = FindViewById<EditText>(Resource.Id.txtRZone5HR);

			txtRZone1PACE = FindViewById<EditText>(Resource.Id.txtRZone1PACE);
			txtRZone2PACE = FindViewById<EditText>(Resource.Id.txtRZone2PACE);
			txtRZone3PACE = FindViewById<EditText>(Resource.Id.txtRZone3PACE);
			txtRZone4PACE = FindViewById<EditText>(Resource.Id.txtRZone4PACE);
			txtRZone5PACE = FindViewById<EditText>(Resource.Id.txtRZone5PACE);

			txtRZone1Power = FindViewById<EditText>(Resource.Id.txtRZone1Power);
			txtRZone2Power = FindViewById<EditText>(Resource.Id.txtRZone2Power);
			txtRZone3Power = FindViewById<EditText>(Resource.Id.txtRZone3Power);
			txtRZone4Power = FindViewById<EditText>(Resource.Id.txtRZone4Power);
			txtRZone5Power = FindViewById<EditText>(Resource.Id.txtRZone5Power);

			txtRFTPace = FindViewById<EditText>(Resource.Id.txtRFTPace);
			txtRFTPHB = FindViewById<EditText>(Resource.Id.txtRFTPHB);
			txtRFTPower = FindViewById<EditText>(Resource.Id.txtRFTPower);

			txtBZone1HR = FindViewById<EditText>(Resource.Id.txtBZone1HR);
			txtBZone2HR = FindViewById<EditText>(Resource.Id.txtBZone2HR);
			txtBZone3HR = FindViewById<EditText>(Resource.Id.txtBZone3HR);
			txtBZone4HR = FindViewById<EditText>(Resource.Id.txtBZone4HR);
			txtBZone5HR = FindViewById<EditText>(Resource.Id.txtBZone5HR);

			txtBZone1POWER = FindViewById<EditText>(Resource.Id.txtBZone1POWER);
			txtBZone2POWER = FindViewById<EditText>(Resource.Id.txtBZone2POWER);
			txtBZone3POWER = FindViewById<EditText>(Resource.Id.txtBZone3POWER);
			txtBZone4POWER = FindViewById<EditText>(Resource.Id.txtBZone4POWER);
			txtBZone5POWER = FindViewById<EditText>(Resource.Id.txtBZone5POWER);

			txtBFTPHB = FindViewById<EditText>(Resource.Id.txtBFTPHB);
			txtBFTPower = FindViewById<EditText>(Resource.Id.txtBFTPower);
			#endregion

			#region Actions

			txtGoalDate.Touch += SetupDatePicker;

			FindViewById<RelativeLayout>(Resource.Id.collapsePhysical).Click += ActionCollepse;
			FindViewById<RelativeLayout>(Resource.Id.collapseGoals).Click += ActionCollepse;
			FindViewById<RelativeLayout>(Resource.Id.collapseBestResults).Click += ActionCollepse;
			FindViewById<RelativeLayout>(Resource.Id.collapseSelfRanking).Click += ActionCollepse;

			FindViewById<TextView>(Resource.Id.edtPhysical).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtGoals).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtBestResults).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtSeflRanking).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtSwim).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtRun).Click += ActionEdit;
			FindViewById<TextView>(Resource.Id.edtBike).Click += ActionEdit;

			FindViewById<TextView>(Resource.Id.edtPhysical).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtGoals).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtBestResults).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtSeflRanking).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtSwim).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtRun).SetTextColor(GROUP_COLOR);
			FindViewById<TextView>(Resource.Id.edtBike).SetTextColor(GROUP_COLOR);

			FindViewById<Button>(Resource.Id.btnUpdate).Click += ActionUpdate;
			FindViewById<Button>(Resource.Id.btnUpdate).SetBackgroundColor(GROUP_COLOR);
			#endregion

			var contentView = FindViewById<LinearLayout>(Resource.Id.contentView);
			var childs = GetAllChildren(contentView);
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i] is EditText)
					((EditText)childs[i]).TextChanged += (s, e) => { };
			}
		}
		List<View> GetAllChildren(View view)
		{
			if (!(view is ViewGroup))
			{
				List<View> viewArrayList = new List<View>();
				viewArrayList.Add(view);
				return viewArrayList;
			}

			List<View> result = new List<View>();

			ViewGroup vg = (ViewGroup)view;
			for (int i = 0; i < vg.ChildCount; i++)
			{
				View child = vg.GetChildAt(i);
				List<View> viewArrayList = new List<View>();
				viewArrayList.Add(view);
				viewArrayList.AddRange(GetAllChildren(child));
				result.AddRange(viewArrayList);
			}
			return result;
		}
		private void SetInputValidation()
		{
			try
			{
				SetupPicker(txtSprint, Constants.PICKER_TIME);
				SetupPicker(txtOlympic, Constants.PICKER_TIME);
				SetupPicker(txtHDistance, Constants.PICKER_TIME);
				SetupPicker(txtDistance, Constants.PICKER_TIME);
				SetupPicker(txtMarathon, Constants.PICKER_TIME);
				SetupPicker(txtHMarathon, Constants.PICKER_TIME);
				SetupPicker(txt10KRun, Constants.PICKER_TIME);

				SetupPicker(txtRankSwim, Constants.PICKER_RANKING);
				SetupPicker(txtRankRun, Constants.PICKER_RANKING);
				SetupPicker(txtRankBike, Constants.PICKER_RANKING);

				SetupPicker(txtSZone1HR, Constants.PICKER_HR);
				SetupPicker(txtSZone2HR, Constants.PICKER_HR);
				SetupPicker(txtSZone3HR, Constants.PICKER_HR);
				SetupPicker(txtSZone4HR, Constants.PICKER_HR);
				SetupPicker(txtSZone5HR, Constants.PICKER_HR);

				SetupPicker(txtRZone1HR, Constants.PICKER_HR);
				SetupPicker(txtRZone2HR, Constants.PICKER_HR);
				SetupPicker(txtRZone3HR, Constants.PICKER_HR);
				SetupPicker(txtRZone4HR, Constants.PICKER_HR);
				SetupPicker(txtRZone5HR, Constants.PICKER_HR);

				SetupPicker(txtBZone1HR, Constants.PICKER_HR);
				SetupPicker(txtBZone2HR, Constants.PICKER_HR);
				SetupPicker(txtBZone3HR, Constants.PICKER_HR);
				SetupPicker(txtBZone4HR, Constants.PICKER_HR);
				SetupPicker(txtBZone5HR, Constants.PICKER_HR);

				SetupPicker(txtSZone1PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtSZone2PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtSZone3PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtSZone4PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtSZone5PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRZone1PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRZone2PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRZone3PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRZone4PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRZone5PACE, Constants.PICKER_PACE, Constants.UNIT_SWIM);

				SetupPicker(txtSFTPace, Constants.PICKER_PACE, Constants.UNIT_SWIM);
				SetupPicker(txtRFTPace, Constants.PICKER_PACE, Constants.UNIT_RUN);
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		private void SetInputBinding()
		{
			try
			{
				#region physical
				this.SetBinding(() => MemberModel.firstname, () => lblFirstname.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.lastname, () => lblLastname.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.country, () => lblCountry.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.address, () => lblAddress.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.bib, () => lblBib.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.age, () => lblAge.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.gender, () => lblGender.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.birth, () => lblBirth.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.email, () => lblEmail.Text, BindingMode.OneWay);
				this.SetBinding(() => MemberModel.phone, () => lblPhone.Text, BindingMode.OneWay);
				#endregion

				#region physical
				this.SetBinding(() => MemberModel.weight, () => txtWeight.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.height, () => txtHeight.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bmi, () => txtBMI.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.fper, () => txtFatPercentage.Text, BindingMode.TwoWay);
				#endregion

				#region goals
				this.SetBinding(() => MemberModel.goalDate, () => txtGoalDate.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.goalName, () => txtGoalName.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.goalLoad, () => txtGoalLoad.Text, BindingMode.TwoWay);
				#endregion

				#region best results
				this.SetBinding(() => MemberModel.sprint, () => txtSprint.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.olympic, () => txtOlympic.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.hdistance, () => txtHDistance.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.fdistance, () => txtDistance.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.fmarathon, () => txtMarathon.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.hmarathon, () => txtHMarathon.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.krun, () => txt10KRun.Text, BindingMode.TwoWay);
				#endregion

				#region self ranking
				this.SetBinding(() => MemberModel.srSwim, () => txtRankSwim.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.srRun, () => txtRankRun.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.srBike, () => txtRankBike.Text, BindingMode.TwoWay);
				#endregion

				#region swim experience
				this.SetBinding(() => MemberModel.sZone1HR, () => txtSZone1HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone2HR, () => txtSZone2HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone3HR, () => txtSZone3HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone4HR, () => txtSZone4HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone5HR, () => txtSZone5HR.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.sZone1PACE, () => txtSZone1PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone2PACE, () => txtSZone2PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone3PACE, () => txtSZone3PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone4PACE, () => txtSZone4PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sZone5PACE, () => txtSZone5PACE.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.sFTPace, () => txtSFTPace.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.sFTPHB, () => txtSFTPHB.Text, BindingMode.TwoWay);
				#endregion

				#region run experience
				this.SetBinding(() => MemberModel.rZone1HR, () => txtRZone1HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone2HR, () => txtRZone2HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone3HR, () => txtRZone3HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone4HR, () => txtRZone4HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone5HR, () => txtRZone5HR.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.rZone1PACE, () => txtRZone1PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone2PACE, () => txtRZone2PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone3PACE, () => txtRZone3PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone4PACE, () => txtRZone4PACE.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone5PACE, () => txtRZone5PACE.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.rZone1POWER, () => txtRZone1Power.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone2POWER, () => txtRZone2Power.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone3POWER, () => txtRZone3Power.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone4POWER, () => txtRZone4Power.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rZone5POWER, () => txtRZone5Power.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.rFTPace, () => txtRFTPace.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rFTPHB, () => txtRFTPHB.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.rFTPower, () => txtRFTPower.Text, BindingMode.TwoWay);
				#endregion

				#region bike experience
				this.SetBinding(() => MemberModel.bZone1HR, () => txtBZone1HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone2HR, () => txtBZone2HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone3HR, () => txtBZone3HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone4HR, () => txtBZone4HR.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone5HR, () => txtBZone5HR.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.bZone1POWER, () => txtBZone1POWER.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone2POWER, () => txtBZone2POWER.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone3POWER, () => txtBZone3POWER.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone4POWER, () => txtBZone4POWER.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bZone5POWER, () => txtBZone5POWER.Text, BindingMode.TwoWay);

				this.SetBinding(() => MemberModel.bFTPHB, () => txtBFTPHB.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.bFTPower, () => txtBFTPower.Text, BindingMode.TwoWay);
				#endregion
			}
			catch (Exception err)
			{
				Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
		}

		#region update actions
		void ActionUpdate(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			var result = UpdateUserDataJson(MemberModel.rootMember);

			ShowMessageBox(null, result, "Cancel", new[] { "OK" }, ActionBackCancel);
		}
		#endregion

		#region Action Collepse
		void ActionCollepse(object sender, EventArgs e)
		{
			switch (int.Parse(((RelativeLayout)sender).Tag.ToString()))
			{
				case Constants.TAG_ANDROID_COLLEPS_PHYSICAL:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewPhysical));
					break;
				case Constants.TAG_ANDROID_COLLEPS_GOALS:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewGoals));
					break;
				case Constants.TAG_ANDROID_COLLEPS_BEST_RESULTS:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewBestResults));
					break;
				case Constants.TAG_ANDROID_COLLEPS_SELF_RANKING:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewSelfRankings));
					break;
				default:
					break;
			}
		}

		void CollepseAnimation(LinearLayout content)
		{
			if (content.Visibility.Equals(ViewStates.Gone))
			{
				content.Visibility = ViewStates.Visible;

				int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				content.Measure(widthSpec, heightSpec);

				ValueAnimator mAnimator = slideAnimator(0, content.MeasuredHeight, content);
				mAnimator.Start();
			}
			else {
				int finalHeight = content.Height;

				ValueAnimator mAnimator = slideAnimator(finalHeight, 0, content);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					content.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, LinearLayout content)
		{
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				var value = (int)animator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = content.LayoutParameters;
				layoutParams.Height = value;
				content.LayoutParameters = layoutParams;
			};
			return animator;
		}
		#endregion

		#region Action Edit

		void ActionEdit(object sender, EventArgs e)
		{
			switch (int.Parse(((TextView)sender).Tag.ToString()))
			{
				case Constants.TAG_EDIT_PHYSICAL:
					txtWeight.Enabled = !txtWeight.Enabled;
					txtHeight.Enabled = !txtHeight.Enabled;
					txtBMI.Enabled = !txtBMI.Enabled;
					txtFatPercentage.Enabled = !txtFatPercentage.Enabled;
					txtWeight.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtHeight.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtBMI.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtFatPercentage.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					break;
				case Constants.TAG_EDIT_GOALS:
					txtGoalDate.Enabled = !txtGoalDate.Enabled;
					txtGoalName.Enabled = !txtGoalName.Enabled;
					txtGoalLoad.Enabled = !txtGoalLoad.Enabled;
					txtGoalDate.SetBackgroundColor(txtGoalDate.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtGoalName.SetBackgroundColor(txtGoalName.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtGoalLoad.SetBackgroundColor(txtGoalLoad.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				case Constants.TAG_EDIT_BEST_RESULTS:
					txtSprint.Enabled = !txtSprint.Enabled;
					txtOlympic.Enabled = !txtOlympic.Enabled;
					txtHDistance.Enabled = !txtHDistance.Enabled;
					txtDistance.Enabled = !txtDistance.Enabled;
					txtMarathon.Enabled = !txtMarathon.Enabled;
					txtHMarathon.Enabled = !txtHMarathon.Enabled;
					txt10KRun.Enabled = !txt10KRun.Enabled;
					txtSprint.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtOlympic.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtHDistance.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtDistance.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtMarathon.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtHMarathon.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txt10KRun.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				case Constants.TAG_EDIT_SELF_RANKING:
					txtRankSwim.Enabled = !txtRankSwim.Enabled;
					txtRankRun.Enabled = !txtRankRun.Enabled;
					txtRankBike.Enabled = !txtRankBike.Enabled;
					txtRankSwim.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRankRun.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRankBike.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				case Constants.TAG_EDIT_SWIM:
					txtSZone1HR.Enabled = !txtSZone1HR.Enabled;
					txtSZone2HR.Enabled = !txtSZone2HR.Enabled;
					txtSZone3HR.Enabled = !txtSZone3HR.Enabled;
					txtSZone4HR.Enabled = !txtSZone4HR.Enabled;
					txtSZone5HR.Enabled = !txtSZone5HR.Enabled;
					txtSZone1PACE.Enabled = !txtSZone1PACE.Enabled;
					txtSZone2PACE.Enabled = !txtSZone2PACE.Enabled;
					txtSZone3PACE.Enabled = !txtSZone3PACE.Enabled;
					txtSZone4PACE.Enabled = !txtSZone4PACE.Enabled;
					txtSZone5PACE.Enabled = !txtSZone5PACE.Enabled;
					txtSFTPace.Enabled = !txtSFTPace.Enabled;
					txtSFTPHB.Enabled = !txtSFTPHB.Enabled;
					txtSZone1HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone2HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone3HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone4HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone5HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone1PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone2PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone3PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone4PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSZone5PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSFTPace.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtSFTPHB.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				case Constants.TAG_EDIT_RUN:
					txtRZone1HR.Enabled = !txtRZone1HR.Enabled;
					txtRZone2HR.Enabled = !txtRZone2HR.Enabled;
					txtRZone3HR.Enabled = !txtRZone3HR.Enabled;
					txtRZone4HR.Enabled = !txtRZone4HR.Enabled;
					txtRZone5HR.Enabled = !txtRZone5HR.Enabled;
					txtRZone1PACE.Enabled = !txtRZone1PACE.Enabled;
					txtRZone2PACE.Enabled = !txtRZone2PACE.Enabled;
					txtRZone3PACE.Enabled = !txtRZone3PACE.Enabled;
					txtRZone4PACE.Enabled = !txtRZone4PACE.Enabled;
					txtRZone5PACE.Enabled = !txtRZone5PACE.Enabled;
					txtRZone1Power.Enabled = !txtRZone1Power.Enabled;
					txtRZone2Power.Enabled = !txtRZone2Power.Enabled;
					txtRZone3Power.Enabled = !txtRZone3Power.Enabled;
					txtRZone4Power.Enabled = !txtRZone4Power.Enabled;
					txtRZone5Power.Enabled = !txtRZone5Power.Enabled;
					txtRFTPace.Enabled = !txtRFTPace.Enabled;
					txtRFTPHB.Enabled = !txtRFTPHB.Enabled;
					txtRFTPower.Enabled = !txtRFTPower.Enabled;
					txtRZone1HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone2HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone3HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone4HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone5HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone1PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone2PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone3PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone4PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone5PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone1Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone2Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone3Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone4Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRZone5Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRFTPace.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRFTPHB.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtRFTPower.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				case Constants.TAG_EDIT_BIKE:
					txtBZone1HR.Enabled = !txtBZone1HR.Enabled;
					txtBZone2HR.Enabled = !txtBZone2HR.Enabled;
					txtBZone3HR.Enabled = !txtBZone3HR.Enabled;
					txtBZone4HR.Enabled = !txtBZone4HR.Enabled;
					txtBZone5HR.Enabled = !txtBZone5HR.Enabled;
					txtBZone1POWER.Enabled = !txtBZone1POWER.Enabled;
					txtBZone2POWER.Enabled = !txtBZone2POWER.Enabled;
					txtBZone3POWER.Enabled = !txtBZone3POWER.Enabled;
					txtBZone4POWER.Enabled = !txtBZone4POWER.Enabled;
					txtBZone5POWER.Enabled = !txtBZone5POWER.Enabled;
					txtBFTPower.Enabled = !txtBFTPower.Enabled;
					txtBFTPHB.Enabled = !txtBFTPHB.Enabled;
					txtBZone1HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone2HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone3HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone4HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone5HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone1POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone2POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone3POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone4POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBZone5POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBFTPower.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					txtBFTPHB.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray); ;
					break;
				default:
					break;
			}
		}

		#endregion
		private void SetupPicker(EditText textView, string format, string title = "")
		{
			int dcount = 0;
			if (format == Constants.PICKER_TIME)
				dcount = 3;
			else if (format == Constants.PICKER_PACE || format == Constants.PICKER_HR)
				dcount = 2;
			else
				dcount = 1;

			textView.Touch += (object sender, View.TouchEventArgs e) =>
			{
				if (e.Event.Action == MotionEventActions.Down)
				{
					TimeFormatDialog myDiag = TimeFormatDialog.newInstance((EditText)sender, dcount, format, title);
					myDiag.Show(FragmentManager, "Diag");
				}
			};
		}
		private void SetupDatePicker(object sender, View.TouchEventArgs e)
		{
			if (e.Event.Action == MotionEventActions.Down)
			{
				DatePickerDialog ddtime = new DatePickerDialog(this, OnDateSet, DateTime.Today.Year, 
				                                               					DateTime.Today.Month - 1,
													 							DateTime.Today.Day
															 	);

				if (txtGoalDate.Text != "")
					ddtime.DatePicker.DateTime = DateTime.ParseExact(txtGoalDate.Text, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
					//ddtime.DatePicker.DateTime = DateTime.Parse(txtGoalDate.Text);

				var origin = new DateTime(1970, 1, 1);
				ddtime.DatePicker.MinDate = (long)(DateTime.Now - origin).TotalMilliseconds;
				ddtime.DatePicker.MaxDate = (long)(DateTime.Now.Date.AddYears(3) - origin).TotalMilliseconds;
				ddtime.SetTitle("");
				ddtime.Show();
			}
		}
		private void SetupAdjustPicker(EditText textView, SeekBar seekBar, int maxValue)
		{
			textView.Touch += (object sender, View.TouchEventArgs e) =>
			{
				if (e.Event.Action == MotionEventActions.Down)
				{
					TimeFormatDialog myDiag = TimeFormatDialog.newInstance((EditText)sender, 1, "adjust", "");
					myDiag.Show(FragmentManager, "Diag");
				}
			};
		}
		void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
		{
			txtGoalDate.Text = e.Date.ToString("MM-dd-yyyy");
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
