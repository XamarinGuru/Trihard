
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	public class TimeFormatDialog : DialogFragment
	{
		NumberPicker[] numPickers;
		int numDials;
		string type;
		string title;
		static string ARG_numDials = "numDials";
		EditText textView;

		public static TimeFormatDialog newInstance(EditText textView, int numDials, string type, string title)
		{
			TimeFormatDialog numdialog = new TimeFormatDialog();
			numdialog.textView = textView;
			numdialog.type = type;
			numdialog.title = title;
			Bundle args = new Bundle();
			args.PutInt(ARG_numDials, numDials);
			numdialog.Arguments = args;
			return numdialog;
		}

		public TimeFormatDialog()
		{
			// Required empty public constructor
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.SetTitle(title);
			return dialog;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (Arguments != null)
			{
				numDials = Arguments.GetInt(ARG_numDials);
				numPickers = new NumberPicker[numDials];
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LinearLayout linLayoutH = new LinearLayout(this.Activity);

			LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			linLayoutH.LayoutParameters = params1;

			if (type == "hr")
			{
				var minValue = 60;
				var maxValue = 250;
				for (int i = 0; i < numDials; i++)
				{
					numPickers[numDials - i - 1] = new NumberPicker(this.Activity);
					numPickers[numDials - i - 1].MaxValue = 19;
					numPickers[numDials - i - 1].MinValue = 0;
					numPickers[numDials - i - 1].Value = 0;

					string[] valSet = new string[20];
					for (int val = minValue; val <= maxValue; val += 10)
					{
						valSet[(val - 60) / 10] = val.ToString();
					}
					numPickers[numDials - i - 1].SetDisplayedValues(valSet);
					linLayoutH.AddView(numPickers[numDials - i - 1]);
				}
			}
			else {
				var maxValue = numDials == 1 ? 5 : 59;
				var minValue = numDials == 1 ? 1 : 0;
				for (int i = 0; i < numDials; i++)
				{
					numPickers[numDials - i - 1] = new NumberPicker(this.Activity);
					numPickers[numDials - i - 1].MaxValue = maxValue;
					numPickers[numDials - i - 1].MinValue = minValue;
					numPickers[numDials - i - 1].Value = 0;
					linLayoutH.AddView(numPickers[numDials - i - 1]);
				}
			}

			LinearLayout linLayoutV = new LinearLayout(this.Activity);
			linLayoutV.Orientation = Orientation.Vertical;
			linLayoutV.AddView(linLayoutH);

			Button okButton = new Button(this.Activity);
			okButton.Click += (sender, e) => {
				string strText = "";
				if (type == "hr")
				{
					strText = (60 + numPickers[1].Value * 10) + "-" + (60 + numPickers[0].Value * 10);
				}else if (numDials == 1)
				{
					strText = numPickers[0].Value.ToString();
				}
				else {
					strText = String.Format("{0:00}", numPickers[1].Value) + ":" + String.Format("{0:00}", numPickers[0].Value);
					if (numDials > 2)
						strText = String.Format("{0:00}", numPickers[2].Value) + ":" + strText;
				}
				textView.Text = strText;
				Dismiss();
			};

			params1.Gravity = GravityFlags.CenterHorizontal;
			okButton.LayoutParameters = params1;
			okButton.Text = "Done";

			linLayoutV.AddView(okButton);
			return linLayoutV;
		}

		public void onSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutInt("CurrentValue", getValue());
		}

		private int getValue()
		{
			int value = 0;
			int mult = 1;
			for (int i = 0; i < numDials; i++)
			{
				value += numPickers[i].Value * mult;
				mult *= 10;
			}
			return value;
		}
	}

	#region adjust dialog
	public class AdjustDialog : DialogFragment
	{
		TextView textView;
		SeekBar seekBar;
		int maxValue;
		bool isType;

		public static AdjustDialog newInstance(TextView textView, SeekBar seekBar, int maxValue, bool isType = false)
		{
			AdjustDialog numdialog = new AdjustDialog();
			numdialog.textView = textView;
			numdialog.seekBar = seekBar;
			numdialog.maxValue = maxValue;
			numdialog.isType = isType;

			return numdialog;
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.SetTitle("");
			return dialog;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LinearLayout linLayoutH = new LinearLayout(this.Activity);

			LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			linLayoutH.LayoutParameters = params1;

			var numPicker =  new NumberPicker(this.Activity);
			numPicker.MaxValue = maxValue;
			numPicker.MinValue = 0;
			if (isType)
			{
				numPicker.SetDisplayedValues(Constants.PRACTICE_TYPES);
				numPicker.Value = Array.IndexOf(Constants.PRACTICE_TYPES, textView.Text);
			}
			else {
				numPicker.Value = (int)float.Parse(textView.Text);
			}

			linLayoutH.AddView(numPicker);


			LinearLayout linLayoutV = new LinearLayout(this.Activity);
			linLayoutV.Orientation = Orientation.Vertical;
			linLayoutV.AddView(linLayoutH);

			Button okButton = new Button(this.Activity);
			okButton.Click += (sender, e) =>
			{
				textView.Text = numPicker.Value.ToString();

				if (isType)
				{
					textView.Text = Constants.PRACTICE_TYPES[numPicker.Value];
				}
				else
				{
					seekBar.Progress = numPicker.Value * 10;
				}
				
				Dismiss();
			};

			params1.Gravity = GravityFlags.CenterHorizontal;
			okButton.LayoutParameters = params1;
			okButton.Text = "Done";

			linLayoutV.AddView(okButton);
			return linLayoutV;

		}

		public void onSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}
	}
	#endregion
}
