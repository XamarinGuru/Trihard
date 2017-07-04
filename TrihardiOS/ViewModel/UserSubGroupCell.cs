using Foundation;
using System;
using UIKit;
using PortableLibrary;
using SDWebImage;
using CoreGraphics;

namespace location2
{
    public partial class UserSubGroupCell : UITableViewCell
    {
        AthleteInSubGroup _user;
        BaseViewController _mSuperVC;

        public static readonly NSString Key = new NSString("UserSubGroupCell");
        public static readonly UINib Nib;

        static UserSubGroupCell()
        {
            Nib = UINib.FromName("UserSubGroupCell", NSBundle.MainBundle);
        }

        protected UserSubGroupCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void SetCell(AthleteInSubGroup user, BaseViewController superVC)
        {
            _user = user;
            _mSuperVC = superVC;

            foreach (var subView in scrollView.Subviews)
                subView.RemoveFromSuperview();

            imgStatus.Image = new UIImage();
            imgPhoto.Image = UIImage.FromFile("icon_no_avatar.png");

            try
            {
                lblName.Text = user.athleteName;
                if (!string.IsNullOrEmpty(user.userImagURI))
                {
                    imgPhoto.SetImage(url: new NSUrl(user.userImagURI));
                }
            }
            catch
            {
            }

            var posX = 0;
            for (var i = 0; i < user.eventsDoneToday.Count; i++)
            {
                var eventDoneToday = user.eventsDoneToday[i];
                var imgTodayDone = new UIImageView(new CGRect(posX, 5, 30, 30));
				var pType = (Constants.EVENT_TYPE)Enum.ToObject(typeof(Constants.EVENT_TYPE), int.Parse(eventDoneToday.eventType));
				switch (pType)
				{
					case Constants.EVENT_TYPE.OTHER:
						imgTodayDone.Image = UIImage.FromFile("icon_other.png");
						break;
					case Constants.EVENT_TYPE.BIKE:
						imgTodayDone.Image = UIImage.FromFile("icon_bike.png");
						break;
					case Constants.EVENT_TYPE.RUN:
						imgTodayDone.Image = UIImage.FromFile("icon_run.png");
						break;
					case Constants.EVENT_TYPE.SWIM:
						imgTodayDone.Image = UIImage.FromFile("icon_swim.png");
						break;
					case Constants.EVENT_TYPE.TRIATHLON:
						imgTodayDone.Image = UIImage.FromFile("icon_triathlon.png");
						break;
					case Constants.EVENT_TYPE.ANOTHER:
						imgTodayDone.Image = UIImage.FromFile("icon_other.png");
						break;
				}
                imgTodayDone.ContentMode = UIViewContentMode.ScaleAspectFit;
                scrollView.AddSubview(imgTodayDone);
                posX += 40;
                scrollView.ContentSize = new CGSize(posX, 40);

                var btnActionEventInstruction = new UIButton(imgTodayDone.Frame);
                scrollView.AddSubview(btnActionEventInstruction);
                btnActionEventInstruction.Tag = i;
                btnActionEventInstruction.TouchUpInside += ActionEventInstruction;
            }

            switch (user.pmcStatus)
            {
                case 1:
                    imgStatus.Image = UIImage.FromFile("icon_circle_green.png");
                    break;
                case 2:
                    imgStatus.Image = UIImage.FromFile("icon_circle_blue.png");
                    break;
                case 3:
                    imgStatus.Image = UIImage.FromFile("icon_circle_red.png");
                    break;
                case 4:
                    imgStatus.Image = UIImage.FromFile("icon_circle_empty.png");
                    break;
            }
        }

        private void ActionEventInstruction(object sender, EventArgs e)
        {
            var fakeUserId = _user.athleteId;
            var eventId = _user.eventsDoneToday[(int)(sender as UIButton).Tag].eventId;

            var currentUser = AppSettings.CurrentUser;

            if (currentUser.userId == fakeUserId)
            {
                currentUser.athleteId = null;
                AppSettings.isFakeUser = false;
                AppSettings.fakeUserName = string.Empty;
            }
            else
            {
                currentUser.athleteId = fakeUserId;
                AppSettings.isFakeUser = true;
                AppSettings.fakeUserName = _user.athleteName;
            }

            AppSettings.CurrentUser = currentUser;

            InvokeOnMainThread(() =>
            {
                UIStoryboard sb = UIStoryboard.FromName("Main", null);
                EventInstructionController eventInstructionVC = sb.InstantiateViewController("EventInstructionController") as EventInstructionController;
                eventInstructionVC.eventID = eventId;
                eventInstructionVC.isNotification = false;
                _mSuperVC.NavigationController.PushViewController(eventInstructionVC, true);

            });
        }

        override public void LayoutSubviews()
        {
            base.LayoutSubviews();

            imgPhoto.LayoutIfNeeded();
            imgPhoto.Layer.CornerRadius = imgPhoto.Frame.Size.Width / 2;
            imgPhoto.Layer.MasksToBounds = true;
            imgPhoto.Layer.BorderWidth = 1;
            imgPhoto.Layer.BorderColor = UIColor.Gray.CGColor;
        }
    }
}