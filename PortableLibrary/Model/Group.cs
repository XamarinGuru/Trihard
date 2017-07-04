using System.Collections.Generic;

namespace PortableLibrary
{
	public class SubGroups
	{
		public string fieldId { get; set; }
		public string fieldName { get; set; }
		public List<Group> groups { get; set; }
	}

	public class Group
	{
		public string groupId { get; set; }
		public string groupName { get; set; }
		public List<AthleteInSubGroup> athletes { get; set; }
		public List<GoHejaEvent> events { get; set; }	
	}
}
