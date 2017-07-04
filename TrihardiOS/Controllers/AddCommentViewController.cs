using System;
using UIKit;
using PortableLibrary;
using System.Threading;

namespace location2
{
    public partial class AddCommentViewController : BaseViewController
    {
		public GoHejaEvent selectedEvent;

        public AddCommentViewController() : base()
		{
		}
		public AddCommentViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			if (!IsNetEnable()) return;

			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});
		}

		partial void ActionAddComment(UIButton sender)
		{
			if (txtComment.Text == "")
			{
				ShowMessageBox(null, Constants.MSG_TYPE_COMMENT);
				return;
			}

			if (!IsNetEnable()) return;

			var author = MemberModel.firstname + " " + MemberModel.lastname;
			var authorID = AppSettings.CurrentUser.userId;

			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_SAVE_COMMENT);

				InvokeOnMainThread(() =>
				{
					var response = AddComment(txtComment.Text, selectedEvent);

					HideLoadingView();
					NavigationController.PopViewController(true);
				});
			});
		}
    }
}