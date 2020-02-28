using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Stocks.iOS {
    public class DialogContentContainer : UIView {
        readonly RendererContainer content;

        public DialogContentContainer(RendererContainer content) {
            BackgroundColor = Color.Transparent.ToUIColor();
            this.content = content;
            AddSubview(content);
        }

        public override CGRect Frame {
            get => base.Frame;
            set {
                base.Frame = value;
                if (content != null) {
                    CGSize contentSize = content.SizeThatFits(new CGSize(Bounds.Width, Bounds.Height));
                    content.Bounds = new CGRect(0, 0, contentSize.Width, contentSize.Height);
                }
            }
        }

        public override void LayoutSubviews() {
            if (content != null) 
                content.Frame = new CGRect(Bounds.Width / 2 - content.Bounds.Width / 2, Bounds.Height / 2 - content.Bounds.Height / 2, content.Bounds.Width, content.Bounds.Height);
        }
    }

    public class DialogViewController : UIViewController, IUIGestureRecognizerDelegate {
        UITapGestureRecognizer tapOutsideRecognizer;
        RendererContainer content;

        public DialogViewController(RendererContainer content) {
            this.content = content;
            View = new DialogContentContainer(content);

            ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            ModalInPopover = true;
        }

        public override void ViewDidAppear(bool animated) {
            base.ViewDidAppear(animated);

            if (tapOutsideRecognizer == null) {
                tapOutsideRecognizer = new UITapGestureRecognizer(OnTap);
                tapOutsideRecognizer.NumberOfTapsRequired = 1;
                tapOutsideRecognizer.CancelsTouchesInView = false;
                tapOutsideRecognizer.Delegate = this;
                View.Window?.AddGestureRecognizer(tapOutsideRecognizer);
            }
        }

        public override void ViewWillDisappear(bool animated) {
            base.ViewWillDisappear(animated);
            RemoveTapRecognizerIfNeed();
        }

        void RemoveTapRecognizerIfNeed() {
            if (tapOutsideRecognizer != null) {
                View.Window?.RemoveGestureRecognizer(tapOutsideRecognizer);
                tapOutsideRecognizer = null;
            }
        }

        void OnTap() {
            if (this.tapOutsideRecognizer.State == UIGestureRecognizerState.Ended) {
                CGPoint location = this.tapOutsideRecognizer.LocationInView(null);

                if (UIApplication.SharedApplication.StatusBarOrientation.IsLandscape()) 
                    location = new CGPoint(location.Y, location.X);

                if (this.content.ContainsInFrame(location))
                    return;

                RemoveTapRecognizerIfNeed();
                DismissModalViewController(false);
            }
        }
    }
}
