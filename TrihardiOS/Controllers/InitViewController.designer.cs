// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
    [Register ("InitViewController")]
    partial class InitViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSignUp { get; set; }

        [Action ("ActionSignIn:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSignIn (UIKit.UIButton sender);

        [Action ("ActionSignUp:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSignUp (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSignUp != null) {
                btnSignUp.Dispose ();
                btnSignUp = null;
            }
        }
    }
}