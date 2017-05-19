using System;
using System.Collections.Generic;

namespace PortableLibrary
{
	public class User
	{
		public string _id { get; set; }
		public string name { get; set; }
	}

	public class LoginUser
	{
		public string userId { get; set; }
		public string athleteId { get; set; }
		public int userType { get; set; }
		//public bool isFakeUser { get; set; }
	}

	public class Athletes
	{
		public List<Athlete> athlete { get; set; }
	}

	public class Athlete
	{
		public string _id { get; set; }
		public string name { get; set; }
		public string fields { get; set; }
		public string userImagURI { get; set; }
		public string eventsDoneToday { get; set; }
		public int pmcStatus { get; set; }
	}

	public class AthleteInSubGroup
	{
		public string athleteId { get; set; }
		public string athleteName { get; set; }
		public string userImagURI { get; set; }
		public string eventsDoneToday { get; set; }
		public int pmcStatus { get; set; }
	}
}
