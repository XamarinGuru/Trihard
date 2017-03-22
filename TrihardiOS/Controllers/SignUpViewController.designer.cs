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
    [Register ("SignUpViewController")]
    partial class SignUpViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton acceprtBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSignUP { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidAge { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidFirstname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidLastname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnValidUsername { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView listingView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtAge { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtFirstName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtLastName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtNickName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtUsername { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorFirstname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorLastname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewErrorUsername { get; set; }

        [Action ("ActionAcceptTerms:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAcceptTerms (UIKit.UIButton sender);

        [Action ("ActionBack:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionBack (UIKit.UIButton sender);

        [Action ("ActionSignUp:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSignUp (UIKit.UIButton sender);

        [Action ("ActionTerms:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionTerms (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (acceprtBtn != null) {
                acceprtBtn.Dispose ();
                acceprtBtn = null;
            }

            if (btnSignUP != null) {
                btnSignUP.Dispose ();
                btnSignUP = null;
            }

            if (btnValidAge != null) {
                btnValidAge.Dispose ();
                btnValidAge = null;
            }

            if (btnValidEmail != null) {
                btnValidEmail.Dispose ();
                btnValidEmail = null;
            }

            if (btnValidFirstname != null) {
                btnValidFirstname.Dispose ();
                btnValidFirstname = null;
            }

            if (btnValidLastname != null) {
                btnValidLastname.Dispose ();
                btnValidLastname = null;
            }

            if (btnValidPassword != null) {
                btnValidPassword.Dispose ();
                btnValidPassword = null;
            }

            if (btnValidTerms != null) {
                btnValidTerms.Dispose ();
                btnValidTerms = null;
            }

            if (btnValidUsername != null) {
                btnValidUsername.Dispose ();
                btnValidUsername = null;
            }

            if (listingView != null) {
                listingView.Dispose ();
                listingView = null;
            }

            if (txtAge != null) {
                txtAge.Dispose ();
                txtAge = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtFirstName != null) {
                txtFirstName.Dispose ();
                txtFirstName = null;
            }

            if (txtLastName != null) {
                txtLastName.Dispose ();
                txtLastName = null;
            }

            if (txtNickName != null) {
                txtNickName.Dispose ();
                txtNickName = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }

            if (txtUsername != null) {
                txtUsername.Dispose ();
                txtUsername = null;
            }

            if (viewErrorEmail != null) {
                viewErrorEmail.Dispose ();
                viewErrorEmail = null;
            }

            if (viewErrorFirstname != null) {
                viewErrorFirstname.Dispose ();
                viewErrorFirstname = null;
            }

            if (viewErrorLastname != null) {
                viewErrorLastname.Dispose ();
                viewErrorLastname = null;
            }

            if (viewErrorPassword != null) {
                viewErrorPassword.Dispose ();
                viewErrorPassword = null;
            }

            if (viewErrorUsername != null) {
                viewErrorUsername.Dispose ();
                viewErrorUsername = null;
            }
        }
    }
}