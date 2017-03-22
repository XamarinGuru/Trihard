using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
	public partial class PointInfoView : UIView
	{
		Point point = new Point();

		public delegate void PopWillCloseHandler();
		public event PopWillCloseHandler PopWillClose;

		UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));

		CGSize size = new CGSize(300, 350);

		public PointInfoView(IntPtr handle) : base(handle)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public static PointInfoView Create(Point point)
		{
			var arr = NSBundle.MainBundle.LoadNib("PointInfoView", null, null);
			var v = Runtime.GetNSObject<PointInfoView>(arr.ValueAt(0));

			v.point = point;

			nfloat lx = (UIScreen.MainScreen.Bounds.Width - v.size.Width) / 2;
			nfloat ly = (UIScreen.MainScreen.Bounds.Height - v.size.Height) / 2;
			v.Frame = new CGRect(new CGPoint(lx, ly), v.size);

			v.effectView.Alpha = 0;

			v.lblName.Text = "Name: " + point.name;
			v.lblDescription.Text = point.description;
			v.lblInterval.Text = "Interval: " + point.interval;

			v.Layer.CornerRadius = 5;
			v.ClipsToBounds = true;

			return v;
		}

		partial void ActionNavigate(UIButton sender)
		{
			if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("waze://")))
			{
				var wazeURL = new NSUrl(string.Format("waze://?ll={0},{1}&navigate=yes", point.lat, point.lng));
				UIApplication.SharedApplication.OpenUrl(wazeURL);
			}
			else {
				var wazeAppURL = new NSUrl("http://itunes.apple.com/us/app/id323229106");
				UIApplication.SharedApplication.OpenUrl(wazeAppURL);
			}
		}

		partial void ActionClose(UIButton sender)
		{
			Close();
		}


		public void PopUp(bool animated = true, Action popAnimationFinish = null)
		{
			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			effectView.Frame = window.Bounds;
			window.EndEditing(true);
			window.AddSubview(effectView);
			window.AddSubview(this);

			if (animated)
			{
				Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
				UIView.Animate(0.15, delegate
				{
					Transform = CGAffineTransform.MakeScale(1, 1);
					effectView.Alpha = 0.8f;
				}, delegate
				{
					if (null != popAnimationFinish)
						popAnimationFinish();
				});
			}
			else {
				effectView.Alpha = 0.8f;
			}
		}

		public void Close(bool animated = true)
		{
			if (animated)
			{
				UIView.Animate(0.15, delegate
				{
					Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
					effectView.Alpha = 0;
				}, delegate
				{
					this.RemoveFromSuperview();
					effectView.RemoveFromSuperview();
					if (null != PopWillClose) PopWillClose();
				});
			}
			else {
				if (null != PopWillClose) PopWillClose();
			}
		}
	}
}