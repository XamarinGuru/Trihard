using System;
using Android.OS;

namespace goheja.Services
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}