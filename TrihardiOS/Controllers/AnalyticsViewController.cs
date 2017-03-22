using System;
using Foundation;
using UIKit;
using CoreLocation;
using System.Threading.Tasks;
using PortableLibrary;
using Google.Maps;
using System.Drawing;
using CoreGraphics;
using System.Collections.Generic;

namespace location2
{
	public partial class AnalyticsViewController : BaseViewController
	{
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

		MapView mMapView;
		Marker markerMyLocation = null;

		EventPoints mEventMarker = new EventPoints();

		trackSvc.Service1 meServ = new trackSvc.Service1();

		public AnalyticsViewController(IntPtr handle) : base(handle)
		{
			MemberModel = new RootMemberModel();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			InitUISettings();

			if (!IsNetEnable()) return;

			//MemberModel.rootMember = GetUserObject();

			InitMapView();
		}

		void InitUISettings()
		{
			switch (pType)
			{
				case (int)RIDE_TYPE.bike:
					speedTypeLbl.Text = "km/h";
					imgTypeIcon.Image = UIImage.FromBundle("bikeRound_new.png");
					break;
				case (int)RIDE_TYPE.run:
					speedTypeLbl.Text = "min/km";
					imgTypeIcon.Image = UIImage.FromBundle("runRound_new.png");
					break;
				case (int)RIDE_TYPE.mountain:
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

			var camera = CameraPosition.FromCamera(myLocation2D, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
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
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(PortableLibrary.Constants.MSG_LOADING_ALL_MARKERS);

				MemberModel.rootMember = GetUserObject();

				mEventMarker = GetNearestEventMarkers(AppSettings.UserID);
				//mEventMarker = GetAllMarkers("58aafae816528b16d898a1f3");

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

		void SetMapPosition(CLLocation location, double bearing = -1)
		{
			try
			{
				if (mMapView == null) return;

				if (bearing == -1)
					mMapView.Animate(CameraPosition.FromCamera(location.Coordinate.Latitude, location.Coordinate.Longitude, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL));
				else
					mMapView.Animate(CameraPosition.FromCamera(location.Coordinate.Latitude, location.Coordinate.Longitude, PortableLibrary.Constants.MAP_ZOOM_LEVEL, bearing, 0));

				if (markerMyLocation != null)
					markerMyLocation.Position = new CLLocationCoordinate2D(location.Coordinate.Latitude, location.Coordinate.Longitude);
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
			}
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



		#region Public Methods
		CLLocation _lastLocation = null;
		double _currentDistance = 0;
		Double _lastAltitude = 0;
		DateTime _dt;
		double _speed = 0;
		float currdistance = 0;

		int count = 0;

		void LocationUpdated(object sender, EventArgs e)
		{
			count++;
			Console.WriteLine("location===" + count + "\n");
			CLLocationsUpdatedEventArgs locArgs = e as CLLocationsUpdatedEventArgs;
			var location = locArgs.Locations[locArgs.Locations.Length - 1];

			try
			{
				if (location != null & _lastLocation != null)
				{
					_currentDistance = _currentDistance + location.DistanceFrom(_lastLocation) / 1000;
				}
				_lastAltitude = NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude") + Calculate.difAlt(_lastAltitude, location.Altitude);
			}
			catch
			{
			}

			_dt = DateTime.Now;

			if (pType == (int)RIDE_TYPE.bike)
			{
				_speed = location.Speed * 3.6;
			}
			if (pType == (int)RIDE_TYPE.run)
			{
				if (location.Speed > 0)
					_speed = 16.6666 / location.Speed;
				else
					_speed = 0;
			}
			float course = float.Parse(location.Course.ToString());

			currdistance = float.Parse(_currentDistance.ToString());
			float currAlt = float.Parse(_lastAltitude.ToString());
			float currspeed = float.Parse(_speed.ToString());

			try
			{
				var name = MemberModel.firstname + " " + MemberModel.lastname;
				var loc = location.Coordinate.Latitude.ToString() + "," + location.Coordinate.Longitude.ToString();
				var country = MemberModel.country;

				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					meServ.updateMomgoData(name, loc, _dt, true, AppSettings.DeviceUDID, currspeed, true, AppSettings.UserID, country, currdistance, true, currAlt, true, course, true, 0, true, pType.ToString(), PortableLibrary.Constants.SPEC_GROUP_TYPE);
					Console.Write("location update !!! " + name + "===" + loc + "===" + _dt.ToString() + "\n");
				});

				if (currspeed < 0)
					currspeed = 0;
				lblSpeed.Text = currspeed.ToString("0.00");
				lblAlt.Text = currAlt.ToString("0.00");
				lblDist.Text = _currentDistance.ToString("0.00");

				SetMapPosition(location, course);

				_lastLocation = location;
				NSUserDefaults.StandardUserDefaults.SetDouble(_currentDistance, "lastDistance");
				NSUserDefaults.StandardUserDefaults.SetDouble(currAlt, "lastAltitude");
			}
			catch
			{
			}
		}


		#endregion
		private int _duration = 0;

		async Task StartTimer()
		{
			_duration = 0;
			while (true)
			{
				await Task.Delay(1000);
				if (pState == PRACTICE_STATE.playing) _duration++;
				NSUserDefaults.StandardUserDefaults.SetInt(_duration, "timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");

				lblTimer.Text = s;

				Console.Write("duration===" + _duration + "\n");
			}
		}

		partial void ActionBack(UIButton sender)
		{
			if (pState == PRACTICE_STATE.ready)
			{
				NavigationController.PopViewController(true);
			}
			else
			{
				ShowMessageBox(null, "You sure you want to stop practice?", "Cancel", new[] { "OK" }, StopPractice);
			}
		}

		partial void ActionStartPause(UIButton sender)
		{
			if (pState == PRACTICE_STATE.ready)
			{
				StartTimer();

				btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_pause.png"), UIControlState.Normal);
				btnStop.Hidden = false;

				LocationHelper.StartLocationManager();
				LocationHelper.LocationUpdated += LocationUpdated;

				pState = PRACTICE_STATE.playing;

				try
				{
					var name = MemberModel.firstname + " " + MemberModel.lastname;
					var location = _lastLocation.Coordinate.Latitude.ToString() + "," + _lastLocation.Coordinate.Longitude.ToString();
					var speed = float.Parse(_lastLocation.Speed.ToString());
					var alt = float.Parse(NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude").ToString());
					var bearing = float.Parse(_lastLocation.Course.ToString());

					System.Threading.ThreadPool.QueueUserWorkItem(delegate
					{
						meServ.updateMomgoData(name, location, _dt, true, AppSettings.DeviceUDID, speed, true, AppSettings.UserID, MemberModel.country, currdistance, true, alt, true, bearing, true, 1, true, pType.ToString(), PortableLibrary.Constants.SPEC_GROUP_TYPE);
					});
				}
				catch
				{
				}
			}
			else if (pState == PRACTICE_STATE.playing)
			{
				btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_resume.png"), UIControlState.Normal);
				btnStop.Hidden = false;

				LocationHelper.StopLocationManager();

				pState = PRACTICE_STATE.pause;
			}
			else if (pState == PRACTICE_STATE.pause)
			{
				btnStartPause.SetBackgroundImage(UIImage.FromFile("icon_pause.png"), UIControlState.Normal);
				btnStop.Hidden = false;

				LocationHelper.StartLocationManager();
				LocationHelper.LocationUpdated += LocationUpdated;

				pState = PRACTICE_STATE.playing;
			}
		}

		partial void ActionStop(UIButton sender)
		{
			StopPractice();
		}

		void StopPractice()
		{
			try
			{
				var name = MemberModel.firstname + " " + MemberModel.lastname;
				var location = _lastLocation.Coordinate.Latitude.ToString() + "," + _lastLocation.Coordinate.Longitude.ToString();
				var speed = float.Parse(_lastLocation.Speed.ToString());
				var alt = float.Parse(NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude").ToString());
				var bearing = float.Parse(_lastLocation.Course.ToString());

				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					meServ.updateMomgoData(name, location, _dt, true, AppSettings.DeviceUDID, speed, true, AppSettings.UserID, MemberModel.country, currdistance, true, alt, true, bearing, true, 2, true, pType.ToString(), PortableLibrary.Constants.SPEC_GROUP_TYPE);
				});
			}
			catch
			{
			}

			LocationHelper.StopLocationManager();

			NSUserDefaults.StandardUserDefaults.SetInt(0, "timer");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastDistance");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastAltitude");

			NavigationController.PopViewController(true);
		}
	}
}

