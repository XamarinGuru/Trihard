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
    [Register ("PracticeSelectionViewController")]
    partial class PracticeSelectionViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView mainframe { get; set; }

        [Action ("ActionSelectedType:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSelectedType (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (mainframe != null) {
                mainframe.Dispose ();
                mainframe = null;
            }
        }
    }
}