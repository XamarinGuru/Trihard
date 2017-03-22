using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using goheja.Services;

/// <summary>
/// Singleton class for Application wide objects. 
/// </summary>
namespace goheja
{
	public class App
	{
		public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate {};
		
		protected readonly string logTag = "App";
        public LocationServiceConnection locationServiceConnection; //added by Afroz date 2/9/2016

        public static App Current
		{
			get { return current; }
		} 
		private static App current;
		
		public LocationService LocationService
		{
			get {
				if (this.locationServiceConnection.Binder == null)
					throw new Exception ("Service not bound yet");
				// note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
				return this.locationServiceConnection.Binder.Service;
			}
		}

		#region Application context

		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			new Task ( () => { 
				
				Log.Debug (logTag, "Calling StartService");
				Android.App.Application.Context.StartService (new Intent (Android.App.Application.Context, typeof(LocationService)));
				this.locationServiceConnection = new LocationServiceConnection (null);

				this.locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
					Log.Debug (logTag, "Service Connected");
					this.LocationServiceConnected ( this, e );
				};

                Intent locationServiceIntent = new Intent (Android.App.Application.Context, typeof(LocationService));
				Log.Debug (logTag, "Calling service binding");
				Android.App.Application.Context.BindService (locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
			} ).Start ();
		}
		#endregion
	}
}


