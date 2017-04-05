using System;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.GrapeCity.Xuni.FlexChart;
using PortableLibrary;

using EventArgs = System.EventArgs;
using Com.GrapeCity.Xuni.ChartCore;
using Android.Graphics;

namespace goheja
{
	public class FragmentCalendar : Android.Support.V4.App.Fragment
	{
		int ALPHA_FILL = Convert.ToInt32(0.3 * 255);
		int ALPHA_AXIS = Convert.ToInt32(0.8 * 255);

		SwipeTabActivity rootActivity;

		public View mView;

		FlexChart mPChart;
		ChartRectangleAnnotation annoFocused = new ChartRectangleAnnotation();

		TextView lblCycleDuration, lblRunDuration, lblSwimDuration, lblCycleDistance, lblRunDistance, lblSwimDistance, lblCycleStress, lblRunStress, lblSwimStress;
		ImageView btnCycle, btnRun, btnSwim;
		LinearLayout viewCycle, viewRun, viewSwim;

		ReportGraphData pData;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			rootActivity = this.Activity as SwipeTabActivity;
			return inflater.Inflate(Resource.Layout.fCalendar, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			mView = view;

			SetUISettings();

			if (!rootActivity.IsNetEnable()) return;
		}

		public override void OnResume()
		{
			base.OnResume();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				rootActivity.ShowLoadingView("Loading data...");

				var performanceData = rootActivity.GetPerformance();
				var gaugeData = rootActivity.GetGauge();

				rootActivity.HideLoadingView();

				rootActivity.RunOnUiThread(() =>
				{
					InitPerformanceGraph(performanceData);
					InitGaugeData(gaugeData);
				});
			});
		}

		private void SetUISettings()
		{
			#region UI Variables
			lblCycleDuration = mView.FindViewById<TextView>(Resource.Id.lblCycleDuration);
			lblRunDuration = mView.FindViewById<TextView>(Resource.Id.lblRunDuration);
			lblSwimDuration = mView.FindViewById<TextView>(Resource.Id.lblSwimDuration);
			lblCycleDistance = mView.FindViewById<TextView>(Resource.Id.lblCycleDistance);
			lblRunDistance = mView.FindViewById<TextView>(Resource.Id.lblRunDistance);
			lblSwimDistance = mView.FindViewById<TextView>(Resource.Id.lblSwimDistance);
			lblCycleStress = mView.FindViewById<TextView>(Resource.Id.lblCycleStress);
			lblRunStress = mView.FindViewById<TextView>(Resource.Id.lblRunStress);
			lblSwimStress = mView.FindViewById<TextView>(Resource.Id.lblSwimStress);

			mView.FindViewById<TextView>(Resource.Id.lblCycleDurationTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblRunDurationTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblSwimDurationTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblCycleDistanceTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblRunDistanceTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblSwimDistanceTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblCycleStressTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblRunStressTitle).SetTextColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<TextView>(Resource.Id.lblSwimStressTitle).SetTextColor(rootActivity.GROUP_COLOR);

			lblCycleDuration.SetTextColor(rootActivity.GROUP_COLOR);
			lblRunDuration.SetTextColor(rootActivity.GROUP_COLOR);
			lblSwimDuration.SetTextColor(rootActivity.GROUP_COLOR);
			lblCycleDistance.SetTextColor(rootActivity.GROUP_COLOR);
			lblRunDistance.SetTextColor(rootActivity.GROUP_COLOR);
			lblSwimDistance.SetTextColor(rootActivity.GROUP_COLOR);
			lblCycleStress.SetTextColor(rootActivity.GROUP_COLOR);
			lblRunStress.SetTextColor(rootActivity.GROUP_COLOR);
			lblSwimStress.SetTextColor(rootActivity.GROUP_COLOR);

			mView.FindViewById<LinearLayout>(Resource.Id.bgSymbolCycling).SetBackgroundColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<LinearLayout>(Resource.Id.bgSymbolRunning).SetBackgroundColor(rootActivity.GROUP_COLOR);
			mView.FindViewById<LinearLayout>(Resource.Id.bgSymbolSwimming).SetBackgroundColor(rootActivity.GROUP_COLOR);

			btnCycle = mView.FindViewById<ImageView>(Resource.Id.btnCycle);
			btnRun = mView.FindViewById<ImageView>(Resource.Id.btnRun);
			btnSwim = mView.FindViewById<ImageView>(Resource.Id.btnSwim);

			viewCycle = mView.FindViewById<LinearLayout>(Resource.Id.viewCycle);
			viewRun = mView.FindViewById<LinearLayout>(Resource.Id.viewRun);
			viewSwim = mView.FindViewById<LinearLayout>(Resource.Id.viewSwim);

			CollepseAnimation(viewCycle);
			CollepseAnimation(viewRun);
			CollepseAnimation(viewSwim);
			#endregion

			#region Actions
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsCycle).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsRun).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsSwim).Click += ActionCollepse;
			mView.FindViewById<Button>(Resource.Id.ActionViewCalendar).Click += ActionViewCalendar;

			mView.FindViewById<Button>(Resource.Id.ActionViewCalendar).SetBackgroundColor(rootActivity.GROUP_COLOR);
			//toggle series visibility
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleTSB).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleATL).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleCTL).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleDailyLoad).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleDailyIf).Click += ActionToggleSeries;
			#endregion
		}


		void InitPerformanceGraph(ReportGraphData pData)
		{
			this.pData = pData;

			mView.FindViewById<ScrollView>(Resource.Id.scrollView).ScrollTo(0, 0);
			if (pData == null) return;

			var chartContent = mView.FindViewById<LinearLayout>(Resource.Id.chartContent);
			chartContent.RemoveAllViews();

			try
			{
				mPChart = new FlexChart(this.Activity);
				LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
				mPChart.LayoutParameters = params1;

				chartContent.AddView(mPChart);

				#region configure
				mPChart.SetPalette(Palettes.Modern);
				mPChart.SetBackgroundColor(Color.Transparent);
				mPChart.ChartType = ChartType.Splinearea;
				mPChart.BindingX = pData.categoryField;
				mPChart.Animated = false;
				#endregion

				#region regend
				mPChart.Legend.Position = ChartPositionType.None;
				#endregion

				#region axis
				mPChart.AxisX.LabelsVisible = false;
				mPChart.AxisX.MajorTickWidth = 0;
				mPChart.AxisX.LineWidth = 0.5f;

				mPChart.AxisY.LabelsVisible = false;
				mPChart.AxisY.LineColor = new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, ALPHA_AXIS);
				mPChart.AxisY.LineWidth = 2;

				for (int i = 0; i < pData.valueAxes.Count; i++)
				{
					var axis = pData.valueAxes[i];
					ChartAxis cAxis = new ChartAxis(mPChart, i % 2 == 0 ? ChartPositionType.Left : ChartPositionType.Right);
					cAxis.Name = axis.id;
					cAxis.LineColor = Color.ParseColor(axis.axisColor);
					cAxis.MajorTickWidth = 0;
					cAxis.MajorGridVisible = false;
					cAxis.LabelsVisible = false;
					cAxis.AxisLineVisible = false;

					mPChart.Axes.Add(cAxis);
				}
				#endregion

				#region series
				foreach (var series in pData.graphs)
				{
					ChartSeries cSeries = new ChartSeries(mPChart, series.title, series.valueField);
					cSeries.AxisY = series.valueAxis;

					Color sColor = Color.ParseColor(series.lineColor);
					var sRGBA = new Color(sColor.R, sColor.G, sColor.B, ALPHA_FILL);
					cSeries.SetColor(new Java.Lang.Integer(sRGBA.ToArgb()));

					ImageView imgSymbol = new ImageView(this.Context);
					switch (series.valueField)
					{
						case "tsb":
							imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symTSB);
							break;
						case "atl":
							imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symATL);
							break;
						case "ctl":
							imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symCTL);
							break;
						case "dayliTss":
							imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symDailyTSS);
							break;
						case "dayliIf":
							imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symDailyIF);
							break;
					}
					imgSymbol.SetBackgroundColor(sColor);

					if (series.lineAlpha.Equals(0))
					{
						cSeries.ChartType = ChartType.Scatter;
						cSeries.SymbolSize = new Java.Lang.Float(1.5f);
						cSeries.SymbolColor = new Java.Lang.Integer(sColor);
						cSeries.SetColor(new Java.Lang.Integer(sColor));
					}
					else
					{
						cSeries.BorderWidth = 0.5f;
						cSeries.SetColor(new Java.Lang.Integer(sRGBA.ToArgb()));
					}

					mPChart.Series.Add(cSeries);
				}
				#endregion

				#region annotation
				if (pData.TodayIndex() != -1)
				{
					ChartRectangleAnnotation today = new ChartRectangleAnnotation();
					today.Attachment = ChartAnnotationAttachment.DataIndex;
					today.PointIndex = pData.TodayIndex();
					today.Width = 3;
					today.Height = 10000;

					today.Color = Color.Black;
					today.BorderWidth = 0;
					today.FontSize = 10;
					today.TextColor = Color.White.ToArgb();
					today.TooltipText = "Future planned performance";
					mPChart.Annotations.Add(today);
				}

				annoFocused.Attachment = ChartAnnotationAttachment.DataIndex;
				annoFocused.PointIndex = pData.TodayIndex();
				annoFocused.Width = 1;
				annoFocused.Height = 10000;
				annoFocused.Color = Color.White;
				annoFocused.BorderWidth = 0;
				annoFocused.Visible = false;
				annoFocused.FontSize = 12;
				annoFocused.TextColor = Color.White.ToArgb();
				mPChart.Annotations.Add(annoFocused);
				#endregion

				mPChart.ItemsSource = pData.GetSalesDataList();

				#region custom tooltip
				mPChart.Tooltip.Content = new MyTooltip(mPChart, this, pData, annoFocused);
				#endregion
				mPChart.ZoomMode = ZoomMode.X;
				mPChart.AxisX.Scale = 1;
			}
			catch (Exception ex)
			{
				rootActivity.ShowTrackMessageBox(ex.Message);
			}
		}

		void InitGaugeData(Gauge gaugeData)
		{
			if (gaugeData == null) return;

			try
			{

				lblCycleDuration.Text = rootActivity.FormatNumber(gaugeData.Bike[0].value) + "%";
				lblRunDuration.Text = rootActivity.FormatNumber(gaugeData.Run[0].value) + "%";
				lblSwimDuration.Text = rootActivity.FormatNumber(gaugeData.Swim[0].value) + "%";

				lblCycleDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";
				lblRunDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";
				lblSwimDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";

				lblCycleStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
				lblRunStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
				lblSwimStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
			}
			catch (Exception ex)
			{
				rootActivity.ShowTrackMessageBox(ex.Message);
			}
		}

		void ActionViewCalendar(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(EventCalendarActivity));
			StartActivityForResult(intent, 1);
		}

		#region Action Collepse

		void ActionToggleSeries(object sender, EventArgs e)
		{
			var sIndex = int.Parse(((LinearLayout)sender).Tag.ToString());
			var series = mPChart.Series.Get(sIndex) as ChartSeries;
			var sVisibility = series.SeriesVisibility == ChartSeriesVisibilityType.Hidden ? ChartSeriesVisibilityType.Visible : ChartSeriesVisibilityType.Hidden;

			series.SetVisibility(sVisibility);
		}

		void ActionCollepse(object sender, EventArgs e)
		{
			switch (int.Parse(((RelativeLayout)sender).Tag.ToString()))
			{
				case 0:
					btnCycle.SetImageResource(viewCycle.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewCycle);
					break;
				case 1:
					btnRun.SetImageResource(viewRun.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewRun);
					break;
				case 2:
					btnSwim.SetImageResource(viewSwim.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewSwim);
					break;
				default:
					break;
			}
		}

		void CollepseAnimation(LinearLayout content)
		{
			if (content.Visibility.Equals(ViewStates.Gone))
			{
				content.Visibility = ViewStates.Visible;

				int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				content.Measure(widthSpec, heightSpec);

				ValueAnimator mAnimator = slideAnimator(0, content.MeasuredHeight, content);
				mAnimator.Start();
			}
			else
			{
				int finalHeight = content.Height;

				ValueAnimator mAnimator = slideAnimator(finalHeight, 0, content);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					content.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, LinearLayout content)
		{
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				var value = (int)animator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = content.LayoutParameters;
				layoutParams.Height = value;
				content.LayoutParameters = layoutParams;
			};
			return animator;
		}
		#endregion
	}

#region custom tooltip
	public class MyTooltip : BaseChartTooltipView
	{
		FragmentCalendar mContext;
		ChartRectangleAnnotation mAnnoFocused;

		ReportGraphData mData;

		public MyTooltip(FlexChart chart, FragmentCalendar context, ReportGraphData data, ChartRectangleAnnotation annoFocused) : base(chart)
		{
			mContext = context;
			mAnnoFocused = annoFocused;
			mData = data;
		}
		public override void Render(SuperChartDataPoint point)
		{
			try
			{
				var data = mData.dataProvider[point.PointIndex];
				mAnnoFocused.PointIndex = point.PointIndex;
				mAnnoFocused.Text = String.Format("Date: {0}", data.date);
				mAnnoFocused.Visible = true;

				mContext.mView.FindViewById<TextView>(Resource.Id.txtTSB).Text = String.Format("TSB: {0}", data.tsb);
				mContext.mView.FindViewById<TextView>(Resource.Id.txtATL).Text = String.Format("ATL: {0}", data.atl);
				mContext.mView.FindViewById<TextView>(Resource.Id.txtCTL).Text = String.Format("CTL: {0}", data.ctl);
				mContext.mView.FindViewById<TextView>(Resource.Id.txtDailyTSS).Text = String.Format("Daily Load: {0}", data.dayliTss);
				mContext.mView.FindViewById<TextView>(Resource.Id.txtDailyIF).Text = String.Format("Day Intencity: {0}", data.dayliIf);
			}
			catch (Exception err)
			{
			}
		}
	}
#endregion
}
