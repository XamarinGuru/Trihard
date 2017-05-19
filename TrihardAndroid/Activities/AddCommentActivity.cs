
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "AddCommentActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AddCommentActivity : BaseActivity
	{
		RootMemberModel MemberModel = new RootMemberModel();

		EditText txtComment;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AddCommentActivity);

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
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);
			FindViewById(Resource.Id.ActionAddComment).Click += ActionAddComment;
			FindViewById(Resource.Id.ActionAddComment).SetBackgroundColor(GROUP_COLOR);
		}

		void ActionAddComment(object sender, EventArgs e)
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

				var response = SetComment(string.Empty, authorID, txtComment.Text, AppSettings.selectedEvent._id);

				HideLoadingView();

				ActionBackCancel();
			});
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				ActionBackCancel();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
