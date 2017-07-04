using Foundation;
using System;
using UIKit;
using PortableLibrary.Model;

namespace location2
{
	public partial class LapTriathlonCell : BaseLapTableViewCell
	{
		public static readonly NSString Key = new NSString("LapTriathlonCell");
		public static readonly UINib Nib;

		static LapTriathlonCell()
		{
			Nib = UINib.FromName("LapTriathlonCell", NSBundle.MainBundle);
		}

		protected LapTriathlonCell(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void SetCell(Lap lapData)
		{
			lap.Text = lapData.lap;
			lapKm.Text = lapData.lapKm;
			elapsedTime.Text = lapData.elapsedTime;
			avgPace.Text = lapData.avgPace;
			avgHr.Text = lapData.avgHr;
			avgCadance.Text = lapData.avgCadance;
			avgPower.Text = lapData.avgPower;
			swim_Strock.Text = lapData.swim_Strock;
		}
	}
}