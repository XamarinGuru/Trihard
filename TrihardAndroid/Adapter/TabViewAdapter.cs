using Android.Support.V4.App;
using System;

namespace goheja
{
    public class TabViewAdapter : FragmentPagerAdapter
    {
        public event EventHandler TabChanged;

        public TabViewAdapter(FragmentManager fm, FragmentActivity activity) : base(fm)
        {
        }

        public override int Count
        {
            get { return 3; }
        }

        public override Fragment GetItem(int position)
        {
			if (position == 0)
                return new FragmentCalendar();
            if (position == 1)
				return new FragmentEvents();
            if (position == 2)
				return new FragmentProfile();

            TabChanged?.Invoke(position, null);

            return null;
        }

    }
}

