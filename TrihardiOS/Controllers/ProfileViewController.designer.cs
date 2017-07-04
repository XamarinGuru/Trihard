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
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnNotificationSetting { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton changePictureBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgPicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPhone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton removeNitroEventsBtn { get; set; }

        [Action ("ActionChangePassword:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionChangePassword (UIKit.UIButton sender);

        [Action ("ActionChangePhoto:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionChangePhoto (UIKit.UIButton sender);

        [Action ("ActionEditProfile:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionEditProfile (UIKit.UIButton sender);

        [Action ("ActionNotificationSetting:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionNotificationSetting (UIKit.UIButton sender);

        [Action ("ActionSignOut:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSignOut (UIKit.UIButton sender);

        [Action ("ActionSyncDevice:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSyncDevice (UIKit.UIButton sender);

        [Action ("removeNitroEvents:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void removeNitroEvents (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnNotificationSetting != null) {
                btnNotificationSetting.Dispose ();
                btnNotificationSetting = null;
            }

            if (changePictureBtn != null) {
                changePictureBtn.Dispose ();
                changePictureBtn = null;
            }

            if (imgPicture != null) {
                imgPicture.Dispose ();
                imgPicture = null;
            }

            if (lblEmail != null) {
                lblEmail.Dispose ();
                lblEmail = null;
            }

            if (lblPhone != null) {
                lblPhone.Dispose ();
                lblPhone = null;
            }

            if (lblUserName != null) {
                lblUserName.Dispose ();
                lblUserName = null;
            }

            if (removeNitroEventsBtn != null) {
                removeNitroEventsBtn.Dispose ();
                removeNitroEventsBtn = null;
            }
        }
    }
}