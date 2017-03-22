
using System.Collections.Generic;

namespace PortableLibrary
{
	public class Point
	{
		public string markerId { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public double lat { get; set;}
		public double lng { get; set;}
		public string type { get; set;}
		public string eventid { get; set;}
		public int interval { get; set;}
	}

	public class EventPoints
	{
		public string eventid { get; set; }
		public List<Point> markers { get; set; }
	}
}
