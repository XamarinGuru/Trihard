using Foundation;
using System;
using UIKit;
using PortableLibrary.Model;

namespace location2
{
	public partial class LapSwimCell : BaseLapTableViewCell
	{
		public static readonly NSString Key = new NSString("LapSwimCell");
		public static readonly UINib Nib;

		static LapSwimCell()
		{
			Nib = UINib.FromName("LapSwimCell", NSBundle.MainBundle);
		}

		protected LapSwimCell(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void SetCell(Lap lapData)
		{
			lap.Text = lapData.lap;
			lapKm.Text = lapData.lapKm;
			elapsedTime.Text = lapData.elapsedTime;
			avgPace.Text = lapData.avgPace;
			avgCadance.Text = lapData.avgCadance;
			swim_Strock.Text = lapData.swim_Strock;
		}
	}
}