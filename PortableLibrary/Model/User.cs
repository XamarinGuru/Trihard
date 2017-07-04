using System.Collections.Generic;
using USER_TYPE = PortableLibrary.Constants.USER_TYPE;
using OS_TYPE = PortableLibrary.Constants.OS_TYPE;

namespace PortableLibrary
{
	public class User
	{
		public string _id { get; set; }
		public string name { get; set; }
	}

	public class LoginUser
	{
        public LoginUser()
        {
            userType = USER_TYPE.ATHLETE;
            isFcmOn = true;
        }
		public string userId { get; set; }
		public string athleteId { get; set; }
        public USER_TYPE userType { get; set; }
        public string fcmToken { get; set; }
        public bool isFcmOn { get; set; }
        public OS_TYPE osType { get; set; }
		//public bool isFakeUser { get; set; }
	}

	public class Athletes
	{
		public List<Athlete> athlete { get; set; }
	}

    public class EventDoneToday
    {
        public string eventId { get; set; }
        public string eventType { get; set; }
    }
	public class Athlete
	{
		public string _id { get; set; }
		public string name { get; set; }
		public string fields { get; set; }
		public string userImagURI { get; set; }
		public List<EventDoneToday> eventsDoneToday { get; set; }
		public int pmcStatus { get; set; }
	}

	public class AthleteInSubGroup
	{
		public string athleteId { get; set; }
		public string athleteName { get; set; }
		public string userImagURI { get; set; }
		public List<EventDoneToday> eventsDoneToday { get; set; }
		public int pmcStatus { get; set; }
	}
}
