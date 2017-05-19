using Foundation;
using System;
using UIKit;
using PortableLibrary;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace location2
{
	public partial class CoachAthletesBySubGroupViewController : BaseViewController
	{
		public List<AthleteInSubGroup> _users = new List<AthleteInSubGroup>();

		public CoachAthletesBySubGroupViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();

			if (!IsNetEnable()) return;

			tableView.Source = new UsersSubGroupTableViewSource(_users, this);
			tableView.ReloadData();
		}

		void InitUISettings()
		{
			txtSearch.EditingChanged += ActionSearch;

			NavigationController.NavigationBar.Hidden = false;

			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);
		}

		void ActionSearch(object sender, EventArgs e)
		{
			(tableView.Source as UsersSubGroupTableViewSource).PerformSearch((sender as UITextField).Text);
			tableView.ReloadData();
		}

		partial void ActionGoToGroup(UIButton sender)
		{
			var sb = UIStoryboard.FromName("Main", null);
			CoachGroupsViewController coachGroupsVC = sb.InstantiateViewController("CoachGroupsViewController") as CoachGroupsViewController;
			NavigationController.PushViewController(coachGroupsVC, true);
		}

		#region UserTableViewSource
		class UsersSubGroupTableViewSource : UITableViewSource
		{
			List<AthleteInSubGroup> _athletes;
			List<AthleteInSubGroup> _searchAthletes;
			BaseViewController mSuperVC;

			public UsersSubGroupTableViewSource(List<AthleteInSubGroup> users, BaseViewController superVC)
			{
				_athletes = new List<AthleteInSubGroup>();

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
				UserSubGroupCell cell = tableView.DequeueReusableCell("UserSubGroupCell") as UserSubGroupCell;
				cell.SetCell(_searchAthletes[indexPath.Row]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!mSuperVC.IsNetEnable()) return;

				var selectedAthlete = _searchAthletes[indexPath.Row];
				var fakeUserId = selectedAthlete.athleteId;
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
						if (tmpUser.athleteId == fakeUserId)
							AppSettings.fakeUserName = tmpUser.athleteName;
					}
				}

				AppSettings.CurrentUser = currentUser;

				var sb = UIStoryboard.FromName("Main", null);
				MainPageViewController nextVC = sb.InstantiateViewController("MainPageViewController") as MainPageViewController;
				mSuperVC.NavigationController.PushViewController(nextVC, true);
			}

			public void PerformSearch(string strSearch)
			{
				strSearch = strSearch.ToLower();
				_searchAthletes = _athletes.Where(x => x.athleteName.ToLower().Contains(strSearch)).ToList();
			}
		}
		#endregion
	}
}