using Foundation;
using System;
using UIKit;
using PortableLibrary.Model;

namespace location2
{
	public class BaseLapTableViewCell : UITableViewCell
	{
		protected BaseLapTableViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		public virtual void SetCell(Lap lap) { }
	}

    public partial class LapBikeOrRunCell : BaseLapTableViewCell
    {
        public static readonly NSString Key = new NSString("LapBikeOrRunCell");
        public static readonly UINib Nib;

        static LapBikeOrRunCell()
        {
            Nib = UINib.FromName("LapBikeOrRunCell", NSBundle.MainBundle);
        }

        protected LapBikeOrRunCell(IntPtr handle) : base(handle)
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
        }
    }
}