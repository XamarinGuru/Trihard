using System;

namespace location2
{
	public static class Calculate
	{
		
		public static double distance(double lat1, double lon1, double lat2, double lon2, char unit) {
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;
			if (unit == 'K') {
				dist = dist * 1.609344;
			} else if (unit == 'N') {
				dist = dist * 0.8684;
			}
			return (dist);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts decimal degrees to radians             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts radians to decimal degrees             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}

		public static double difAlt(double prev, double curr)
		{
			try
			{
				if ((curr - prev) > 0)
					return curr - prev;
				else
					return 0;
			}
			catch
			{
				return 0;
			}

		}
		public static string getDirection(Double degree)
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
}

