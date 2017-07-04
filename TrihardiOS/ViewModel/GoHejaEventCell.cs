using System;
using Foundation;
using PortableLibrary;
using UIKit;

namespace location2
{
	public partial class GoHejaEventCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("NitroEventCell");
		public static readonly UINib Nib;

		static GoHejaEventCell()
		{
			Nib = UINib.FromName("NitroEventCell", NSBundle.MainBundle);
		}

		protected GoHejaEventCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public void SetCell(GoHejaEvent goHejaEvent)
		{
			var strTitle = goHejaEvent.title;
			var strTime = String.Format("{0:t}", goHejaEvent.StartDateTime());

			lblTitle.Text = strTitle;
			lblEventTime.Text = strTime;

			var eventStart = goHejaEvent.StartDateTime();
			var dateNow = DateTime.Now;
			var durMin = goHejaEvent.durMin == "" ? 0 : int.Parse(goHejaEvent.durMin);
			var durHrs = goHejaEvent.durHrs == "" ? 0 : int.Parse(goHejaEvent.durHrs);
			var durSec = durHrs * 3600 + durMin * 60;

			if (goHejaEvent.attended == "0" && DateTime.Compare(eventStart, dateNow.AddSeconds(durSec)) < 0)
			{
				var attrTitle = new NSAttributedString(lblTitle.Text, strikethroughStyle: NSUnderlineStyle.Single);
				var attrTime = new NSAttributedString(lblEventTime.Text, strikethroughStyle: NSUnderlineStyle.Single);
				lblTitle.AttributedText = attrTitle;
				lblEventTime.AttributedText = attrTime;
				lblTitle.TextColor = UIColor.FromRGB(112, 112, 112);
				lblEventTime.TextColor = UIColor.FromRGB(112, 112, 112);
			}
			else
			{
				var attrTitle = new NSAttributedString(lblTitle.Text, strikethroughStyle: NSUnderlineStyle.None);
				var attrTime = new NSAttributedString(lblEventTime.Text, strikethroughStyle: NSUnderlineStyle.None);
				lblTitle.AttributedText = attrTitle;
				lblEventTime.AttributedText = attrTime;
				lblTitle.TextColor = UIColor.White;
				lblEventTime.TextColor = UIColor.White;
			}

			var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(goHejaEvent.type));
			switch (pType)
			{
				case Constants.EVENT_TYPE.OTHER:
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
				case Constants.EVENT_TYPE.BIKE:
					imgType.Image = UIImage.FromFile("icon_bike.png");
					break;
				case Constants.EVENT_TYPE.RUN:
					imgType.Image = UIImage.FromFile("icon_run.png");
					break;
				case Constants.EVENT_TYPE.SWIM:
					imgType.Image = UIImage.FromFile("icon_swim.png");
					break;
				case Constants.EVENT_TYPE.TRIATHLON:
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case Constants.EVENT_TYPE.ANOTHER:
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
			}
		}
	}
}
