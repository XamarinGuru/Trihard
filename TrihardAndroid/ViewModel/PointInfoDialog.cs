
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "PointInfoDialog")]
	public class PointInfoDialog : DialogFragment
	{
		Point point;

		public static PointInfoDialog newInstance(Point point)
		{
			PointInfoDialog inputDialog = new PointInfoDialog();

			inputDialog.point = point;

			return inputDialog;
		}

		public PointInfoDialog()
		{
			// Required empty public constructor
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			return dialog;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var infoView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_PointInfoView, null);

			infoView.FindViewById<TextView>(Resource.Id.lblName).Text = "Name: " + point.name;
			infoView.FindViewById<TextView>(Resource.Id.lblDescription).Text = point.description;
			infoView.FindViewById<TextView>(Resource.Id.lblInterval).Text = "Interval: " + point.interval;

			infoView.FindViewById<ImageView>(Resource.Id.ActionClose).Click += (sender, e) => Dismiss();
			infoView.FindViewById<Button>(Resource.Id.ActionNavigate).Click += (sender, e) => ActionNavigate();

			return infoView;
		}

		void ActionNavigate()
		{
			try
			{
				string url = string.Format("waze://?ll={0},{1}&navigate=yes", point.lat, point.lng);
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
				StartActivity(intent);
			}
			catch (ActivityNotFoundException ex)
			{
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.waze"));
				StartActivity(intent);
			}
		}
	}
}
