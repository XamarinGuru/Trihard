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
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorPassword { get; set; }

        [Action ("ActionBack:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionBack (UIKit.UIButton sender);

        [Action ("ActionForgotPassword:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionForgotPassword (UIKit.UIButton sender);

        [Action ("ActionLogin:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionLogin (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnLogin != null) {
                btnLogin.Dispose ();
                btnLogin = null;
            }

            if (btnValidEmail != null) {
                btnValidEmail.Dispose ();
                btnValidEmail = null;
            }

            if (btnValidPassword != null) {
                btnValidPassword.Dispose ();
                btnValidPassword = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }

            if (viewErrorEmail != null) {
                viewErrorEmail.Dispose ();
                viewErrorEmail = null;
            }

            if (viewErrorPassword != null) {
                viewErrorPassword.Dispose ();
                viewErrorPassword = null;
            }
        }
    }
}