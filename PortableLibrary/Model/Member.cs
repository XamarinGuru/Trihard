using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentValidation;

namespace PortableLibrary
{
	public class RootObject : AbstractValidator<RootMember>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}


	public class IdData
	{
		public IdData()
		{
			_id = "";
			name = "";
			value = "";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
	public class userSystemData
	{
		public userSystemData()
		{
			_id = "";
			name = "";
			value = "";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
	public class athHistory
	{
		public athHistory()
		{
			_id = "";
			date = new DateTime().ToString();
			text = "";
		}
		public string _id { get; set; }
		public string date { get; set; }
		public string text { get; set; }
	}
	public class Performance
	{
		public Performance()
		{
			_id = "";
			name = "";
			value = "0";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
	public class Physical
	{
		public Physical()
		{
			_id = "";
			name = "";
			value = "";
			unit = "";
			unit = "";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
		public string unit { get; set; }
	}
	public class BestResults
	{
		public BestResults()
		{
			_id = "";
			name = "";
			value = "";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
	public class Experience
	{
		public Experience()
		{
			_id = "";
			name = "";
			value = "";

		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
	public class SelfRanking
	{
		public SelfRanking()
		{
			fieldId = "";
			fieldName = "";
			rank = "";
		}
		public string fieldId { get; set; }
		public string fieldName { get; set; }
		public string rank { get; set; }
	}
	public class athGoals
	{
		public athGoals()
		{
			_id = "";
			Date = DateTime.Now.ToString();
			Name = "";
			Load = 0;
		}
		public string _id { get; set; }
		public string Date { get; set; }
		public string Name { get; set; }
		public float Load { get; set; }
	}

	public class eventProp
	{
		public eventProp()
		{
			_id = 0;
			name = "";
			value = "";
		}
		public int _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}

	public class Event
	{
		public Event()
		{
			title = "";
			eventId = "";
			start = 0d;
			end = 0d;
			isDone = "";
			isComments = "";
			color = "#111111";
			tss = 0f;
			hb = 0;
			type = "";
			duration = "00:00";
			practiceProp = new List<eventProp>();
		}
		public string title { get; set; }
		public string eventId { get; set; }
		public double start { get; set; }
		public double end { get; set; }
		public string isDone { get; set; }
		public string isComments { get; set; }
		public string color { get; set; }
		public float tss { get; set; }
		public List<eventProp> practiceProp { get; set; }
		public int hb { get; set; }
		public string type { get; set; }
		public string duration { get; set; }
	}

	public class ZoneLevel
	{
		public ZoneLevel()
		{
			_id = "";
			name = "";
			hr = "70";
			pace = "";
			power = "";
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string hr { get; set; }
		public string pace { get; set; }
		public string power { get; set; }
	}

	public class Injuries
	{
		public Injuries()
		{
			_id = "";
			name = "";
			value = "";
			isCurrent = false;
			date = new DateTime().ToString();
		}
		public string _id { get; set; }
		public string name { get; set; }
		public string value { get; set; }
		public bool isCurrent { get; set; }
		public string date { get; set; }
	}

	public class discipZones
	{
		public discipZones()
		{
			fieldId = "";
			fieldName = "";
			ZoneLevel = new List<ZoneLevel>();
			FTPACE = "";
			FTPWATT = "";
			FTPHB = "";
		}
		public string fieldId { get; set; }
		public string fieldName { get; set; }
		public List<ZoneLevel> ZoneLevel { get; set; }
		public string FTPACE { get; set; }
		public string FTPWATT { get; set; }
		public string FTPHB { get; set; }
	}

	public class Profile
	{
		public Profile()
		{

			idData = new List<IdData>();
			userSystemData = new List<userSystemData>();

			injuries = new List<Injuries>();
			//injuries.Add(inj());

			performance = new List<Performance>();
			//performance.Add(acl());
			//performance.Add(atl());
			//performance.Add(tsb());
			//performance.Add(tss());

			physical = new List<Physical>();
			//physical.Add(weight());
			//physical.Add(height());
			//physical.Add(bmi());
			//physical.Add(fper());

			goals = new List<athGoals>();
			//goals.Add(goal());

			history = new List<athHistory>();
			//history.Add(historyEvent());

			bestResults = new List<BestResults>();

			experience = new List<Experience>();
			//experience.Add(expSwim());
			//experience.Add(expRun());
			//experience.Add(expBike());
			//experience.Add(expEndur());

			selfRanking = new List<SelfRanking>();
			//selfRanking.Add(srSwim());
			//selfRanking.Add(srRun());
			//selfRanking.Add(srBike( ));

			events = new List<Event>();
			fields = new List<discipZones>();
		}
		public List<IdData> idData { get; set; }
		public List<userSystemData> userSystemData { get; set; }
		public List<Injuries> injuries { get; set; }
		public List<athHistory> history { get; set; }
		public List<Performance> performance { get; set; }
		public List<Physical> physical { get; set; }
		public List<athGoals> goals { get; set; }
		public List<BestResults> bestResults { get; set; }
		public List<Experience> experience { get; set; }
		public List<SelfRanking> selfRanking { get; set; }
		public List<Event> events { get; set; }
		public List<discipZones> fields { get; set; }

		private Injuries inj()
		{
			Injuries inj = new Injuries();
			inj._id = "0";
			inj.name = "";
			inj.value = "";
			return inj;
		}

		private Performance acl()
		{
			Performance p = new Performance();
			p._id = "0";
			p.name = "ACL";
			p.value = "";
			return p;
		}
		private Performance atl()
		{
			Performance p = new Performance();
			p._id = "1";
			p.name = "ATL";
			p.value = "";
			return p;
		}
		private Performance tsb()
		{
			Performance p = new Performance();
			p._id = "2";
			p.name = "TSB";
			p.value = "";
			return p;
		}
		private Performance tss()
		{
			Performance p = new Performance();
			p._id = "3";
			p.name = "TSS";
			p.value = "";
			return p;
		}
		private Physical weight()
		{
			Physical p = new Physical();
			p._id = "0";
			p.name = "Weight";
			p.value = "600";
			return p;
		}
		private Physical height()
		{
			Physical p = new Physical();
			p._id = "1";
			p.name = "Height";
			p.value = "175";
			return p;
		}
		private Physical bmi()
		{
			Physical p = new Physical();
			p._id = "2";
			p.name = "BMI";
			p.value = "23";
			return p;
		}
		private Physical fper()
		{
			Physical p = new Physical();
			p._id = "3";
			p.name = "Fat percentage";
			p.value = "33";
			return p;
		}
		private Physical fp()
		{
			Physical p = new Physical();
			p._id = "3";
			p.name = "Fat percentage";
			p.value = "";
			return p;
		}
		private athGoals goal()
		{
			athGoals p = new athGoals();
			p._id = "0";
			p.Name = DateTime.Now.Year.ToString();
			p.Date = "";
			p.Load = 0;
			return p;
		}
		private athHistory historyEvent()
		{
			athHistory p = new athHistory();
			p._id = "0";
			p.date = new DateTime().ToString();
			p.text = "";
			return p;
		}
		private Experience expSwim()
		{
			Experience p = new Experience();
			p._id = "0";
			p.name = "Swim";
			p.value = "";
			return p;
		}
		private Experience expBike()
		{
			Experience p = new Experience();
			p._id = "1";
			p.name = "Bike";
			p.value = "";
			return p;
		}
		private Experience expRun()
		{
			Experience p = new Experience();
			p._id = "2";
			p.name = "Run";
			p.value = "";
			return p;
		}
		private Experience expEndu()
		{
			Experience p = new Experience();
			p._id = "3";
			p.name = "Endurance";
			p.value = "";
			return p;
		}
		private Experience expEndur()
		{
			Experience p = new Experience();
			p._id = "3";
			p.name = "Endurance";
			p.value = "";
			return p;
		}

		private SelfRanking srSwim()
		{
			SelfRanking p = new SelfRanking();
			p.fieldId = "3";
			p.fieldName = "Swim";
			p.rank = "1";
			return p;
		}
		private SelfRanking srBike()
		{
			SelfRanking p = new SelfRanking();
			p.fieldId = "1";
			p.fieldName = "Bike";
			p.rank = "1";
			return p;
		}
		private SelfRanking srRun()
		{
			SelfRanking p = new SelfRanking();
			p.fieldId = "2";
			p.fieldName = "Run";
			p.rank = "1";
			return p;
		}
	}

	public class RootMemberModel : RootObject
	{
		public RootMemberModel()
		{
			rootMember = new RootMember();
		}
		public RootMember rootMember { get; set; }

		//user info
		public string firstname { get { return rootMember.profile.idData[0].value; } set { rootMember.profile.idData[0].value = value; } }
		public string lastname { get { return rootMember.profile.idData[1].value; } set { rootMember.profile.idData[1].value = value; } }
		public string username { get { return rootMember.userName; } set { rootMember.userName = value; } }
		public string password { get { return rootMember.password; } set { rootMember.password = value; } }

		public string country { get { return rootMember.profile.idData[2].value; } set { rootMember.profile.idData[2].value = value; } }
		public string address { get { return rootMember.profile.idData[3].value; } set { rootMember.profile.idData[3].value = value; } }
		public string bib { get { return rootMember.profile.idData[4].value; } set { rootMember.profile.idData[4].value = value; } }
		public string age { get { return rootMember.profile.idData[5].value; } set { rootMember.profile.idData[5].value = value; } }
		public string gender { get { return rootMember.profile.idData[6].value; } set { rootMember.profile.idData[6].value = value; } }
		public string birth { get { return rootMember.profile.idData[7].value; } set { rootMember.profile.idData[7].value = value; } }
		public string email { get { return rootMember.profile.idData[8].value; } set { rootMember.profile.idData[8].value = value; } }
		public string phone { get { return rootMember.profile.idData[9].value; } set { rootMember.profile.idData[9].value = value; } }

		//physical
		public string weight { get { return rootMember.profile.physical[0].value; } set { rootMember.profile.physical[0].value = value; } }
		public string height { get { return rootMember.profile.physical[1].value; } set { rootMember.profile.physical[1].value = value; } }
		public string bmi { get { return rootMember.profile.physical[2].value; } set { rootMember.profile.physical[2].value = value; } }
		public string fper { get { return rootMember.profile.physical[3].value; } set { rootMember.profile.physical[3].value = value; } }

		//goals
		public string id { get { return rootMember.profile.goals[0]._id; } set { rootMember.profile.goals[0]._id = value; } }
		public string goalDate { get { return rootMember.profile.goals[0].Date; } set { rootMember.profile.goals[0].Date = value; } }
		public string goalName { get { return rootMember.profile.goals[0].Name; } set { rootMember.profile.goals[0].Name = value; } }
		public float goalLoad { get { return rootMember.profile.goals[0].Load; } set { rootMember.profile.goals[0].Load = value; } }

		//best results
		public string sprint { get { return rootMember.profile.bestResults[0].value; } set { rootMember.profile.bestResults[0].value = value; } }
		public string olympic { get { return rootMember.profile.bestResults[1].value; } set { rootMember.profile.bestResults[1].value = value; } }
		public string hdistance { get { return rootMember.profile.bestResults[2].value; } set { rootMember.profile.bestResults[2].value = value; } }
		public string fdistance { get { return rootMember.profile.bestResults[3].value; } set { rootMember.profile.bestResults[3].value = value; } }
		public string krun { get { return rootMember.profile.bestResults[4].value; } set { rootMember.profile.bestResults[4].value = value; } }
		public string hmarathon { get { return rootMember.profile.bestResults[5].value; } set { rootMember.profile.bestResults[5].value = value; } }
		public string fmarathon { get { return rootMember.profile.bestResults[6].value; } set { rootMember.profile.bestResults[6].value = value; } }

		//self ranking
		public string srSwim { get { return rootMember.profile.selfRanking[0].rank; } set { rootMember.profile.selfRanking[0].rank = value; } }
		public string srRun { get { return rootMember.profile.selfRanking[1].rank; } set { rootMember.profile.selfRanking[1].rank = value; } }
		public string srBike { get { return rootMember.profile.selfRanking[2].rank; } set { rootMember.profile.selfRanking[2].rank = value; } }

		//experience swim
		public string sZone1HR { get { return rootMember.profile.fields[0].ZoneLevel[0].hr; } set { rootMember.profile.fields[0].ZoneLevel[0].hr = value; } }
		public string sZone2HR { get { return rootMember.profile.fields[0].ZoneLevel[1].hr; } set { rootMember.profile.fields[0].ZoneLevel[1].hr = value; } }
		public string sZone3HR { get { return rootMember.profile.fields[0].ZoneLevel[2].hr; } set { rootMember.profile.fields[0].ZoneLevel[2].hr = value; } }
		public string sZone4HR { get { return rootMember.profile.fields[0].ZoneLevel[3].hr; } set { rootMember.profile.fields[0].ZoneLevel[3].hr = value; } }
		public string sZone5HR { get { return rootMember.profile.fields[0].ZoneLevel[4].hr; } set { rootMember.profile.fields[0].ZoneLevel[4].hr = value; } }

		public string sZone1PACE { get { return rootMember.profile.fields[0].ZoneLevel[0].pace; } set { rootMember.profile.fields[0].ZoneLevel[0].pace = value; } }
		public string sZone2PACE { get { return rootMember.profile.fields[0].ZoneLevel[1].pace; } set { rootMember.profile.fields[0].ZoneLevel[1].pace = value; } }
		public string sZone3PACE { get { return rootMember.profile.fields[0].ZoneLevel[2].pace; } set { rootMember.profile.fields[0].ZoneLevel[2].pace = value; } }
		public string sZone4PACE { get { return rootMember.profile.fields[0].ZoneLevel[3].pace; } set { rootMember.profile.fields[0].ZoneLevel[3].pace = value; } }
		public string sZone5PACE { get { return rootMember.profile.fields[0].ZoneLevel[4].pace; } set { rootMember.profile.fields[0].ZoneLevel[4].pace = value; } }

		public string sFTPace { get { return rootMember.profile.fields[0].FTPACE; } set { rootMember.profile.fields[0].FTPACE = value; } }
		public string sFTPHB { get { return rootMember.profile.fields[0].FTPHB; } set { rootMember.profile.fields[0].FTPHB = value; } }

		//experience run
		public string rZone1HR { get { return rootMember.profile.fields[2].ZoneLevel[0].hr; } set { rootMember.profile.fields[2].ZoneLevel[0].hr = value; } }
		public string rZone2HR { get { return rootMember.profile.fields[2].ZoneLevel[1].hr; } set { rootMember.profile.fields[2].ZoneLevel[1].hr = value; } }
		public string rZone3HR { get { return rootMember.profile.fields[2].ZoneLevel[2].hr; } set { rootMember.profile.fields[2].ZoneLevel[2].hr = value; } }
		public string rZone4HR { get { return rootMember.profile.fields[2].ZoneLevel[3].hr; } set { rootMember.profile.fields[2].ZoneLevel[3].hr = value; } }
		public string rZone5HR { get { return rootMember.profile.fields[2].ZoneLevel[4].hr; } set { rootMember.profile.fields[2].ZoneLevel[4].hr = value; } }

		public string rZone1PACE { get { return rootMember.profile.fields[2].ZoneLevel[0].pace; } set { rootMember.profile.fields[2].ZoneLevel[0].pace = value; } }
		public string rZone2PACE { get { return rootMember.profile.fields[2].ZoneLevel[1].pace; } set { rootMember.profile.fields[2].ZoneLevel[1].pace = value; } }
		public string rZone3PACE { get { return rootMember.profile.fields[2].ZoneLevel[2].pace; } set { rootMember.profile.fields[2].ZoneLevel[2].pace = value; } }
		public string rZone4PACE { get { return rootMember.profile.fields[2].ZoneLevel[3].pace; } set { rootMember.profile.fields[2].ZoneLevel[3].pace = value; } }
		public string rZone5PACE { get { return rootMember.profile.fields[2].ZoneLevel[4].pace; } set { rootMember.profile.fields[2].ZoneLevel[4].pace = value; } }

		public string rZone1POWER { get { return rootMember.profile.fields[2].ZoneLevel[0].power; } set { rootMember.profile.fields[2].ZoneLevel[0].power = value; } }
		public string rZone2POWER { get { return rootMember.profile.fields[2].ZoneLevel[1].power; } set { rootMember.profile.fields[2].ZoneLevel[1].power = value; } }
		public string rZone3POWER { get { return rootMember.profile.fields[2].ZoneLevel[2].power; } set { rootMember.profile.fields[2].ZoneLevel[2].power = value; } }
		public string rZone4POWER { get { return rootMember.profile.fields[2].ZoneLevel[3].power; } set { rootMember.profile.fields[2].ZoneLevel[3].power = value; } }
		public string rZone5POWER { get { return rootMember.profile.fields[2].ZoneLevel[4].power; } set { rootMember.profile.fields[2].ZoneLevel[4].power = value; } }

		public string rFTPace { get { return rootMember.profile.fields[2].FTPACE; } set { rootMember.profile.fields[2].FTPACE = value; } }
		public string rFTPHB { get { return rootMember.profile.fields[2].FTPHB; } set { rootMember.profile.fields[2].FTPHB = value; } }
		public string rFTPower { get { return rootMember.profile.fields[2].FTPWATT; } set { rootMember.profile.fields[2].FTPWATT = value; } }

		//experience bike
		public string bZone1HR { get { return rootMember.profile.fields[1].ZoneLevel[0].hr; } set { rootMember.profile.fields[1].ZoneLevel[0].hr = value; } }
		public string bZone2HR { get { return rootMember.profile.fields[1].ZoneLevel[1].hr; } set { rootMember.profile.fields[1].ZoneLevel[1].hr = value; } }
		public string bZone3HR { get { return rootMember.profile.fields[1].ZoneLevel[2].hr; } set { rootMember.profile.fields[1].ZoneLevel[2].hr = value; } }
		public string bZone4HR { get { return rootMember.profile.fields[1].ZoneLevel[3].hr; } set { rootMember.profile.fields[1].ZoneLevel[3].hr = value; } }
		public string bZone5HR { get { return rootMember.profile.fields[1].ZoneLevel[4].hr; } set { rootMember.profile.fields[1].ZoneLevel[4].hr = value; } }

		public string bZone1POWER { get { return rootMember.profile.fields[1].ZoneLevel[0].power; } set { rootMember.profile.fields[1].ZoneLevel[0].power = value; } }
		public string bZone2POWER { get { return rootMember.profile.fields[1].ZoneLevel[1].power; } set { rootMember.profile.fields[1].ZoneLevel[1].power = value; } }
		public string bZone3POWER { get { return rootMember.profile.fields[1].ZoneLevel[2].power; } set { rootMember.profile.fields[1].ZoneLevel[2].power = value; } }
		public string bZone4POWER { get { return rootMember.profile.fields[1].ZoneLevel[3].power; } set { rootMember.profile.fields[1].ZoneLevel[3].power = value; } }
		public string bZone5POWER { get { return rootMember.profile.fields[1].ZoneLevel[4].power; } set { rootMember.profile.fields[1].ZoneLevel[4].power = value; } }

		public string bFTPHB { get { return rootMember.profile.fields[1].FTPHB; } set { rootMember.profile.fields[1].FTPHB = value; } }
		public string bFTPower { get { return rootMember.profile.fields[1].FTPWATT; } set { rootMember.profile.fields[1].FTPWATT = value; } }
	}

	public class RootMember
	{
		public RootMember()
		{
			profile = new Profile();
			auth = "";
			userName = "";
			password = "";
			sportComp = 0;
			sportCompKey = "";
		}
		public Profile profile { get; set; }
		public string auth { get; set; }
		public string userName { get; set; }
		public string password { get; set; }
		public int sportComp { get; set; }
		public string sportCompKey { get; set; }
	}
}

