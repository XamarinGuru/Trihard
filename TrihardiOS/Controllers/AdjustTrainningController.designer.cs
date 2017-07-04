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
    [Register ("AdjustTrainningController")]
    partial class AdjustTrainningController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch attended { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSaveAdjust { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider seekDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider seekTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider seekTSS { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel strType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtTss { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewType { get; set; }

        [Action ("ActionAdjustTrainning:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAdjustTrainning (UIKit.UIButton sender);

        [Action ("ActionDataChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDataChanged (UIKit.UISlider sender);

        [Action ("ActionSwitchType:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSwitchType (UIKit.UIButton sender);

        [Action ("ActionValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionValueChanged (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (attended != null) {
                attended.Dispose ();
                attended = null;
            }

            if (btnSaveAdjust != null) {
                btnSaveAdjust.Dispose ();
                btnSaveAdjust = null;
            }

            if (imgType != null) {
                imgType.Dispose ();
                imgType = null;
            }

            if (seekDistance != null) {
                seekDistance.Dispose ();
                seekDistance = null;
            }

            if (seekTime != null) {
                seekTime.Dispose ();
                seekTime = null;
            }

            if (seekTSS != null) {
                seekTSS.Dispose ();
                seekTSS = null;
            }

            if (strType != null) {
                strType.Dispose ();
                strType = null;
            }

            if (txtDistance != null) {
                txtDistance.Dispose ();
                txtDistance = null;
            }

            if (txtTime != null) {
                txtTime.Dispose ();
                txtTime = null;
            }

            if (txtTss != null) {
                txtTss.Dispose ();
                txtTss = null;
            }

            if (viewType != null) {
                viewType.Dispose ();
                viewType = null;
            }
        }
    }
}