//using System;
//using System.Collections.Generic;

//namespace location2
//{
//	public sealed class Facade
//	{
//		private static readonly Lazy<Facade> lazy = new Lazy<Facade>(() => new Facade());

//		public static Facade Instance { get { return lazy.Value; } }

//		private Facade()
//		{

//		}

//		private ScheduleARideVM _currentRide = new ScheduleARideVM();
//		public ScheduleARideVM CurrentRide
//		{
//			get
//			{
//				return _currentRide;
//			}
//		}
//	}
//}
