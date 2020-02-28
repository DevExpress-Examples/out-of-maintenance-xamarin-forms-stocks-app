using System;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Services;
using Stocks.UI.Views;
using Xamarin.Forms;

namespace Stocks.UI.Services {
    public class DialogService: IDialogService {
        WeakReference<Page> page;
        string tag;

        public DialogService(Page page, string tag = null) {
            this.page = new WeakReference<Page>(page);
            this.tag = tag;
        }

        public Task ShowActionSheet() {
            Page target;
            if (!page.TryGetTarget(out target))
                return Task.FromResult<object>(null);
            return Task.FromResult<object>(null);

        }

        public Task ShowAlert() {
            Page target;
            if (!page.TryGetTarget(out target))
                return Task.FromResult<object>(null);
            return Task.FromResult<object>(null);
        }

        public Task ShowDialog(object viewModel) {
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            Page target;
            if (!page.TryGetTarget(out target))
                return Task.FromResult<object>(null);


            PopupPage dialog = PageLocator.Instance.GetPage(viewModel, tag) as PopupPage;
            if (dialog == null) {
                return Task.FromResult<object>(null);
            }
            dialog.Disappearing += (sender, args) => waitHandle.Set();
            dialog.ShowAsync();

            return Task.Run(() => waitHandle.WaitOne());
        }

        public Task ShowFullScreenDialog(object viewModel) {
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            Page target;
            if (!page.TryGetTarget(out target))
                return Task.FromResult<object>(null);
            

            Page dialog = PageLocator.Instance.GetPage(viewModel, tag);
            if (dialog == null) {
                return Task.FromResult<object>(null);
            }
            dialog.Disappearing += (sender, args) => waitHandle.Set();
            target.Navigation.PushModalAsync(dialog);

            return Task.Run(() => waitHandle.WaitOne());
        }

    }
}
