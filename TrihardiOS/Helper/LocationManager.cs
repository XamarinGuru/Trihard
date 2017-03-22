using System;
using CoreLocation;
using UIKit;

namespace location2
{
	public class LocationManager
	{
		public CLLocationManager locMgr;
		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
		public LocationManager (){
			this.locMgr = new CLLocationManager();
			this.locMgr.PausesLocationUpdatesAutomatically = false; 

			// iOS 8 has additional permissions requirements
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
			//	iPhoneLocationManager.RequestWhenInUseAuthorization ();

				//locMgr.RequestAlwaysAuthorization (); // works in background
				locMgr.RequestWhenInUseAuthorization (); // only in foreground
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				locMgr.AllowsBackgroundLocationUpdates = true;
			}
		}

		public CLLocationManager LocMgr{
			get { return this.locMgr; }
		}
		public   CLLocationManager StartLocationUpdates (){
			if (CLLocationManager.LocationServicesEnabled) {
				//set the desired accuracy, in meters
				LocMgr.DesiredAccuracy = 1;
				LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
				{
					// fire our custom Location Updated event
					LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
				};
				LocMgr.StartUpdatingLocation();
			}
			return this.locMgr;
		}

		public void StopLocationUpdates()
		{
			LocMgr.StopUpdatingLocation();
		}
	}
	public class LocationUpdatedEventArgs : EventArgs
	{
		CLLocation location;

		public LocationUpdatedEventArgs(CLLocation location)
		{
			this.location = location;
		}

		public CLLocation Location
		{
			get { return location; }
		}
	}
}

