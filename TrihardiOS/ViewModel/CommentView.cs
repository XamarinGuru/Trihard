using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using PortableLibrary;
using SDWebImage;

namespace location2
{
    public partial class CommentView : UIView
    {
        public CommentView (IntPtr handle) : base (handle)
        {
        }

		public static CommentView Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("CommentView", null, null);
			var v = Runtime.GetNSObject<CommentView>(arr.ValueAt(0));
			return v;
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public nfloat SetView(Comment comment)
		{
			try
			{
                imgPhoto.Image = UIImage.FromFile("icon_no_avatar.png");
                if (!string.IsNullOrEmpty(comment.authorUrl))
				{
					imgPhoto.SetImage(url: new NSUrl(comment.authorUrl));
				}
			}
			catch
			{
                imgPhoto.Image = UIImage.FromFile("icon_no_avatar.png");
			}

			var deltaSecs = float.Parse(comment.date) / 1000;
			var commentDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(deltaSecs).ToLocalTime();

			lblAuthor.Text = comment.author;
			lblCommentDate.Text = String.Format("{0:t}", commentDate) + " | " + String.Format("{0:d}", commentDate);
			lblComment.Text = comment.commentText;

			LayoutIfNeeded();

			return viewContent.Frame.Size.Height;
		}

        public void SetHighlight(BaseViewController baseVC)
        {
            lblCommentDate.TextColor = baseVC.FromHexString(PortableLibrary.Constants.COLOR_NEW_NOTIFICATION);
            imgNewSymbol.Hidden = false;
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