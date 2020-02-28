using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Stocks.UI.Android.Services;
using Stocks.UI.Views;
using XApplication = Xamarin.Forms.Application;
using AView = Android.Views.View;
using XFView = Xamarin.Forms.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using System.Threading;

[assembly: Dependency(typeof(StocksApplicationServiceImpl))]
namespace Stocks.UI.Android.Services {
    public class StocksApplicationServiceImpl : StocksApplicationService {
        Activity Activity => StocksApp.Context as Activity;
        FrameLayout DecoreView => (FrameLayout)Activity.Window.DecorView;

        public override async Task ShowPopupPageAsync(PopupPage popupPage) {
            var decoreView = DecoreView;
            popupPage.Parent = XApplication.Current.MainPage;
            var renderer = GetOrCreateRenderer(popupPage);
            decoreView.AddView(renderer.View);
            await PostAsync(renderer.View);
        }
        public override async Task ClosePopupPageAsync(PopupPage popupPage) {
            var renderer = GetOrCreateRenderer(popupPage);
            if (renderer != null) {
                var element = renderer.Element;

                DecoreView.RemoveView(renderer.View);
                renderer.Dispose();

                if (element != null)
                    element.Parent = null;

                await PostAsync(DecoreView);
            }

            await Task.FromResult(true);
        }

        Task PostAsync(AView nativeView) {
            if (nativeView == null)
                return Task.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();

            nativeView.Post(() => {
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        IVisualElementRenderer GetOrCreateRenderer(VisualElement element) {
            var renderer = Platform.GetRenderer(element);
            if (renderer == null) {
                renderer = Platform.CreateRendererWithContext(element, StocksApp.Context);
                Platform.SetRenderer(element, renderer);
            }
            return renderer;
        }

        public override Task ShowAlertDialogAsync(DXDialogContentView dialogContentView) {
            var builder = new AlertDialog.Builder(Activity);

            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            AlertDialog alertDialog = builder.Create();
            builder.Dispose();
            alertDialog.SetCanceledOnTouchOutside(true);
            IVisualElementRenderer renderer = GetOrCreateRenderer(dialogContentView);
            RendererContainer view = new RendererContainer(StocksApp.Context, renderer);
            alertDialog.SetView(view);
            dialogContentView.DialogClosed += (o, args) => { alertDialog.Cancel(); renderer.Dispose(); waitHandle.Set(); };
            alertDialog.Window.SetSoftInputMode(SoftInput.StateVisible);
            alertDialog.Show();
            return Task.Run(() => waitHandle.WaitOne());
        }
    }
}
