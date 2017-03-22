using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using PortableLibrary;

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

		public nfloat SetView(Content comment)
		{
			var deltaSecs = float.Parse(comment.date) / 1000;
			var commentDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(deltaSecs).ToLocalTime();

			lblAuthor.Text = comment.author;
			lblCommentDate.Text = String.Format("{0:t}", commentDate) + " | " + String.Format("{0:d}", commentDate);
			lblComment.Text = comment.commentText;

			LayoutIfNeeded();

			return viewContent.Frame.Size.Height;
		}
    }
}