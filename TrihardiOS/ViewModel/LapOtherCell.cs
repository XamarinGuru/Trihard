using Foundation;
using System;
using UIKit;
using PortableLibrary.Model;

namespace location2
{
	public partial class LapOtherCell : BaseLapTableViewCell
	{
		public static readonly NSString Key = new NSString("LapOtherCell");
		public static readonly UINib Nib;

		static LapOtherCell()
		{
			Nib = UINib.FromName("LapOtherCell", NSBundle.MainBundle);
		}

		protected LapOtherCell(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void SetCell(Lap lapData)
		{
			lap.Text = lapData.lap;
			elapsedTime.Text = lapData.elapsedTime;
			avgHr.Text = lapData.avgHr;
			avgPower.Text = lapData.avgPower;
			time.Text = lapData.time;
		}
	}
}