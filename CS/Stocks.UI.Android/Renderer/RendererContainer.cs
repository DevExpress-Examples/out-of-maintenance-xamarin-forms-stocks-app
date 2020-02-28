using System;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Stocks.UI.Android {
    public class RendererContainer : ViewGroup {
        IVisualElementRenderer renderer;

        public RendererContainer(Context context, IVisualElementRenderer renderer)
            : base(context) {
            this.renderer = renderer;
            AddView(renderer.View);
        }

        internal VisualElement Element { get { return this.renderer.Element; } }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            double avWidth = widthMode == MeasureSpecMode.Unspecified ? double.PositiveInfinity : Context.FromPixels(MeasureSpec.GetSize(widthMeasureSpec));
            double avHeight = heightMode == MeasureSpecMode.Unspecified ? double.PositiveInfinity : Context.FromPixels(MeasureSpec.GetSize(heightMeasureSpec));
            Size size = renderer.Element.Measure(avWidth, avHeight).Request;
            SetMeasuredDimension(CalculateSize(widthMode, avWidth, size.Width), CalculateSize(heightMode, avHeight, size.Height));
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b) {
            double width = Context.FromPixels(r - l);
            double height = Context.FromPixels(b - t);
            Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(renderer.Element, new Rectangle(0, 0, width, height));
            renderer.UpdateLayout();
        }

        int CalculateSize(MeasureSpecMode specMode, double availableSize, double measuredSize) {
            double size;
            switch (specMode) {
                case MeasureSpecMode.Exactly:
                size = availableSize;
                break;
                case MeasureSpecMode.AtMost:
                size = Math.Min(measuredSize, availableSize);
                break;
                case MeasureSpecMode.Unspecified:
                size = measuredSize;
                break;
                default:
                size = 0;
                break;
            }
            return (int)Context.ToPixels(size);
        }
    }
}
