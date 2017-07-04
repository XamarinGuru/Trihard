using System.Collections.Generic;

namespace PortableLibrary.Model
{
	public class Lap
	{
        public string name { get; set; }
        public string time { get; set; }
        public string lap { get; set; }
        public string lapKm { get; set; }
        public string TotalTimerTime { get; set; }
        public string elapsedTime { get; set; }
        public string cumulative { get; set; }
        public string avgPace { get; set; }
        public string totalAcent { get; set; }
        public string avgPower { get; set; }
        public string avgHr { get; set; }
        public string avgCadance { get; set; }
        public string AvgCadencePosition { get; set; }
		public float acumPower { get; set; }
        public float AvgFractionalCadence { get; set; }
        public float MaxFractionalCadence { get; set; }
        public float AvgLeftPco { get; set; }
        public float AvgRightPco { get; set; }
        public float LeftRightBalance { get; set; }
        public float MaxCadence { get; set; }
        public float MaxCadencePosition { get; set; }
        public float AvgLeftPowerPhase { get; set; }
        public float AvgRightPowerPhase { get; set; }
        public float AvgPowerPosition { get; set; }
        public float MaxPower { get; set; }
        public float MaxPowerPosition { get; set; }
        public float NormalizedPower { get; set; }
        public string swim_Strock { get; set; }
        public float TrainingStressScore { get; set; }
	}

    public class ReportData
    {
        public int type { get; set; }
        public string eventId { get; set; }
        public string eventSourcId { get; set; }
        public string userId { get; set; }
        public string eventName { get; set; }
        public string commentsId { get; set; }
        public long date { get; set; }
        public List<Item> data { get; set; }
        public List<Lap> lapData { get; set; }

		public string GetTotalValue(string key)
		{
			foreach (var item in data)
			{
				if (item.name == key)
				{
					if (item.value == "-")
					{
						return "0";
					}
					else if (item.value.Split(new char[] { ' ' }).Length == 2)
					{
						return item.value.Split(new char[] { ' ' })[0];
					}
					else
					{
						return item.value;
					}
				}
			}
			return null;
		}
    }
}
