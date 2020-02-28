using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Stocks.UI.Views {
	public class PortfolioItemCountView : ContentView {
		Label countLabel;
		SKCanvasView typeIndicator;

		public static readonly BindableProperty InsetsProperty = BindableProperty.Create(nameof(Insets), typeof(Thickness), typeof(PortfolioItemCountView), new Thickness(4), propertyChanged: OnInsetsPropertyChanged);
		public static readonly BindableProperty BeakSizeProperty = BindableProperty.Create(nameof(BeakSize), typeof(double), typeof(PortfolioItemCountView), default(double), propertyChanged: OnBeakSizePropertyChanged);
		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(PortfolioItemCountView), default(int), propertyChanged: OnCountPropertyChanged);
		public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(PortfolioItemCountView), Color.Default, propertyChanged: OnIndicatorColorPropertyChanged);
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColorProperty), typeof(Color), typeof(PortfolioItemCountView), Color.Default, propertyChanged: OnTextColorPropertyChanged);

        RelativeLayout layout;

		public Thickness Insets { get => (Thickness)GetValue(InsetsProperty); set => SetValue(InsetsProperty, value); }
		public double BeakSize { get => (double)GetValue(BeakSizeProperty); set => SetValue(BeakSizeProperty, value); }

		public int Count { get => (int)GetValue(CountProperty); set => SetValue(CountProperty, value); }
		public Color IndicatorColor { get => (Color)GetValue(IndicatorColorProperty); set => SetValue(IndicatorColorProperty, value); }
		public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

		public PortfolioItemCountView() {
			InitializeChildren();
            CompressedLayout.SetIsHeadless(this, true);
        }

		void InitializeChildren() {
            layout = new RelativeLayout();
            this.Content = layout; 
            CompressedLayout.SetIsHeadless(layout, true);

			typeIndicator = new SKCanvasView();
			typeIndicator.PaintSurface += OnPaintSurface;
			layout.Children.Add(
				typeIndicator,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => parent.Width),
				Constraint.RelativeToParent((parent) => parent.Height)
			);

			countLabel = new Label {
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};
			layout.Children.Add(
				countLabel,
				Constraint.FromExpression(() => Insets.Left),
				Constraint.FromExpression(() => Insets.Top),
				Constraint.RelativeToParent((parent) => parent.Width - Insets.HorizontalThickness - BeakSize),
				Constraint.RelativeToParent((parent) => parent.Height - Insets.VerticalThickness)
			);
		}

		void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e) {
			SKImageInfo info = e.Info;
			SKSurface surface = e.Surface;
			SKCanvas canvas = surface.Canvas;

			SKPath indicatorPath = new SKPath();
			float indicatorBeakX = (float)(e.Info.Width - BeakSize * DeviceDisplay.MainDisplayInfo.Density);
			float indicatorCenterY = (float)info.Height / 2;
			indicatorPath.MoveTo(new SKPoint(0.0f, 0.0f));
			indicatorPath.LineTo(new SKPoint(indicatorBeakX, 0.0f));
			indicatorPath.LineTo(new SKPoint(info.Width, indicatorCenterY));
			indicatorPath.LineTo(new SKPoint(indicatorBeakX, info.Height));
			indicatorPath.LineTo(new SKPoint(0.0f, info.Height));
			indicatorPath.Close();

			Color actualColor = IndicatorColor;
			SKPaint fillPaint = new SKPaint {
				Color = new SKColor(
					(byte)(255 * actualColor.R),
					(byte)(255 * actualColor.G),
					(byte)(255 * actualColor.B),
					(byte)(255 * actualColor.A))
			};
			canvas.Clear(SKColors.Transparent);
			canvas.DrawPath(indicatorPath, fillPaint);
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
			SizeRequest labelSize = countLabel.Measure(widthConstraint, heightConstraint);
			Size sizeRequest = new Size(
				labelSize.Request.Width + Insets.HorizontalThickness + Padding.HorizontalThickness + BeakSize,
				labelSize.Request.Height + Insets.VerticalThickness + Padding.VerticalThickness);
			labelSize.Request = sizeRequest;
			labelSize.Minimum = sizeRequest;

			return new SizeRequest(sizeRequest, sizeRequest);
		}

		void InvalidateAll() {
			InvalidateMeasure();
			typeIndicator.InvalidateSurface();
		}
		void OnCountChanged() {
			countLabel.Text = $"{Count:n0}";
			InvalidateMeasure();
			typeIndicator.InvalidateSurface();
		}
		void OnIndicatorColorChanged() {
			typeIndicator.InvalidateSurface();
		}
		void OnTextColorChanged() {
			countLabel.TextColor = TextColor;
		}

		static void OnInsetsPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is PortfolioItemCountView countView)) return;
			countView.InvalidateAll();
		}
		static void OnCountPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is PortfolioItemCountView countView)) return;
			countView.OnCountChanged();
		}
		static void OnIndicatorColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is PortfolioItemCountView countView)) return;
			countView.OnIndicatorColorChanged();
		}
		static void OnTextColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is PortfolioItemCountView countView)) return;
			countView.OnTextColorChanged();
		}

		static void OnBeakSizePropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is PortfolioItemCountView countView)) return;
			countView.InvalidateAll();
		}
	}
}
