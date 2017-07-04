
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Java.Lang;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "LocationActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class LocationActivity : BaseActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
	{
		const int Location_Request_Code = 0;

		EventPoints mEventMarker = new EventPoints();
		IList<string> pointIDs;

		SupportMapFragment mMapViewFragment;
		GoogleMap mMapView = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LocationActivity);

			mMapViewFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mMapViewFragment.GetMapAsync(this);
		}

		#region google map

		public void OnMapReady(GoogleMap googleMap)
		{
			mMapView = googleMap;

			if (mMapView != null)
			{
				mMapView.SetOnMarkerClickListener(this);

				GetMarkersAndPoints();
			}
		}

		#endregion
		void GetMarkersAndPoints()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetAllMarkers(AppSettings.selectedEvent._id);
				var trackPoints = GetTrackPoints(AppSettings.selectedEvent._id);

				HideLoadingView();

				var boundPoints = new List<LatLng>();

				pointIDs = new List<string>();

				RunOnUiThread(() =>
				{
					try
					{
						if (mEventMarker != null && mEventMarker.markers.Count > 0)
						{
							for (int i = 0; i < mEventMarker.markers.Count; i++)
							{
								var point = mEventMarker.markers[i];
								var pointLocation = new LatLng(point.lat, point.lng);
								boundPoints.Add(pointLocation);

								AddMapPin(pointLocation, point.type);
							}
						}

						if (trackPoints != null && trackPoints.Count > 0)
						{
							if (trackPoints[0].Count > 0)
							{
								var startPoint = trackPoints[0][0];
								var endPoint = trackPoints[trackPoints.Count - 1][trackPoints[trackPoints.Count - 1].Count - 1];
								var startLocation = new LatLng(startPoint.Latitude, startPoint.Longitude);
								var endLocation = new LatLng(endPoint.Latitude, endPoint.Longitude);
								AddMapPin(startLocation, "pSTART");
								AddMapPin(endLocation, "pFINISH");
							}

							for (int i = 0; i < trackPoints.Count; i++)
							{
								var tPoints = trackPoints[i];
								List<LatLng> paths = new List<LatLng>();
								LatLng[] arrPath;
								for (int j = 0; j < tPoints.Count; j++)
								{
									var tPoint = tPoints[j];
									var tLocation = new LatLng(tPoint.Latitude, tPoint.Longitude);

									if (j < tPoints.Count - 1)
									{
										var distance = DistanceAtoB(tPoint, tPoints[j + 1]);

										if (Constants.AVAILABLE_DISTANCE_MAP > distance)
										{
											var nPoint = tPoints[j + 1];
											paths.Add(tLocation);
										}
										else
										{
											arrPath = new LatLng[paths.Count];
											for (var k = 0; k < paths.Count; k++)
												arrPath[k] = paths[k];
											mMapView.AddPolyline(new PolylineOptions().Add(arrPath).InvokeColor(GetRandomColor(i)).InvokeWidth(5f));

											paths = new List<LatLng>();
										}
									}

									boundPoints.Add(tLocation);
								}

								arrPath = new LatLng[paths.Count];
								for (var k = 0; k < paths.Count; k++)
									arrPath[k] = paths[k];
								mMapView.AddPolyline(new PolylineOptions().Add(arrPath).InvokeColor(GetRandomColor(i)).InvokeWidth(5f));

								paths = new List<LatLng>();
							}
						}

						if (boundPoints.Count == 0)
						{
							var location = new LatLng(Constants.LOCATION_ISURAEL[0], Constants.LOCATION_ISURAEL[1]);
							CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(location, Constants.MAP_ZOOM_LEVEL);
							mMapView.MoveCamera(cu_center);
						}
						else
						{
							var mapBounds = new LatLngBounds.Builder();
							foreach (var bound in boundPoints)
								mapBounds.Include(bound);
							mMapView.MoveCamera(CameraUpdateFactory.NewLatLngBounds(mapBounds.Build(), 50));
						}
					}
					catch (Exception ex)
					{
						ShowTrackMessageBox(ex.Message);
					}
				});
			});
		}

		void AddMapPin(LatLng position, string type)
		{
			try
			{
				MarkerOptions markerOpt = new MarkerOptions();
				markerOpt.SetPosition(position);

				var metrics = Resources.DisplayMetrics;
				var wScreen = metrics.WidthPixels;

				Bitmap bmp = GetPinIconByType(type);
				Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 7, true);
				markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

				RunOnUiThread(() =>
				{
					var marker = mMapView.AddMarker(markerOpt);
					pointIDs.Add(marker.Id);
				});
			}
			catch (Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}
		}

		public bool OnMarkerClick(Marker marker)
		{
			try
			{
				PortableLibrary.Point selectedPoint = null;
				for (var i = 0; i < pointIDs.Count; i++)
				{
					if (marker.Id == pointIDs[i])
						selectedPoint = mEventMarker.markers[i];
				}
				if (selectedPoint == null) return false;

				PointInfoDialog myDiag = PointInfoDialog.newInstance(selectedPoint);
				myDiag.Show(FragmentManager, "Diag");
			}
			catch(Exception ex)
			{
				ShowTrackMessageBox(ex.Message);
			}

			return true;
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				ActionBackCancel();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
