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
    [Register ("CoachSubGroupViewController")]
    partial class CoachSubGroupViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName5 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGroupName6 { get; set; }

        [Action ("ActionSelectedSubGroup:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSelectedSubGroup (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (lblGroupName1 != null) {
                lblGroupName1.Dispose ();
                lblGroupName1 = null;
            }

            if (lblGroupName2 != null) {
                lblGroupName2.Dispose ();
                lblGroupName2 = null;
            }

            if (lblGroupName3 != null) {
                lblGroupName3.Dispose ();
                lblGroupName3 = null;
            }

            if (lblGroupName4 != null) {
                lblGroupName4.Dispose ();
                lblGroupName4 = null;
            }

            if (lblGroupName5 != null) {
                lblGroupName5.Dispose ();
                lblGroupName5 = null;
            }

            if (lblGroupName6 != null) {
                lblGroupName6.Dispose ();
                lblGroupName6 = null;
            }
        }
    }
}