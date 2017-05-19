using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Locations;

namespace goheja.Services
{
	[Service]
	public class LocationService : Service, ILocationListener
	{
		public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
		public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
		public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
		public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

		public LocationService() 
		{
		}

		// Set our location manager as the system location service
		protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService ("location") as LocationManager;

		IBinder binder;

		public override void OnCreate ()
		{
			base.OnCreate ();
		}

		// This gets called when StartService is called in our App class
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			return StartCommandResult.Sticky;
		}

		// This gets called once, the first time any client bind to the Service
		// and returns an instance of the LocationServiceBinder. All future clients will
		// reuse the same instance of the binder
		public override IBinder OnBind (Intent intent)
		{
			binder = new LocationServiceBinder (this);
			return binder;
		}

		// Handle location updates from the location manager
		public void StartLocationUpdates () 
		{
			var locationCriteria = new Criteria();

			locationCriteria.VerticalAccuracy = Accuracy.High;
			locationCriteria.SpeedAccuracy = Accuracy.High;
			locationCriteria.HorizontalAccuracy = Accuracy.High;
			locationCriteria.AltitudeRequired = true;
			locationCriteria.PowerRequirement = Power.High;
			locationCriteria.SpeedRequired = true;

			var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
			LocMgr.RequestLocationUpdates(locationProvider, 1000,1, this);
		}

		public void StopLocationUpdates()
		{
			LocMgr.RemoveUpdates(this);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		#region ILocationListener implementation

		public void OnLocationChanged (Android.Locations.Location location)
		{
			this.LocationChanged (this, new LocationChangedEventArgs (location));
		}

		public void OnProviderDisabled (string provider)
		{
			this.ProviderDisabled (this, new ProviderDisabledEventArgs (provider));
		}

		public void OnProviderEnabled (string provider)
		{
			this.ProviderEnabled (this, new ProviderEnabledEventArgs (provider));
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			this.StatusChanged (this, new StatusChangedEventArgs (provider, status, extras));
		} 

		#endregion
	}
}

