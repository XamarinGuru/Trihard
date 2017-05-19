// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
    [Register ("PageViewController")]
    partial class MainPageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnCalendar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnHome { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnProfile { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraintTabBarHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView pageContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView tabBar { get; set; }

        [Action ("ActionTab:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionTab (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnCalendar != null) {
                btnCalendar.Dispose ();
                btnCalendar = null;
            }

            if (btnHome != null) {
                btnHome.Dispose ();
                btnHome = null;
            }

            if (btnProfile != null) {
                btnProfile.Dispose ();
                btnProfile = null;
            }

            if (constraintTabBarHeight != null) {
                constraintTabBarHeight.Dispose ();
                constraintTabBarHeight = null;
            }

            if (pageContent != null) {
                pageContent.Dispose ();
                pageContent = null;
            }

            if (tabBar != null) {
                tabBar.Dispose ();
                tabBar = null;
            }
        }
    }
}