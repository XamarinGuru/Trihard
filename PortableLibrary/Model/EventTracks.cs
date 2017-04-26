using System;
using System.Collections.Generic;

namespace PortableLibrary
{
	public class TPoint
	{
		public string Altitude { get; set; }
		public string Bearing { get; set; }
		public string EHPE { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string Speed { get; set; }
		public string lapNo { get; set; }
		public string lapImage { get; set; }
		public string LocalTime { get; set;}
		public DateTime index { get; set;}
	}

	public class EventTracks
	{
		//public string CompressedTrackPoints { get; set; }
		//public string Points { get; set;}
		public string eventId { get; set;}
		public string userId { get; set; }
		public string eventSourceId { get; set;}
		//public int eventCompId { get; set;}
		public List<TPoint> TrackPoints { get; set; }
	}
}
