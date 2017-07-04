using System;
using UIKit;
using CoreLocation;
using PortableLibrary;
using Google.Maps;
using System.Drawing;
using CoreGraphics;
using System.Collections.Generic;
using System.Threading;

using Constants = PortableLibrary.Constants;
using System.Timers;

namespace location2
{
	public partial class AnalyticsViewController : BaseViewController
	{
        public Constants.EVENT_TYPE pType;
		Constants.PLAYING_STATE pState = Constants.PLAYING_STATE.READY;

		MapView mMapView;
		Marker markerMyLocation = null;

		EventPoints mEventMarker = new EventPoints();

        CLLocation _currentLocation, _lastLocation;

		public AnalyticsViewController(IntPtr handle) : base(handle)
		{
			MemberModel = new RootMemberModel();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			if (!IsNetEnable()) return;
   
            InitUISettings();

            SetControlButtons();

			InitMapView();
		}

		void InitUISettings()
		{
			switch (pType)
			{
				case Constants.EVENT_TYPE.BIKE:
					speedTypeLbl.Text = "km/h";
					imgTypeIcon.Image = UIImage.FromBundle("bikeRound_new.png");
					break;
				case Constants.EVENT_TYPE.RUN:
					speedTypeLbl.Text = "min/km";
					imgTypeIcon.Image = UIImage.FromBundle("runRound_new.png");
					break;
				case Constants.EVENT_TYPE.OTHER:
					speedTypeLbl.Text = "km/h";
					imgTypeIcon.Image = UIImage.FromBundle("icon_06.png");
					break;
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}
		void InitMapView()
		{
			var myLocation = LocationHelper.GetLocationResult();
			var myLocation2D = new CLLocationCoordinate2D(myLocation.Latitude, myLocation.Longitude);

			var camera = CameraPosition.FromCamera(myLocation2D, Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;

			mMapView.TappedMarker = ClickedDropItem;

			markerMyLocation = new Marker
			{
				Position = myLocation2D,
				Map = mMapView,
				Icon = UIImage.FromFile("pin_me.png")
			};
		}

		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			viewMapContent.LayoutIfNeeded();
			var width = viewMapContent.Frame.Width;
			var height = viewMapContent.Frame.Height;
			mMapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mMapView);

			SetNearestEventMarkers();
		}

		void SetNearestEventMarkers()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_ALL_MARKERS);

				MemberModel.rootMember = GetUserObject();

				var userId = GetUserID();
				mEventMarker = GetNearestEventMarkers(userId);

				HideLoadingView();

				InvokeOnMainThread(() =>
				{
					var boundPoints = new List<CLLocationCoordinate2D>();

					if (mEventMarker != null && mEventMarker.markers.Count > 0)
					{
						for (int i = 0; i < mEventMarker.markers.Count; i++)
						{
							var point = mEventMarker.markers[i];
							var imgPin = GetPinIconByType(point.type);
							var pointLocation = new CLLocationCoordinate2D(point.lat, point.lng);
							boundPoints.Add(pointLocation);

							AddMapPin(pointLocation, imgPin, i);
						}
					}

					if (boundPoints.Count > 0)
					{
						var mapBounds = new CoordinateBounds();

						var myLocation = LocationHelper.GetLocationResult();
						var myLocation2D = new CLLocationCoordinate2D(myLocation.Latitude, myLocation.Longitude);
						boundPoints.Add(myLocation2D);

						foreach (var bound in boundPoints)
							mapBounds = mapBounds.Including(bound);
						mMapView.MoveCamera(CameraUpdate.FitBounds(mapBounds, 50.0f));
					}
				});
			});
		}

		void AddMapPin(CLLocationCoordinate2D position, UIImage icon, int zIndex)
		{
			var marker = new Marker
			{
				Position = position,
				Map = mMapView,
				Icon = icon,
				ZIndex = zIndex
			};
		}

		

		#region map pin click event
		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			if (marker.ZIndex == -1) return true;

			var selectedPoint = mEventMarker.markers[marker.ZIndex];

			PointInfoView cpuv = PointInfoView.Create(selectedPoint);
			cpuv.PopUp(true, delegate
			{
				Console.WriteLine("cpuv will close");
			});

			return true;
		}
		#endregion

		partial void ActionStartPause(UIButton sender)
		{

			switch (pState)
			{
				case Constants.PLAYING_STATE.READY:
					pState = Constants.PLAYING_STATE.PLAYING;
					StartTimer();
					LocationHelper.StartLocationManager();
                    LocationHelper.LocationUpdated -= LocationUpdated;
					LocationHelper.LocationUpdated += LocationUpdated;
					RecordPractice(Constants.RECORDING_STATE.START);
					break;
				case Constants.PLAYING_STATE.PLAYING:
					pState = Constants.PLAYING_STATE.PAUSE;
					LocationHelper.StopLocationManager();
					break;
				case Constants.PLAYING_STATE.PAUSE:
					pState = Constants.PLAYING_STATE.PLAYING;
					LocationHelper.StartLocationManager();
                    LocationHelper.LocationUpdated -= LocationUpdated;
					LocationHelper.LocationUpdated += LocationUpdated;
					break;
			}

			SetControlButtons();
		}

		void SetControlButtons()
		{
			switch (pState)
			{
				case Constants.PLAYING_STATE.READY:
                    btnBack.Hidden = false;
                    btnStop.Hidden = true;
					btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_play.png"), UIControlState.Normal);
					break;
				case Constants.PLAYING_STATE.PLAYING:
					btnBack.Hidden = true;
					btnStop.Hidden = false;
					btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_pause.png"), UIControlState.Normal);
					break;
				case Constants.PLAYING_STATE.PAUSE:
					btnBack.Hidden = true;
					btnStop.Hidden = false;
					btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_play.png"), UIControlState.Normal);
					break;
			}
		}

        partial void ActionBack(UIButton sender)
        {
            NavigationController.PopViewController(true);
        }

		partial void ActionStop(UIButton sender)
		{
			ShowMessageBox(null, Constants.MSG_COMFIRM_STOP_SPORT_COMP, "Cancel", new[] { "OK" }, StopPractice);
		}

		void StopPractice()
		{
            RecordPractice(Constants.RECORDING_STATE.END);

            LocationHelper.StopLocationManager();

			pState = Constants.PLAYING_STATE.READY;
			SetControlButtons();

            lblSpeed.Text = "0.0";
            lblAlt.Text = "0.0";
            lblDist.Text = "0.0";
            lblTimer.Text = "00:00:00";

            lastAlt = 0;
            dist = 0;
            gainedAlt = 0;

            duration = 0;
		}

		#region Public Methods

        float lastAlt, dist, gainedAlt;
        int nUpdateCount = 0;

        void LocationUpdated(object sender, EventArgs e)
        {
            CLLocationsUpdatedEventArgs locArgs = e as CLLocationsUpdatedEventArgs;

            _currentLocation = locArgs.Locations[locArgs.Locations.Length - 1];

            if (_currentLocation == null) return;

            SetMapPosition(_currentLocation);

            Console.WriteLine("Location Updated===" + nUpdateCount);

            if (pState != Constants.PLAYING_STATE.PLAYING || (nUpdateCount++) % 2 != 0) return;

            try
            {
                if (_lastLocation != null)
                    dist += (float)_currentLocation.DistanceFrom(_lastLocation) / 1000;

                if (pType == Constants.EVENT_TYPE.BIKE)
                {
					lblSpeed.Text = (_currentLocation.Speed * 3.6f).ToString("0.00");
                }
                if (pType == Constants.EVENT_TYPE.RUN)
                {
                    lblSpeed.Text = (16.6666 / (_currentLocation.Speed)).ToString("0.00");
                }
				if (_currentLocation.Speed < 0.1)
				{
					lblSpeed.Text = "0.00";
				}

				float dAlt = DifAlt(lastAlt, (float)_currentLocation.Altitude);
				if (dAlt < 4) gainedAlt = gainedAlt + dAlt;

				lblAlt.Text = gainedAlt.ToString("0.00");
                lblDist.Text = dist.ToString("0.00");

				_lastLocation = _currentLocation;
				lastAlt = (float)_currentLocation.Altitude;

                RecordPractice(Constants.RECORDING_STATE.RECORDING);
            }
            catch
            {
            }
        }

		void RecordPractice(Constants.RECORDING_STATE recordType)
		{
			var tRecord = new TRecord();

			try
			{
				tRecord.fullName = MemberModel.firstname + " " + MemberModel.lastname; ;
                tRecord.loc = String.Format("{0},{1}", _currentLocation.Coordinate.Latitude, _currentLocation.Coordinate.Longitude); ;
				tRecord.date = DateTime.Now;
				tRecord.deviceId = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
				tRecord.athid = GetUserID();
				tRecord.country = MemberModel.country;
				tRecord.distance = dist;
				tRecord.speed = float.Parse(_currentLocation.Speed.ToString()) * 3.6f;
				tRecord.gainedAlt = gainedAlt;
				tRecord.bearinng = (float)_currentLocation.Course;
				tRecord.recordType = recordType;
				tRecord.sportType = pType;

				RecordPracticeTrack(tRecord);
			}
			catch
			{
			}
		}

		#endregion

		void SetMapPosition(CLLocation location)
		{
			try
			{
				if (mMapView == null) return;

                if (location.Course == -1)
					mMapView.Animate(CameraPosition.FromCamera(location.Coordinate.Latitude, location.Coordinate.Longitude, Constants.MAP_ZOOM_LEVEL));
				else
					mMapView.Animate(CameraPosition.FromCamera(location.Coordinate.Latitude, location.Coordinate.Longitude, Constants.MAP_ZOOM_LEVEL, location.Course, 0));

				if (markerMyLocation != null)
					markerMyLocation.Position = new CLLocationCoordinate2D(location.Coordinate.Latitude, location.Coordinate.Longitude);
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
			}
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
			if (pState == Constants.PLAYING_STATE.PLAYING) duration++;

            InvokeOnMainThread(() =>
            {
                lblTimer.Text = TimeSpan.FromSeconds(duration).ToString(@"hh\:mm\:ss");
            });
		}
	}
}

