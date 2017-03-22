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
    [Register ("AnalyticsViewController")]
    partial class AnalyticsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView altimg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel altTypeLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView bpmImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel bpmLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnStartPause { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnStop { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView distImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel distTypLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgTypeIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAlt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBpm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDist { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSpeed { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTimer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblWatt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView mainframe { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView speedImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel speedTypeLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewMapContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView wattImg { get; set; }

        [Action ("ActionBack:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionBack (UIKit.UIButton sender);

        [Action ("ActionStartPause:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionStartPause (UIKit.UIButton sender);

        [Action ("ActionStop:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionStop (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (altimg != null) {
                altimg.Dispose ();
                altimg = null;
            }

            if (altTypeLbl != null) {
                altTypeLbl.Dispose ();
                altTypeLbl = null;
            }

            if (bpmImg != null) {
                bpmImg.Dispose ();
                bpmImg = null;
            }

            if (bpmLbl != null) {
                bpmLbl.Dispose ();
                bpmLbl = null;
            }

            if (btnBack != null) {
                btnBack.Dispose ();
                btnBack = null;
            }

            if (btnStartPause != null) {
                btnStartPause.Dispose ();
                btnStartPause = null;
            }

            if (btnStop != null) {
                btnStop.Dispose ();
                btnStop = null;
            }

            if (distImg != null) {
                distImg.Dispose ();
                distImg = null;
            }

            if (distTypLbl != null) {
                distTypLbl.Dispose ();
                distTypLbl = null;
            }

            if (imgTypeIcon != null) {
                imgTypeIcon.Dispose ();
                imgTypeIcon = null;
            }

            if (lblAlt != null) {
                lblAlt.Dispose ();
                lblAlt = null;
            }

            if (lblBpm != null) {
                lblBpm.Dispose ();
                lblBpm = null;
            }

            if (lblDist != null) {
                lblDist.Dispose ();
                lblDist = null;
            }

            if (lblSpeed != null) {
                lblSpeed.Dispose ();
                lblSpeed = null;
            }

            if (lblTimer != null) {
                lblTimer.Dispose ();
                lblTimer = null;
            }

            if (lblWatt != null) {
                lblWatt.Dispose ();
                lblWatt = null;
            }

            if (mainframe != null) {
                mainframe.Dispose ();
                mainframe = null;
            }

            if (speedImg != null) {
                speedImg.Dispose ();
                speedImg = null;
            }

            if (speedTypeLbl != null) {
                speedTypeLbl.Dispose ();
                speedTypeLbl = null;
            }

            if (viewMapContent != null) {
                viewMapContent.Dispose ();
                viewMapContent = null;
            }

            if (wattImg != null) {
                wattImg.Dispose ();
                wattImg = null;
            }
        }
    }
}