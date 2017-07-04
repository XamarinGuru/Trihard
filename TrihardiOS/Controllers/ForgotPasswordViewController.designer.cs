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
    [Register ("ForgotPasswordViewController")]
    partial class ForgotPasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnForgotPW { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorEmail { get; set; }

        [Action ("ActionBack:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionBack (UIKit.UIButton sender);

        [Action ("ActionResetPassword:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionResetPassword (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnForgotPW != null) {
                btnForgotPW.Dispose ();
                btnForgotPW = null;
            }

            if (btnValidEmail != null) {
                btnValidEmail.Dispose ();
                btnValidEmail = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (viewErrorEmail != null) {
                viewErrorEmail.Dispose ();
                viewErrorEmail = null;
            }
        }
    }
}