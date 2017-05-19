using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using System.Collections.Generic;

namespace location2
{
	public partial class CoachSubGroupViewController : BaseViewController
	{
		public string _group_id;
		SubGroups _subGroups = new SubGroups();
		string[] _groupIDs = new string[6];
		List<UILabel> _lblGroupNames = new List<UILabel>();

		public CoachSubGroupViewController() : base()
		{
		}
		public CoachSubGroupViewController(IntPtr handle) : base(handle)
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

				_subGroups = GetSubGroups(_group_id);

				InvokeOnMainThread(() =>
				{
					for (int i = 0; i < _subGroups.groups.Count; i++)
					{
						_groupIDs[i] = _subGroups.groups[i].groupId;
						_lblGroupNames[i].Text = _subGroups.groups[i].groupName;
					}

					HideLoadingView();
				});
			});
		}

		void InitUISettings()
		{
			NavigationController.NavigationBar.Hidden = false;

			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			_lblGroupNames.Add(lblGroupName1);
			_lblGroupNames.Add(lblGroupName2);
			_lblGroupNames.Add(lblGroupName3);
			_lblGroupNames.Add(lblGroupName4);
			_lblGroupNames.Add(lblGroupName5);
			_lblGroupNames.Add(lblGroupName6);
		}

		partial void ActionSelectedSubGroup(UIButton sender)
		{
			var selectedGroupId = _groupIDs[(int)sender.Tag - 1];

			if (_subGroups.groups == null) return;

			foreach (var subGroup in _subGroups.groups)
			{
				if (subGroup.groupId == selectedGroupId)
				{
					if (subGroup.athletes.Count == 0)
					{
						ShowMessageBox(null, Constants.MSG_NO_ATHLETES);
						return;
					}

					var sb = UIStoryboard.FromName("Main", null);
					CoachAthletesBySubGroupViewController coachSubGroupVC = sb.InstantiateViewController("CoachAthletesBySubGroupViewController") as CoachAthletesBySubGroupViewController;
					coachSubGroupVC._users = subGroup.athletes;
					NavigationController.PushViewController(coachSubGroupVC, true);
				}
			}
		}
	}
}