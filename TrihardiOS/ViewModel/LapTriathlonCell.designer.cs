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
    [Register ("LapTriathlonCell")]
    partial class LapTriathlonCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel avgCadance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel avgHr { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel avgPace { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel avgPower { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel elapsedTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lap { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lapKm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel swim_Strock { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (avgCadance != null) {
                avgCadance.Dispose ();
                avgCadance = null;
            }

            if (avgHr != null) {
                avgHr.Dispose ();
                avgHr = null;
            }

            if (avgPace != null) {
                avgPace.Dispose ();
                avgPace = null;
            }

            if (avgPower != null) {
                avgPower.Dispose ();
                avgPower = null;
            }

            if (elapsedTime != null) {
                elapsedTime.Dispose ();
                elapsedTime = null;
            }

            if (lap != null) {
                lap.Dispose ();
                lap = null;
            }

            if (lapKm != null) {
                lapKm.Dispose ();
                lapKm = null;
            }

            if (swim_Strock != null) {
                swim_Strock.Dispose ();
                swim_Strock = null;
            }
        }
    }
}