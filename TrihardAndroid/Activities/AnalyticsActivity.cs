using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Preferences;
using Android.Content.PM;
using System.Timers;
using PortableLibrary;
using Android;
using Android.Support.V4.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Collections.Generic;

namespace goheja
{
	[Activity(Label = "Go-Heja", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AnalyticsActivity : BaseActivity, ILocationListener, IOnMapReadyCallback, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
	{
		readonly string[] PermissionsLocation =
		{
		  Manifest.Permission.AccessCoarseLocation,
		  Manifest.Permission.AccessFineLocation
		};
		const int RequestLocationId = 0;

		enum RIDE_TYPE
		{
			bike = 0,
			run = 1,
			mountain = 2
		};

		enum PRACTICE_STATE
		{
			ready,
			playing,
			pause
		}
		public int pType;
		PRACTICE_STATE pState = PRACTICE_STATE.ready;

		LocationManager _locationManager;

		SupportMapFragment mMapViewFragment;
		GoogleMap mMapView = null;
		Marker markerMyLocation = null;

		EventPoints mEventMarker = new EventPoints();
		IList<string> pointIDs;


		trackSvc.Service1 svc = new trackSvc.Service1();

		ISharedPreferences contextPref;
		ISharedPreferences filePref;

		ISharedPreferencesEditor contextPrefEdit;
		ISharedPreferencesEditor filePrefEdit;

		Location _currentLocation;
		Location _lastLocation;

		TextView _speedText, _altitudeText, _distance, _timerText, _title;
		float lastAlt, dist, gainAlt;

		int fFlag = 1;

		Button btnStartPause;
		Button btnStop;
		Button btnLapDist;

		Timer _timer = new Timer();
		int duration = 0;
		int lapDuration = 0;

		DateTime tempTime = DateTime.Now;

		private RootMemberModel MemberModel { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AnalyticsActivity);

			this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			if (!IsNetEnable()) return;

			MemberModel = new RootMemberModel();
			MemberModel.rootMember = GetUserObject();

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			contextPref = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
			filePref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

			contextPrefEdit = contextPref.Edit();
			filePrefEdit = filePref.Edit();

			contextPrefEdit.PutFloat("gainAlt", 0f).Commit();
			contextPrefEdit.PutFloat("lastAlt", 0f).Commit();
			contextPrefEdit.PutFloat("dist", 0f).Commit();

			filePrefEdit.PutFloat("lastAlt", 0f);
			filePrefEdit.PutFloat("gainAlt", 0f);
			filePrefEdit.PutFloat("distance", 0f);
			filePrefEdit.PutString("prevLoc", "");
			filePrefEdit.Commit();

			InitUISettings();
		}

		void InitUISettings()
		{
			pType = Intent.GetIntExtra("pType", 0);

			TextView speedLbl = FindViewById<TextView>(Resource.Id.speedTv);
			Button dummyBtn = FindViewById<Button>(Resource.Id.dummyType);

			switch (pType)
			{
				case (int)RIDE_TYPE.bike:
					speedLbl.Text = "km/h";
					dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound_new);
					break;
				case (int)RIDE_TYPE.run:
					speedLbl.Text = "min/km";
					dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
					break;
				case (int)RIDE_TYPE.mountain:
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

			FindViewById<Button>(Resource.Id.btnStartPause).Click += ActionStartPause;
			FindViewById<Button>(Resource.Id.btnStop).Click += ActionStop;
			FindViewById<Button>(Resource.Id.btnBack).Click += ActionBack;

			btnStartPause = FindViewById<Button>(Resource.Id.btnStartPause);
			btnStop = FindViewById<Button>(Resource.Id.btnStop);
			btnLapDist = FindViewById<Button>(Resource.Id.btnLD);
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
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
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

			if (pState == PRACTICE_STATE.ready)
			{
				StartTimer();

				btnStartPause.SetBackgroundResource(Resource.Drawable.resume_inactive);
				btnStop.Visibility = ViewStates.Visible;


				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);

				pState = PRACTICE_STATE.playing;

				try
				{
					var name = MemberModel.firstname + " " + MemberModel.lastname;
					var loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
					DateTime dt = DateTime.Now;
					dist = contextPref.GetFloat("dist", 0f);
					var country = MemberModel.country;

					var userId = GetUserID();

					svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, userId, country, dist, true, gainAlt, true, _currentLocation.Bearing, true, 1, true, pType.ToString(), Constants.SPEC_GROUP_TYPE);
				}
				catch (Exception err)
				{
					//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
				}
			}
			else if (pState == PRACTICE_STATE.playing)
			{
				btnStartPause.SetBackgroundResource(Resource.Drawable.resume_active);
				btnStop.Visibility = ViewStates.Visible;

				_locationManager.RemoveUpdates(this);

				pState = PRACTICE_STATE.pause;
			}
			else if (pState == PRACTICE_STATE.pause)
			{
				btnStartPause.SetBackgroundResource(Resource.Drawable.resume_inactive);
				btnStop.Visibility = ViewStates.Visible;

				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);

				pState = PRACTICE_STATE.playing;
			}
		}

		void StartTimer()
		{
			_timer.Interval = 1000;
			_timer.Elapsed += OnTimedEvent;
			_timer.Enabled = true;
		}
		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (pState == PRACTICE_STATE.playing)
			{
				duration++;
				lapDuration++;
			}
			var timespan = TimeSpan.FromSeconds(duration);

			RunOnUiThread(() =>
			{
				_timerText.Text = timespan.ToString(@"hh\:mm\:ss");
			});
		}

		private void ActionBack(object sender, EventArgs e)
		{
			if (pState == PRACTICE_STATE.ready)
			{
				var activity = new Intent();
				SetResult(Result.Canceled, activity);
				Finish();
			}
			else
			{
				ShowMessageBox(null, Constants.MSG_COMFIRM_STOP_SPORT_COMP, "OK", "Cancel", StopPractice);
			}
		}
		private void ActionStop(object sender, EventArgs e)
		{
			StopPractice();
		}
		private void StopPractice()
		{
			_title.Text = "Go-Heja Live is ready...";

			try
			{
				var name = MemberModel.firstname + " " + MemberModel.lastname;
				var loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
				DateTime dt = DateTime.Now;
				dist = contextPref.GetFloat("dist", 0f);
				var country = MemberModel.country;
				var userId = GetUserID();

				svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, userId, country, dist, true, gainAlt, true, _currentLocation.Bearing, true, 2, true, pType.ToString(), Constants.SPEC_GROUP_TYPE);
			}
			catch (Exception err)
			{
				//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}

			filePrefEdit.PutFloat("lastAlt", 0f).Commit();
			filePrefEdit.PutFloat("gainAlt", 0f).Commit();
			filePrefEdit.PutFloat("distance", 0f).Commit();
			filePrefEdit.PutString("prevLoc", "").Commit();
			filePrefEdit.PutFloat("lastAlt", 0f).Commit();
			filePrefEdit.PutFloat("gainAlt", 0f).Commit();
			filePrefEdit.PutFloat("distance", 0f).Commit();
			filePrefEdit.PutString("prevLoc", "").Commit();
			filePrefEdit.PutFloat("dist", 0f).Commit();

			contextPrefEdit.PutFloat("dist", 0f).Commit();

			_locationManager.RemoveUpdates(this);

			ActionBackCancel();
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

		public void OnLocationChanged(Location location)
		{
			string status = "online";

			_currentLocation = location;

			try
			{
				RunOnUiThread(() =>
				{
					if (_currentLocation == null)
					{
						_title.Text = "Unable to determine your location.";
					}
					else
					{
						_title.Text = "On the go";

						//if (!isPaused)
						{
							if (_lastLocation != null)
							{
								dist = contextPref.GetFloat("dist", 0f) + _currentLocation.DistanceTo(_lastLocation) / 1000;
							}
							lastAlt = contextPref.GetFloat("lastAlt", 0f);
							float dAlt = difAlt(lastAlt, float.Parse(_currentLocation.Altitude.ToString()));
							if (dAlt < 4) gainAlt = gainAlt + dAlt;

							contextPrefEdit.PutFloat("gainAlt", gainAlt).Commit();
						}

						if (pType == (int)RIDE_TYPE.bike)
						{
							_speedText.Text = (_currentLocation.Speed * 3.6f).ToString("0.00");
							btnLapDist.Text = "Lap distance : " + (dist % 5).ToString("0.00");
							if (dist % 5 < 0.01)
							{
								if (DateTime.Now - tempTime > ts)
								{
									tempTime = DateTime.Now;
									vibrate(1000);
								}
							}
						}
						if (pType == (int)RIDE_TYPE.run)
						{
							_speedText.Text = (16.6666 / (_currentLocation.Speed)).ToString("0.00");
							btnLapDist.Text = "Lap distance : " + (dist % 1).ToString("0.00");
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

						_altitudeText.Text = gainAlt.ToString("0.00");
						_distance.Text = (dist).ToString("0.00");

						//if (!isPaused)
						{
							var name = MemberModel.firstname + " " + MemberModel.lastname;
							DateTime dt = DateTime.Now;
							float speed = float.Parse(_currentLocation.Speed.ToString()) * 3.6f;
							var country = MemberModel.country;
							var userId = GetUserID();

							record merecord = new record(name, _currentLocation.Latitude, _currentLocation.Longitude, dt, AppSettings.DeviceUDID, userId, country, dist, speed, gainAlt, _currentLocation.Bearing, 0, pType.ToString());
							handleRecord updateRecord = new handleRecord();
							status = updateRecord.updaterecord(merecord, IsNetEnable());//the record and is there internet connection

							//mMapView.AnimateCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), Constants.MAP_ZOOM_LEVEL, 45f, _currentLocation.Bearing)));
							SetMapPosition(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), _currentLocation.Bearing);
						}

						_lastLocation = _currentLocation;
						contextPrefEdit.PutFloat("lastAlt", float.Parse(_currentLocation.Altitude.ToString())).Commit();
						contextPrefEdit.PutFloat("dist", dist).Commit();
						if (fFlag == 1 || status == "backFromOffline")
						{
							status = "online";
						}
						fFlag = 0;
					}
				});
			}
			catch (Exception err)
			{
				//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
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

