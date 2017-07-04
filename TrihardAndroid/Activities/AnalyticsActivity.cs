using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Content.PM;
using System.Timers;
using PortableLibrary;
using Android;
using Android.Support.V4.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace goheja
{
	[Activity(Label = "Go-Heja", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AnalyticsActivity : BaseActivity, ILocationListener, IOnMapReadyCallback, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
	{
		const int RequestLocationId = 0;
        readonly string[] PermissionsLocation =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };

        RootMemberModel MemberModel { get; set; }

        Constants.EVENT_TYPE pType;
        Constants.PLAYING_STATE pState = Constants.PLAYING_STATE.READY;

		LocationManager _locationManager;

		SupportMapFragment mMapViewFragment;
		GoogleMap mMapView = null;
		Marker markerMyLocation = null;

		EventPoints mEventMarker = new EventPoints();
		IList<string> pointIDs;

		Location _currentLocation, _lastLocation;

		TextView _speedText, _altitudeText, _distance, _timerText, _title;

		Button btnStartPause, btnStop, btnBack;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AnalyticsActivity);

			Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			if (!IsNetEnable()) return;

            pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), Intent.GetIntExtra("pType", 0));

			MemberModel = new RootMemberModel();
			MemberModel.rootMember = GetUserObject();

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			InitUISettings();

            SetControlButtons();
		}

		void InitUISettings()
		{
			TextView speedLbl = FindViewById<TextView>(Resource.Id.speedTv);
			Button dummyBtn = FindViewById<Button>(Resource.Id.dummyType);

			switch (pType)
			{
                case Constants.EVENT_TYPE.BIKE:
					speedLbl.Text = "km/h";
					dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound_new);
					break;
                case Constants.EVENT_TYPE.RUN:
					speedLbl.Text = "min/km";
					dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
					break;
                case Constants.EVENT_TYPE.OTHER:
					speedLbl.Text = "km/h";
					dummyBtn.SetBackgroundResource(Resource.Drawable.icon_06);
					break;
			}

			_title = FindViewById<TextView>(Resource.Id.TitleBarText);
			_speedText = FindViewById<TextView>(Resource.Id.tvSpeed);
			_altitudeText = FindViewById<TextView>(Resource.Id.tvAltitude);
			_distance = FindViewById<TextView>(Resource.Id.tvDistance);
			_timerText = FindViewById<TextView>(Resource.Id.btnTotalTime);

			mMapViewFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mMapViewFragment.GetMapAsync(this);

            btnStartPause = FindViewById<Button>(Resource.Id.btnStartPause);
            btnStop = FindViewById<Button>(Resource.Id.btnStop);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);

			btnStartPause.Click += ActionStartPause;
			btnStop.Click += ActionStop;
			btnBack.Click += ActionBack;
		}

		#region google map

		public void OnMapReady(GoogleMap googleMap)
		{
			if (googleMap == null) return;

			mMapView = googleMap;
			mMapView.MyLocationEnabled = false;

			mMapView.SetOnMarkerClickListener(this);

			CheckLocationPermission();
		}

		void SetMap()
		{
			var currentLocation = GetGPSLocation();

			try
			{
				MarkerOptions markerOpt = new MarkerOptions();
				markerOpt.SetPosition(new LatLng(currentLocation.Latitude, currentLocation.Longitude));

				var metrics = Resources.DisplayMetrics;
				var wScreen = metrics.WidthPixels;

				Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.pin_me);
				Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 10, true);
				markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

				RunOnUiThread(() =>
				{
					markerMyLocation = mMapView.AddMarker(markerOpt);
				});

				SetMapPosition(new LatLng(currentLocation.Latitude, currentLocation.Longitude));

				SetNearestEventMarkers(currentLocation);
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		void SetNearestEventMarkers(Location currentLocation)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_ALL_MARKERS);

				var userId = GetUserID();

				mEventMarker = GetNearestEventMarkers(userId);

				HideLoadingView();

				if (mEventMarker == null || mEventMarker.markers == null || mEventMarker.markers.Count == 0) return;

				try
				{
					var mapBounds = new LatLngBounds.Builder();

					mapBounds.Include(new LatLng(currentLocation.Latitude, currentLocation.Longitude));

					pointIDs = new List<string>();

					RunOnUiThread(() =>
					{
						for (int i = 0; i < mEventMarker.markers.Count; i++)
						{
							var point = mEventMarker.markers[i];
							var pointLocation = new LatLng(point.lat, point.lng);
							mapBounds.Include(pointLocation);

							AddMapPin(pointLocation, point.type);
						}

						mMapView.MoveCamera(CameraUpdateFactory.NewLatLngBounds(mapBounds.Build(), 50));
					});
				}
				catch (Exception ex)
				{
					ShowTrackMessageBox(ex.Message);
				}
			});
		}

		void AddMapPin(LatLng position, string type)
		{
			try
			{
				MarkerOptions markerOpt = new MarkerOptions();
				markerOpt.SetPosition(position);

				var metrics = Resources.DisplayMetrics;
				var wScreen = metrics.WidthPixels;

				Bitmap bmp = GetPinIconByType(type);
				Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 7, true);
				markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

				RunOnUiThread(() =>
				{
					var marker = mMapView.AddMarker(markerOpt);
					pointIDs.Add(marker.Id);
				});
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		public bool OnMarkerClick(Marker marker)
		{
			try
			{
				PortableLibrary.Point selectedPoint = null;
				for (var i = 0; i < pointIDs.Count; i++)
				{
					if (marker.Id == pointIDs[i])
						selectedPoint = mEventMarker.markers[i];
				}
				if (selectedPoint == null) return false;

				PointInfoDialog myDiag = PointInfoDialog.newInstance(selectedPoint);
				myDiag.Show(FragmentManager, "Diag");
			}
			catch(Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return true;
		}

        #endregion



        private void ActionStartPause(object sender, EventArgs e)
        {
            if (!_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                ShowMessageBox(null, Constants.MSG_GPS_DISABLED);
                return;
            }

            switch (pState)
            {
                case Constants.PLAYING_STATE.READY:
                    pState = Constants.PLAYING_STATE.PLAYING;
                    StartTimer();
                    _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
                    RecordPractice(Constants.RECORDING_STATE.START);
                    break;
                case Constants.PLAYING_STATE.PLAYING:
                    pState = Constants.PLAYING_STATE.PAUSE;
                    _locationManager.RemoveUpdates(this);
                    break;
                case Constants.PLAYING_STATE.PAUSE:
                    pState = Constants.PLAYING_STATE.PLAYING;
                    _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
                    break;
            }

            SetControlButtons();
        }

        void SetControlButtons()
        {
            switch (pState)
            {
                case Constants.PLAYING_STATE.READY:
                    btnBack.Visibility = ViewStates.Visible;
                    btnStop.Visibility = ViewStates.Gone;
                    btnStartPause.SetBackgroundResource(Resource.Drawable.icon_play);
                    break;
                case Constants.PLAYING_STATE.PLAYING:
					btnBack.Visibility = ViewStates.Gone;
                    btnStop.Visibility = ViewStates.Visible;
                    btnStartPause.SetBackgroundResource(Resource.Drawable.icon_pause);
                    break;
                case Constants.PLAYING_STATE.PAUSE:
					btnBack.Visibility = ViewStates.Gone;
					btnStop.Visibility = ViewStates.Visible;
                    btnStartPause.SetBackgroundResource(Resource.Drawable.icon_play);
                    break;
            }
        }
		

		private void ActionBack(object sender, EventArgs e)
		{
            ActionBackCancel();
		}

		private void ActionStop(object sender, EventArgs e)
		{
            ShowMessageBox(null, Constants.MSG_COMFIRM_STOP_SPORT_COMP, "OK", "Cancel", StopPractice);
		}

		private void StopPractice()
		{
			_title.Text = "Go-Heja Live is ready...";

            RecordPractice(Constants.RECORDING_STATE.END);

            _locationManager.RemoveUpdates(this);

			pState = Constants.PLAYING_STATE.READY;
			SetControlButtons();

            _speedText.Text = "0.0";
            _altitudeText.Text = "0.0";
            _distance.Text = "0.0";
            _timerText.Text = "00:00:00";

            lastAlt = 0;
            dist = 0;
            gainedAlt = 0;

			duration = 0;
		}


		#region grant location service
		private void CheckLocationPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartLocationService();
			}
			else
			{
				RequestLocationPermission();
			}
		}
		void RequestLocationPermission()
		{
			const string permission = Manifest.Permission.AccessFineLocation;
			if (CheckSelfPermission(permission) == (int)Permission.Granted)
			{
				StartLocationService();
				return;
			}

			if (ShouldShowRequestPermissionRationale(permission))
			{
				ShowMessageBox(null, "Location access is required to gaugo your sports.", "OK", "Cancel", SendingPermissionRequest);
				return;
			}
			SendingPermissionRequest();
		}

		void SendingPermissionRequest()
		{
			ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == RequestLocationId && grantResults[0] == Permission.Granted)
			{
				StartLocationService();
			}
		}


		#endregion
		private void StartLocationService()
		{
			_title.Text = "Searching for GPS...";
			SetMap();
		}

		#region Android Location Service methods

		///<summary>
		/// Updates UI with location data
		/// </summary>
        /// 
        DateTime tempTime = DateTime.Now;
		TimeSpan ts = new TimeSpan(0, 0, 20);

		public void vibrate(long time)
		{
			Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
			vibrator.Vibrate(time);
		}

		public void OnProviderDisabled(string provider) { 
			_title.Text = "GPS disabled"; 

			using (var alert = new AlertDialog.Builder(this))
			{
				alert.SetTitle("Please enable GPS");
				alert.SetMessage("Enable GPS in order to get your current location.");

				alert.SetPositiveButton("Enable", (senderAlert, args) =>
				{
					Intent intent = new Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);
					StartActivity(intent);
				});

				alert.SetNegativeButton("Continue", (senderAlert, args) =>
				{
					alert.Dispose();
				});

				Dialog dialog = alert.Create();
				dialog.Show();
			}
		}
		public void OnProviderEnabled(string provider) { _title.Text = "GPS enabled"; }
		public void OnStatusChanged(string provider, Availability status, Bundle extras) { _title.Text = "GPS low signal"; }


        float lastAlt, dist, gainedAlt;

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;

            if (_currentLocation == null)
            {
                _title.Text = "Unable to determine your location.";
                return;
            }

            _title.Text = "On the go";

            SetMapPosition(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), _currentLocation.Bearing);

            if (pState != Constants.PLAYING_STATE.PLAYING) return;

            try
            {
                if (_lastLocation != null)
                    dist += _currentLocation.DistanceTo(_lastLocation) / 1000;

                if (pType == Constants.EVENT_TYPE.BIKE)
                {
                    _speedText.Text = (_currentLocation.Speed * 3.6f).ToString("0.00");
                    if (dist % 5 < 0.01)
                    {
                        if (DateTime.Now - tempTime > ts)
                        {
                            tempTime = DateTime.Now;
                            vibrate(1000);
                        }
                    }
                }
                if (pType == Constants.EVENT_TYPE.RUN)
                {
                    _speedText.Text = (16.6666 / (_currentLocation.Speed)).ToString("0.00");
                    if (dist % 1 < 0.01)
                    {
                        if (DateTime.Now - tempTime > ts)
                        {
                            tempTime = DateTime.Now;
                            vibrate(1000);
                        }
                    }
                }
                if (_currentLocation.Speed < 0.1)
                {
                    _speedText.Text = "0.00";
                }

                float dAlt = DifAlt(lastAlt, (float)_currentLocation.Altitude);
				if (dAlt < 4) gainedAlt = gainedAlt + dAlt;

                _altitudeText.Text = gainedAlt.ToString("0.00");
                _distance.Text = dist.ToString("0.00");

				_lastLocation = _currentLocation;
				lastAlt = (float)_currentLocation.Altitude;

                RecordPractice(Constants.RECORDING_STATE.RECORDING);
            }
            catch (Exception err)
            {
                //Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
            }
        }

        void RecordPractice(Constants.RECORDING_STATE recordType)
        {
            var tRecord = new TRecord();

            try
            {
                tRecord.fullName = MemberModel.firstname + " " + MemberModel.lastname; ;
                tRecord.loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude); ;
                tRecord.date = DateTime.Now;
                tRecord.deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                tRecord.athid = GetUserID();
                tRecord.country = MemberModel.country;
                tRecord.distance = dist;
                tRecord.speed = float.Parse(_currentLocation.Speed.ToString()) * 3.6f;
                tRecord.gainedAlt = gainedAlt;
                tRecord.bearinng = _currentLocation.Bearing;
                tRecord.recordType = recordType;
                tRecord.sportType = pType;

                RecordPracticeTrack(tRecord);
            }
            catch
            {
            }
        }

		#endregion

		void SetMapPosition(LatLng location, float bearing = -1)
		{
			if (mMapView == null) return;

			if (bearing == -1)
				mMapView.MoveCamera(CameraUpdateFactory.NewLatLngZoom(location, Constants.MAP_ZOOM_LEVEL));
			else
				mMapView.AnimateCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(location, Constants.MAP_ZOOM_LEVEL, 45f, _currentLocation.Bearing)));

			if (markerMyLocation != null)
				markerMyLocation.Position = location;
		}

		private Location GetGPSLocation()
		{
			_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
			Location currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
			_locationManager.RemoveUpdates(this);

			if (currentLocation == null)
			{
				currentLocation = new Location("");
				currentLocation.Latitude = Constants.LOCATION_ISURAEL[0];
				currentLocation.Longitude = Constants.LOCATION_ISURAEL[1];

				_title.Text = "Unable to determine your location.";
			}
			return currentLocation;
		}

		System.Timers.Timer _timer = new System.Timers.Timer();
		int duration = 0;

		void StartTimer()
		{
			_timer.Interval = 1000;
            _timer.Elapsed -= OnTimedEvent;
			_timer.Elapsed += OnTimedEvent;
			_timer.Enabled = true;
		}
		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (pState == Constants.PLAYING_STATE.PLAYING)  duration++;

			RunOnUiThread(() =>
			{
				_timerText.Text = TimeSpan.FromSeconds(duration).ToString(@"hh\:mm\:ss");
			});
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				return false;
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}

