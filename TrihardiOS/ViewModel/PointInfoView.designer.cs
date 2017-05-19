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
    [Register ("PointInfoView")]
    partial class PointInfoView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblInterval { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewButton { get; set; }

        [Action ("ActionClose:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionClose (UIKit.UIButton sender);

        [Action ("ActionNavigate:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionNavigate (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (lblInterval != null) {
                lblInterval.Dispose ();
                lblInterval = null;
            }

            if (lblName != null) {
                lblName.Dispose ();
                lblName = null;
            }

            if (viewButton != null) {
                viewButton.Dispose ();
                viewButton = null;
            }
        }
    }
}