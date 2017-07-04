using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Android;
using System;
using Android.Util;
using Android.Runtime;
using PortableLibrary;
using Com.GrapeCity.Xuni.Core;

namespace goheja
{
	[Activity(Label = "Go-Heja", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SwipeTabActivity : BaseActivity
	{
		string[] PermissionsCalendar =
		{
			Manifest.Permission.ReadCalendar,
			Manifest.Permission.WriteCalendar
		};
		const int RequestCalendarId = 0;

		static Intent serviceIntent = null;

		RelativeLayout tabCalendar, tabAnalytics, tabProfile;

		NonSwipeableViewPager _pager;

		Android.Graphics.Color cTabEnable = new Android.Graphics.Color(146, 146, 146);
		Android.Graphics.Color cTabDisable = new Android.Graphics.Color(69, 69, 69);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.SwipeTabActivity);

			LicenseManager.Key = License.Key;

			InitUISettings();

			if (!AppSettings.isFakeUser)
				CheckCalendarPermission();
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				if (_pager.CurrentItem != 0)
				{
					SetPage(0);
					return true;
				}
				else if(AppSettings.CurrentUser.userType == (int)Constants.USER_TYPE.COACH)
				{
					AppSettings.isFakeUser = false;
                    BackAction();
				}

				return false;
			}

			return base.OnKeyDown(keyCode, e);
		}

		void BackAction()
		{
			var fromWhere = Intent.GetStringExtra("FromWhere");

			if (!string.IsNullOrEmpty(fromWhere) && fromWhere.Equals("CoachList"))
			{
                var nextIntent = new Intent(this, typeof(CoachHomeActivity));
				nextIntent.PutExtra("FromWhere", "CoachList");
				StartActivityForResult(nextIntent, 0);
				Finish();
			}
			else
			{
				ActionBackCancel();
			}
		}

		private void InitUISettings()
		{
			tabCalendar = FindViewById<RelativeLayout>(Resource.Id.tabCalendar);
			tabAnalytics = FindViewById<RelativeLayout>(Resource.Id.tabAnalytics);
			tabProfile = FindViewById<RelativeLayout>(Resource.Id.tabProfile);

			TabViewAdapter _adaptor = new TabViewAdapter(SupportFragmentManager, this);
			_pager = FindViewById<NonSwipeableViewPager>(Resource.Id.pager);
			_pager.SetPagingEnabled(false);
			_pager.OffscreenPageLimit = 2;
			_pager.Adapter = _adaptor;
			_pager.PageSelected += PagerOnPageSelected;

			FindViewById<RelativeLayout>(Resource.Id.tabCalendar).Click += (sender, args) => { SetPage(0); };
			FindViewById<RelativeLayout>(Resource.Id.tabAnalytics).Click += (sender, args) => { SetPage(1); };
			FindViewById<RelativeLayout>(Resource.Id.tabProfile).Click += (sender, args) => { SetPage(2); };

			FindViewById<RelativeLayout>(Resource.Id.ActionBarMain).Visibility = AppSettings.isFakeUser ? ViewStates.Gone : ViewStates.Visible;
		}

		public void SetPage(int position)
		{
			_pager.SetCurrentItem(position, true);
		}

		private void PagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
		{
			SetSelect(e.Position);
		}

		private void SetSelect(int position)
		{
			switch (position)
			{
				case 0:
					tabCalendar.SetBackgroundColor(cTabEnable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabDisable);
					break;
				case 1:
					tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabEnable);
					tabProfile.SetBackgroundColor(cTabDisable);
					break;
				case 2:
					tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabEnable);
					break;
				case 3:
					tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabEnable);
					break;
			}
		}

		#region grant calendar access permission
		private void CheckCalendarPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartBackgroundService();
			}
			else
			{
				RequestCalendarPermission();
			}
		}
		void RequestCalendarPermission()
		{
			const string rdPermission = Manifest.Permission.ReadCalendar;
			const string wrPermission = Manifest.Permission.WriteCalendar;
			if (CheckSelfPermission(rdPermission) == (int)Permission.Granted && CheckSelfPermission(wrPermission) == (int)Permission.Granted)
			{
				StartBackgroundService();
				return;
			}

			if (ShouldShowRequestPermissionRationale(rdPermission) || ShouldShowRequestPermissionRationale(wrPermission))
			{
				ShowMessageBox(null, "Calendar access is required to show your events on your device calendar.", "OK", "Cancel", SendingPermissionRequest);
				return;
			}

			SendingPermissionRequest();
		}

		void SendingPermissionRequest()
		{
			ActivityCompat.RequestPermissions(this, PermissionsCalendar, RequestCalendarId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == RequestCalendarId && grantResults[0] == Permission.Granted)
			{
				StartBackgroundService();
			}
		}

		void StartBackgroundService()
		{
			if (serviceIntent == null)
			{
				AppSettings.baseVC = this;
				serviceIntent = new Intent(this, typeof(BackgroundService));
				this.StartService(serviceIntent);
			}
		}
		#endregion
	}

	public class NonSwipeableViewPager : ViewPager
	{
		bool isEnable;

		public NonSwipeableViewPager(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{

		}

		public NonSwipeableViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			this.isEnable = true;
		}


		public override bool OnTouchEvent(MotionEvent e)
		{
			if (this.isEnable)
			{
				return base.OnTouchEvent(e);
			}
			return false;
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			if (this.isEnable)
			{
				return base.OnInterceptTouchEvent(ev);
			}
			return false;
		}

		public void SetPagingEnabled(bool enabled)
		{
			this.isEnable = enabled;
		}
	}
}
