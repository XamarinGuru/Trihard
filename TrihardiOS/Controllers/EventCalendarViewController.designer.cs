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
    [Register ("EventCalendarViewController")]
    partial class EventCalendarViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Xuni.iOS.Calendar.XuniCalendar calendar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightEventDetail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblATL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCTL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLoad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblNoEvents { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTSB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewCalendar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewDate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (calendar != null) {
                calendar.Dispose ();
                calendar = null;
            }

            if (heightEventDetail != null) {
                heightEventDetail.Dispose ();
                heightEventDetail = null;
            }

            if (lblATL != null) {
                lblATL.Dispose ();
                lblATL = null;
            }

            if (lblCTL != null) {
                lblCTL.Dispose ();
                lblCTL = null;
            }

            if (lblLoad != null) {
                lblLoad.Dispose ();
                lblLoad = null;
            }

            if (lblNoEvents != null) {
                lblNoEvents.Dispose ();
                lblNoEvents = null;
            }

            if (lblTSB != null) {
                lblTSB.Dispose ();
                lblTSB = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }

            if (viewCalendar != null) {
                viewCalendar.Dispose ();
                viewCalendar = null;
            }

            if (viewDate != null) {
                viewDate.Dispose ();
                viewDate = null;
            }
        }
    }
}