using System;
using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using PortableLibrary;

namespace goheja
{
    class util
    {
        public static string getDirection(float degree)
        {
            string direction = "";
            if (0 < degree && degree < 22) return "N";
            if (22 < degree && degree < 47) return "NE";
            if (47 < degree && degree < 112) return "E";
            if (112 < degree && degree < 157) return "SE";
            if (157 < degree && degree < 202) return "S";
            if (202 < degree && degree < 247) return "SW";
            if (247 < degree && degree < 292) return "W";
            if (292 < degree && degree < 337) return "NW";
            if (337 < degree && degree < 359) return "N";

            return direction;
        }
    }
	public static class imageMan
	{
		public static Android.Graphics.Bitmap getPersonalImage()
		{
			var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
			var filePath = System.IO.Path.Combine(sdCardPath, Constants.PATH_USER_IMAGE);
			var s2 = new FileStream(filePath, FileMode.Open);

			try
			{
				Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
				return  bitmap2;
			}
			catch
			{
				return null;
			}
			finally 
			{
				s2.Close();
			}
		}
	}
	public class record
	{
		public string fullName="";
		public double lati = 0;
		public double longti = 0;
		public DateTime date=DateTime.Now;
		public string deviceId = "";
		public string athid="";
		public string country="";
		public float distance = 0f;
		public float speed=0f;
		public float gainedAlt = 0f;
		public float bearinng = 0f;
		public int recordType = 0;
		public string sportType="";

		public record (string fitslast,double latid,double longtid,DateTime recordTimeStamp,string devId,string athletId,string country,float distance,float speed,float cumAlt,float bearing,int recodType,string sportType)
		{
			this.fullName = fitslast;
			this.lati = latid;
			this.longti = longtid;
			this.date = recordTimeStamp;
			this.deviceId = devId;
			this.athid = athletId;
			this.country = country;
			this.distance = distance;
			this.speed = speed;
			this.gainedAlt = cumAlt;
			this.bearinng = bearing;
			this.recordType = recodType;
			this.sportType = sportType;
		}
	}
	public class handleRecord
	{
		List<record> offlineRecords =new List<record>();
		trackSvc.Service1 svc = new trackSvc.Service1();

		public string updaterecord (record theRecord,bool isConnected)
		{
			string status = "";

			if (isConnected) 
			{
				if (offlineRecords.Count > 0) {
					dumpOffLineRecords ();
					status = "backFromOffline";
				}
				   
				try{
					svc.updateMomgoData (theRecord.fullName, String.Format ("{0},{1}",theRecord.lati,theRecord.longti), theRecord.date, true,theRecord.deviceId, theRecord.speed, true,theRecord.athid, theRecord.country, theRecord.distance, true,theRecord.gainedAlt,true, theRecord.bearinng, true, theRecord.recordType, true,theRecord.sportType, Constants.SPEC_GROUP_TYPE);
				}
				catch
				{
					return "offline";
				}

				return status;
			}
			else
			{
				offlineRecords.Add (theRecord);
				return "offline";
			}
		}
		private void  dumpOffLineRecords()
		{
			foreach (record r in offlineRecords) 
			{
				try
				{
					svc.updateMomgoData(r.fullName, String.Format("{0},{1}", r.lati, r.longti), r.date, true, r.deviceId, r.speed, true, r.athid, r.country, r.distance, true, r.gainedAlt, true, r.bearinng, true, r.recordType, true, r.sportType, Constants.SPEC_GROUP_TYPE);
				}
				catch
				{
				}
			}
			 offlineRecords.Clear ();
		}
	}
}