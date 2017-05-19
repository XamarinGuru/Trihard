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
    [Register ("CalendarHomeViewController")]
    partial class CalendarHomeViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnCycleColleps { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnRunningColleps { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSwimmingColleps { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnViewCalendar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView chartContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightCycle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightRunning { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightSwimming { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgSymbolCycling { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgSymbolRunning { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgSymbolSwimming { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleDistanceTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleDurationTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleStress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCycleStressTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblFakeUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunDistanceTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunDurationTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunStress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblRunStressTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimDistanceTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimDurationTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimStress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSwimStressTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView symATL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView symCTL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView symDailyIF { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView symDailyLoad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView symTSB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtATL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtCTL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtDailyIF { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtDailyTSS { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtTSB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewCycle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewRunning { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewSwimming { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        location2.RangeSliderControl zoomSlider { get; set; }

        [Action ("ActionCollect:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionCollect (UIKit.UIButton sender);

        [Action ("ActionViewCalendar:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionViewCalendar (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnCycleColleps != null) {
                btnCycleColleps.Dispose ();
                btnCycleColleps = null;
            }

            if (btnRunningColleps != null) {
                btnRunningColleps.Dispose ();
                btnRunningColleps = null;
            }

            if (btnSwimmingColleps != null) {
                btnSwimmingColleps.Dispose ();
                btnSwimmingColleps = null;
            }

            if (btnViewCalendar != null) {
                btnViewCalendar.Dispose ();
                btnViewCalendar = null;
            }

            if (chartContent != null) {
                chartContent.Dispose ();
                chartContent = null;
            }

            if (heightCycle != null) {
                heightCycle.Dispose ();
                heightCycle = null;
            }

            if (heightRunning != null) {
                heightRunning.Dispose ();
                heightRunning = null;
            }

            if (heightSwimming != null) {
                heightSwimming.Dispose ();
                heightSwimming = null;
            }

            if (imgSymbolCycling != null) {
                imgSymbolCycling.Dispose ();
                imgSymbolCycling = null;
            }

            if (imgSymbolRunning != null) {
                imgSymbolRunning.Dispose ();
                imgSymbolRunning = null;
            }

            if (imgSymbolSwimming != null) {
                imgSymbolSwimming.Dispose ();
                imgSymbolSwimming = null;
            }

            if (lblCycleDistance != null) {
                lblCycleDistance.Dispose ();
                lblCycleDistance = null;
            }

            if (lblCycleDistanceTitle != null) {
                lblCycleDistanceTitle.Dispose ();
                lblCycleDistanceTitle = null;
            }

            if (lblCycleDuration != null) {
                lblCycleDuration.Dispose ();
                lblCycleDuration = null;
            }

            if (lblCycleDurationTitle != null) {
                lblCycleDurationTitle.Dispose ();
                lblCycleDurationTitle = null;
            }

            if (lblCycleStress != null) {
                lblCycleStress.Dispose ();
                lblCycleStress = null;
            }

            if (lblCycleStressTitle != null) {
                lblCycleStressTitle.Dispose ();
                lblCycleStressTitle = null;
            }

            if (lblFakeUserName != null) {
                lblFakeUserName.Dispose ();
                lblFakeUserName = null;
            }

            if (lblRunDistance != null) {
                lblRunDistance.Dispose ();
                lblRunDistance = null;
            }

            if (lblRunDistanceTitle != null) {
                lblRunDistanceTitle.Dispose ();
                lblRunDistanceTitle = null;
            }

            if (lblRunDuration != null) {
                lblRunDuration.Dispose ();
                lblRunDuration = null;
            }

            if (lblRunDurationTitle != null) {
                lblRunDurationTitle.Dispose ();
                lblRunDurationTitle = null;
            }

            if (lblRunStress != null) {
                lblRunStress.Dispose ();
                lblRunStress = null;
            }

            if (lblRunStressTitle != null) {
                lblRunStressTitle.Dispose ();
                lblRunStressTitle = null;
            }

            if (lblSwimDistance != null) {
                lblSwimDistance.Dispose ();
                lblSwimDistance = null;
            }

            if (lblSwimDistanceTitle != null) {
                lblSwimDistanceTitle.Dispose ();
                lblSwimDistanceTitle = null;
            }

            if (lblSwimDuration != null) {
                lblSwimDuration.Dispose ();
                lblSwimDuration = null;
            }

            if (lblSwimDurationTitle != null) {
                lblSwimDurationTitle.Dispose ();
                lblSwimDurationTitle = null;
            }

            if (lblSwimStress != null) {
                lblSwimStress.Dispose ();
                lblSwimStress = null;
            }

            if (lblSwimStressTitle != null) {
                lblSwimStressTitle.Dispose ();
                lblSwimStressTitle = null;
            }

            if (symATL != null) {
                symATL.Dispose ();
                symATL = null;
            }

            if (symCTL != null) {
                symCTL.Dispose ();
                symCTL = null;
            }

            if (symDailyIF != null) {
                symDailyIF.Dispose ();
                symDailyIF = null;
            }

            if (symDailyLoad != null) {
                symDailyLoad.Dispose ();
                symDailyLoad = null;
            }

            if (symTSB != null) {
                symTSB.Dispose ();
                symTSB = null;
            }

            if (txtATL != null) {
                txtATL.Dispose ();
                txtATL = null;
            }

            if (txtCTL != null) {
                txtCTL.Dispose ();
                txtCTL = null;
            }

            if (txtDailyIF != null) {
                txtDailyIF.Dispose ();
                txtDailyIF = null;
            }

            if (txtDailyTSS != null) {
                txtDailyTSS.Dispose ();
                txtDailyTSS = null;
            }

            if (txtTSB != null) {
                txtTSB.Dispose ();
                txtTSB = null;
            }

            if (viewCycle != null) {
                viewCycle.Dispose ();
                viewCycle = null;
            }

            if (viewRunning != null) {
                viewRunning.Dispose ();
                viewRunning = null;
            }

            if (viewSwimming != null) {
                viewSwimming.Dispose ();
                viewSwimming = null;
            }

            if (zoomSlider != null) {
                zoomSlider.Dispose ();
                zoomSlider = null;
            }
        }
    }
}