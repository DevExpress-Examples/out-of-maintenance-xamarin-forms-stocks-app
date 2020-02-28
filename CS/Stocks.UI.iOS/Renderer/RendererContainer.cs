using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Stocks.iOS {
    public class RendererContainer : UIView {
        public IVisualElementRenderer Renderer { get; }

        public RendererContainer(IVisualElementRenderer renderer) {
            Renderer = renderer;
            AddSubview(renderer.NativeView);
        }

        public override CGSize SizeThatFits(CGSize size) {
            SizeRequest measureSizeRequest = Renderer.Element.Measure(size.Width, size.Height);
            Size measureChildSize = measureSizeRequest.Request;
            return new CGSize(measureChildSize.Width, measureChildSize.Height);
        }

        public bool ContainsInFrame(CGPoint point) {
            return Frame.Contains(point.X, point.Y);
        }

#pragma warning disable RECS0018
        public override void LayoutSubviews() {
            Layout.LayoutChildIntoBoundingRegion(Renderer.Element, new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height));
        }
#pragma warning restore RECS0018
    }
}
