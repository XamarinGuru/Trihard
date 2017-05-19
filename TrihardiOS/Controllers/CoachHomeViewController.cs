using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using PortableLibrary;
using System.Linq;

namespace location2
{
	public partial class CoachHomeViewController : BaseViewController
	{
		List<Athlete> _users = new List<Athlete>();

		public CoachHomeViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				_users = GetAllUsers();

				var tblDataSource = new UsersTableViewSource(_users, this);

				InvokeOnMainThread(() =>
				{
					tableView.Source = tblDataSource;
					tableView.ReloadData();
					HideLoadingView();
				});
			});
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			NavigationController.NavigationBar.Hidden = true;
		}

		void InitUISettings()
		{
			txtSearch.EditingChanged += ActionSearch;
		}

		void ActionSearch(object sender, EventArgs e)
		{
			(tableView.Source as UsersTableViewSource).PerformSearch((sender as UITextField).Text);
			tableView.ReloadData();
		}

		partial void ActionGoToGroup(UIButton sender)
		{
			var sb = UIStoryboard.FromName("Main", null);
			CoachGroupsViewController coachGroupsVC = sb.InstantiateViewController("CoachGroupsViewController") as CoachGroupsViewController;
			NavigationController.PushViewController(coachGroupsVC, true);
		}

		#region UserTableViewSource
		class UsersTableViewSource : UITableViewSource
		{
			List<Athlete> _athletes;
			List<Athlete> _searchAthletes;
			BaseViewController mSuperVC;

			public UsersTableViewSource(List<Athlete> users, BaseViewController superVC)
			{
				_athletes = new List<Athlete>();

				if (users == null) return;

				_athletes = users;
				_searchAthletes = users;
				mSuperVC = superVC;
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return _searchAthletes.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 50;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				UserCell cell = tableView.DequeueReusableCell("UserCell") as UserCell;
				cell.SetCell(_searchAthletes[indexPath.Row]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!mSuperVC.IsNetEnable()) return;

				var selectedAthlete = _searchAthletes[indexPath.Row];
				var fakeUserId = selectedAthlete._id;
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
					foreach (var tmpUser in _searchAthletes)
					{
						if (tmpUser._id == fakeUserId)
							AppSettings.fakeUserName = tmpUser.name;
					}
				}

				AppSettings.CurrentUser = currentUser;

				//var nextIntent = new Intent(mSuperActivity, typeof(SwipeTabActivity));
				//mSuperActivity.StartActivityForResult(nextIntent, 0);
				var sb = UIStoryboard.FromName("Main", null);
				MainPageViewController nextVC = sb.InstantiateViewController("MainPageViewController") as MainPageViewController;
				mSuperVC.NavigationController.PushViewController(nextVC, true);
			}

			public void PerformSearch(string strSearch)
			{
				strSearch = strSearch.ToLower();
				_searchAthletes = _athletes.Where(x => x.name.ToLower().Contains(strSearch)).ToList();
			}
		}
		#endregion
	}
}