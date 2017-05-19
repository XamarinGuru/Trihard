using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

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

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});
		}

		void InitUISettings()
		{
			btnAddComment.BackgroundColor = GROUP_COLOR;
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

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_SAVE_COMMENT);

				InvokeOnMainThread(() =>
				{
					var response = SetComment(author, authorID, txtComment.Text, selectedEvent._id);

					HideLoadingView();
					NavigationController.PopViewController(true);
				});
			});
		}
    }
}