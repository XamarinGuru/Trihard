using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using System;

namespace goheja
{
    public class FragmentEvents : Android.Support.V4.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			return inflater.Inflate(Resource.Layout.fEvents, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

			view.FindViewById<ImageView>(Resource.Id.ActionType0).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType1).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType2).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType3).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType4).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType5).Click += ActionSelectedType;
			view.FindViewById<ImageView>(Resource.Id.ActionType6).Click += ActionSelectedType;
        }

		void ActionSelectedType(object sender, EventArgs e)
		{
			var practiceType = int.Parse((sender as ImageView).Tag.ToString());
			var intent = new Intent(Activity, typeof(AnalyticsActivity));
			intent.PutExtra("pType", practiceType);
			StartActivityForResult(intent, 1);
		}
    }
}
