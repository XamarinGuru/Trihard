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
    [Register ("ChangePasswordViewController")]
    partial class ChangePasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnChangePW { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidPwConfirm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPwConfirm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorPwConfirm { get; set; }

        [Action ("ActionResetPassword:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionResetPassword (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnChangePW != null) {
                btnChangePW.Dispose ();
                btnChangePW = null;
            }

            if (btnValidPassword != null) {
                btnValidPassword.Dispose ();
                btnValidPassword = null;
            }

            if (btnValidPwConfirm != null) {
                btnValidPwConfirm.Dispose ();
                btnValidPwConfirm = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }

            if (txtPwConfirm != null) {
                txtPwConfirm.Dispose ();
                txtPwConfirm = null;
            }

            if (viewErrorPassword != null) {
                viewErrorPassword.Dispose ();
                viewErrorPassword = null;
            }

            if (viewErrorPwConfirm != null) {
                viewErrorPwConfirm.Dispose ();
                viewErrorPwConfirm = null;
            }
        }
    }
}