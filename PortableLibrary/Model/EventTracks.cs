using System;
using System.Collections.Generic;

namespace PortableLibrary
{
    public class TRecord
    {
		public string fullName = "";
		public string loc = "0,0";
		public DateTime date = DateTime.Now;
		public string deviceId = "";
		public string athid = "";
		public string country = "";
		public float distance = 0f;
		public float speed = 0f;
		public float gainedAlt = 0f;
		public float bearinng = 0f;
        public Constants.RECORDING_STATE recordType = Constants.RECORDING_STATE.RECORDING;
        public Constants.EVENT_TYPE sportType = Constants.EVENT_TYPE.BIKE;
    }
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
