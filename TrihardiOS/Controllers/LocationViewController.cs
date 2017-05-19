using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using Google.Maps;
using System.Drawing;
using CoreLocation;
using System.Linq;
using System.Collections.Generic;

namespace location2
{
    public partial class LocationViewController : BaseViewController
    {
		public string eventID;
		MapView mMapView;

		EventPoints mEventMarker = new EventPoints();

        public LocationViewController() : base()
		{
		}
		public LocationViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = NavLeftButton();
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			InitUISettings();

			if (!IsNetEnable()) return;

			InitMapView();

			GetMarkersAndPoints();
		}

		void InitUISettings()
		{

		}

		public override void ViewWillLayoutSubviews()
		{
			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		void InitMapView()
		{
			var camera = CameraPosition.FromCamera(31.0461, 34.8516, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;

			mMapView.TappedMarker = ClickedDropItem;
		}

		void GetMarkersAndPoints()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(PortableLibrary.Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetAllMarkers(eventID);
				var trackPoints = GetTrackPoints(eventID);

				HideLoadingView();

				InvokeOnMainThread(() =>
				{
					var boundPoints = new List<CLLocationCoordinate2D>();

					if (mEventMarker != null && mEventMarker.markers.Count > 0)
					{
						for (int i = 0; i < mEventMarker.markers.Count; i++)
						{
							var point = mEventMarker.markers[i];
							var imgPin = GetPinIconByType(point.type);
							var pointLocation = new CLLocationCoordinate2D(point.lat, point.lng);
							boundPoints.Add(pointLocation);

							AddMapPin(pointLocation, imgPin, i);
						}
					}

					if (trackPoints != null && trackPoints.Count > 0)
					{
						if (trackPoints[0].Count > 0)
						{
							var startPoint = trackPoints[0][0];
							var endPoint = trackPoints[trackPoints.Count - 1][trackPoints[trackPoints.Count - 1].Count - 1];
							var startLocation = new CLLocationCoordinate2D(startPoint.Latitude, startPoint.Longitude);
							var endLocation = new CLLocationCoordinate2D(endPoint.Latitude, endPoint.Longitude);
							AddMapPin(startLocation, GetPinIconByType("pSTART"), -1);
							AddMapPin(endLocation, GetPinIconByType("pFINISH"), -1);
						}

						for (int i = 0; i < trackPoints.Count; i ++)
						{
							var tPoints = trackPoints[i];

							var path = new MutablePath();
							var polyline = new Polyline();

							for (int j = 0; j < tPoints.Count; j ++)
							{
								var tPoint = tPoints[j];
								var tLocation = new CLLocationCoordinate2D(tPoint.Latitude, tPoint.Longitude);

								if (j < tPoints.Count - 1)
								{
									var distance = DistanceAtoB(tPoint, tPoints[j + 1]);

									if (PortableLibrary.Constants.AVAILABLE_DISTANCE_MAP > distance)
									{
										var nPoint = tPoints[j + 1];
										path.AddCoordinate(tLocation);
									}
									else {
										polyline.Path = path;
										polyline.StrokeColor = GetRandomColor(i);
										polyline.StrokeWidth = 5;
										polyline.Geodesic = true;
										polyline.Map = mMapView;

										path = new MutablePath();
										polyline = new Polyline();
									}
								}

								polyline.Path = path;
								polyline.StrokeColor = GetRandomColor(i);
								polyline.StrokeWidth = 5;
								polyline.Geodesic = true;
								polyline.Map = mMapView;

								boundPoints.Add(tLocation);
							}
						}
					}

					if (boundPoints.Count == 0)
					{
						var camera = CameraPosition.FromCamera(PortableLibrary.Constants.LOCATION_ISURAEL[0], PortableLibrary.Constants.LOCATION_ISURAEL[1], zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
						mMapView = MapView.FromCamera(RectangleF.Empty, camera);
					}
					else
					{
						var mapBounds = new CoordinateBounds();
						foreach (var bound in boundPoints)
							mapBounds = mapBounds.Including(bound);
						mMapView.MoveCamera(CameraUpdate.FitBounds(mapBounds, 50.0f));
					}
				});
			});
		}

		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			viewMapContent.LayoutIfNeeded();
			var width = viewMapContent.Frame.Width;
			var height = viewMapContent.Frame.Height;
			mMapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mMapView);
		}

		void AddMapPin(CLLocationCoordinate2D position, UIImage icon, int zIndex)
		{
			var marker = new Marker
			{
				Position = position,
				Map = mMapView,
				Icon = icon,
				ZIndex = zIndex
			};
		}
		#region map pin click event
		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			if (marker.ZIndex == -1) return true;

			var selectedPoint = mEventMarker.markers[marker.ZIndex];

			PointInfoView cpuv = PointInfoView.Create(selectedPoint);
			cpuv.PopUp(true, delegate
			{
				Console.WriteLine("cpuv will close");
			});

			return true;
		}
		#endregion
    }
}