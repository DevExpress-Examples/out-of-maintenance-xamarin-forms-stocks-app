using System.Threading.Tasks;
using Stocks.iOS.Services;
using Stocks.UI;
using Stocks.UI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFPlatform = Xamarin.Forms.Platform.iOS.Platform;

[assembly: Dependency(typeof(StocksApplicationServiceImpl))]
namespace Stocks.iOS.Services {
    public class StocksApplicationServiceImpl : StocksApplicationService {
        bool IsiOS9OrNewer => UIDevice.CurrentDevice.CheckSystemVersion(9, 0);

        public override async Task ShowPopupPageAsync(PopupPage popupPage) {
            var renderer = GetOrCreateRenderer(popupPage);
            await ShowPopupElementAsync(popupPage, renderer.ViewController);
        }

        public override async Task ClosePopupPageAsync(PopupPage popupPage) {
            var renderer = XFPlatform.GetRenderer(popupPage);
            var viewController = renderer?.ViewController;
            await ClosePopupElementAsync(popupPage, renderer, viewController);
        }

        IVisualElementRenderer GetOrCreateRenderer(VisualElement popupPage) {
            var renderer = XFPlatform.GetRenderer(popupPage);
            if (renderer == null) {
                renderer = XFPlatform.CreateRenderer(popupPage);
                XFPlatform.SetRenderer(popupPage, renderer);
            }
            return renderer;
        }

        async Task ShowPopupElementAsync(VisualElement element, UIViewController viewController) {
            element.Parent = Application.Current.MainPage;
            element.DescendantRemoved += HandleChildRemoved;

            var window = UIApplication.SharedApplication.KeyWindow;
            await window?.RootViewController.PresentViewControllerAsync(viewController, false);
        }

        void HandleChildRemoved(object sender, ElementEventArgs e) {
            var view = e.Element;
            DisposeModelAndChildrenRenderers((VisualElement)view);
        }
        void DisposeModelAndChildrenRenderers(VisualElement view) {
            IVisualElementRenderer renderer;
            foreach (VisualElement child in view.Descendants()) {
                renderer = XFPlatform.GetRenderer(child);
                XFPlatform.SetRenderer(child, null);

                if (renderer != null) {
                    renderer.NativeView.RemoveFromSuperview();
                    renderer.Dispose();
                }
            }

            renderer = XFPlatform.GetRenderer(view);
            if (renderer != null) {
                renderer.NativeView.RemoveFromSuperview();
                renderer.Dispose();
            }
            XFPlatform.SetRenderer(view, null);
        }
        async Task ClosePopupElementAsync(VisualElement visualElement, IVisualElementRenderer renderer, UIViewController viewController) {
            await Task.Delay(50);
            visualElement.DescendantRemoved -= HandleChildRemoved;

            if (renderer != null && viewController != null && !viewController.IsBeingDismissed) {
                var window = viewController.View.Window;
                window.RootViewController.DismissModalViewController(false);
                viewController.Dispose();
                visualElement.Parent = null;
            }
        }

        public async override Task ShowAlertDialogAsync(DXDialogContentView dialogContentView) {
            var renderer = GetOrCreateRenderer(dialogContentView);
            UIViewController alertController = new  DialogViewController(new RendererContainer(renderer));
            dialogContentView.DialogClosed += async (o, args) =>  await ClosePopupElementAsync(dialogContentView, renderer, alertController);
            await ShowPopupElementAsync(dialogContentView, alertController);
        }
    }
}
