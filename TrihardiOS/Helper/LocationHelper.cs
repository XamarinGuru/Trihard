using System;
using CoreLocation;
using ObjCRuntime;

namespace location2
{
	public class LocationHelper : IDisposable
	{
		private static bool _isTracking;
		public static bool IsTracking { get { return _isTracking; } }
		private static double _longitude;
		private static double _latitude;
		private static DateTime _lastUpdated;

		public static event EventHandler LocationUpdated;

		public static CLLocationManager LocationManager { private set; get; }

		static LocationHelper()
		{
			LocationManager = new CLLocationManager();

			if (LocationManager.RespondsToSelector(new Selector("requestWhenInUseAuthorization")))
			{
				LocationManager.RequestWhenInUseAuthorization();
				//LocationManager.RequestAlwaysAuthorization();
			}
			LocationManager.DistanceFilter = CLLocationDistance.FilterNone;
			LocationManager.AllowsBackgroundLocationUpdates = true;
			LocationManager.PausesLocationUpdatesAutomatically = false;
			LocationManager.LocationsUpdated += LocationManager_FirstLocationsUpdated;
			LocationManager.StartUpdatingLocation();
			_isTracking = true;
		}

		#region IDisposable implementation
		public void Dispose()
		{
			try
			{
				if (LocationManager != null)
				{
					LocationManager.StopUpdatingLocation();
					LocationManager.Dispose();
					LocationManager = null;
				}
			}
			finally
			{

			}
		}
		#endregion

		public static void StartLocationManager()
		{
			if (LocationManager != null)
			{
				LocationManager.LocationsUpdated += LocationManager_LocationsUpdated;
				LocationManager.StartUpdatingLocation();
				_isTracking = true;
			}
		}

		public static void StopLocationManager()
		{
			if (LocationManager != null)
			{
				LocationManager.LocationsUpdated -= LocationManager_LocationsUpdated;
				LocationManager.StopUpdatingLocation();
				_isTracking = false;
			}

		}

		public static void Refresh()
		{
			LocationManager.StopUpdatingLocation();
			LocationManager.StartUpdatingLocation();
		}

		private static void LocationManager_FirstLocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
		{
			if (LocationManager != null)
			{
				LocationManager.LocationsUpdated -= LocationManager_FirstLocationsUpdated;
				LocationManager.StopUpdatingLocation();
				_isTracking = false;
			}

			if (LocationUpdated != null)
			{
				LocationUpdated(sender, e);
			}
			UpdateLocation(e.Locations[e.Locations.Length - 1]);
		}

		private static void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
		{
			if (LocationUpdated != null)
			{
				LocationUpdated(sender, e);
			}
			UpdateLocation(e.Locations[e.Locations.Length - 1]);
		}

		private static void UpdateLocation(CLLocation location)
		{
			_longitude = location.Coordinate.Longitude;
			_latitude = location.Coordinate.Latitude;
			_lastUpdated = DateTime.Now;
		}

		public static LocationResult GetLocationResult()
		{
			return new LocationResult(_latitude, _longitude, _lastUpdated);
		}

		public class LocationResult
		{
			public DateTime UpdatedTime { private set; get; }
			public double Latitude { private set; get; }
			public double Longitude { private set; get; }

			public LocationResult(double latitude, double longitude, DateTime updated)
			{
				UpdatedTime = updated;
				Latitude = latitude;
				Longitude = longitude;
			}
		}
	}
}
