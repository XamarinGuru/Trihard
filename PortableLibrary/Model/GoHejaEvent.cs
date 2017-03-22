using System;

namespace PortableLibrary
{
	public class GoHejaEvent
	{
		public string _id { get; set;}
		public string title { get; set; }
		public string eventSysId { get; set; }
		public string userId { get; set; }
		public string ParentGroupName { get; set; }
		public string ParentGroupId { get; set; }
		public string GroupId { get; set; }
		public string GroupName { get; set; }
		public string start { get; set; }
		public string end { get; set; }
		public string eventColor { get; set; }
		public string eventData { get; set; }
		public string distance { get; set; }
		public string type { get; set; }
		public string notes { get; set; }
		public string attended { get; set; }
		public string programName { get; set; }
		public string programStart { get; set; }
		public string programEnd { get; set; }
		public string durHrs { get; set; }
		public string durMin { get; set; }
		public string tss { get; set; }
		public string hb { get; set; }
		public string pace { get; set; }

		public DateTime StartDateTime()
		{
			return Convert.ToDateTime(start).ToUniversalTime();
		}
		public DateTime EndDateTime()
		{
			return Convert.ToDateTime(end).ToUniversalTime();
		}
	}
}
