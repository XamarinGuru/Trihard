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
    [Register ("SelectPTypeViewController")]
    partial class SelectPTypeViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView stateCycling { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView stateOther { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView stateRunning { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView stateSwimming { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView stateTriathlon { get; set; }

        [Action ("ActionSelectedType:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSelectedType (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (stateCycling != null) {
                stateCycling.Dispose ();
                stateCycling = null;
            }

            if (stateOther != null) {
                stateOther.Dispose ();
                stateOther = null;
            }

            if (stateRunning != null) {
                stateRunning.Dispose ();
                stateRunning = null;
            }

            if (stateSwimming != null) {
                stateSwimming.Dispose ();
                stateSwimming = null;
            }

            if (stateTriathlon != null) {
                stateTriathlon.Dispose ();
                stateTriathlon = null;
            }
        }
    }
}