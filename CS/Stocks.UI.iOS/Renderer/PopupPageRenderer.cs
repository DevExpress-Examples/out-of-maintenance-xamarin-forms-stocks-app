using Stocks.iOS.Renderer;
using Stocks.UI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Stocks.iOS.Renderer {
    public class PopupPageRenderer : PageRenderer {
        PopupPage CurrentElement => (PopupPage)Element;

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
        }

        public override void ViewWillAppear(bool animated) {
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated) {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLayoutSubviews() {
            base.ViewDidLayoutSubviews();

            var currentElement = CurrentElement;

            if (View?.Superview?.Frame == null || currentElement == null)
                return;

            var superviewFrame = View.Superview.Frame;
            var applactionFrame = UIScreen.MainScreen.ApplicationFrame;

            Thickness systemPadding;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0)) {
                var safeAreaInsets = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;

                systemPadding = new Thickness(
                    safeAreaInsets.Left,
                    safeAreaInsets.Top,
                    safeAreaInsets.Right,
                    safeAreaInsets.Bottom);
            }
            else {
                systemPadding = new Thickness {
                    Left = applactionFrame.Left,
                    Top = applactionFrame.Top,
                    Right = applactionFrame.Right - applactionFrame.Width - applactionFrame.Left,
                    Bottom = applactionFrame.Bottom - applactionFrame.Height - applactionFrame.Top
                };
            }

            currentElement.SetValue(PopupPage.SystemPaddingProperty, systemPadding);

            if (Element != null)
                SetElementSize(new Size(superviewFrame.Width, superviewFrame.Height));

            currentElement.ForceLayout();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
