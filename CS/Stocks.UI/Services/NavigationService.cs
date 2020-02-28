using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Services;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Services {
    public interface INavigationHost {
        Task NavigateTo(Page page);
        Task ReplaceWith(Page page);
        Task<Page> NavigateBack();

        Page CurrentPage { get; }
    }

    public class HostHolder {
        public INavigationHost Item { get; }
        public HostHolder Inner { get; set; }

        public HostHolder(INavigationHost item) {
            Item = item;
        }
    }

    public class NavigationService : INavigationService {
        static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        PageLocator pageLocator = PageLocator.Instance;
        Stack<HostHolder> holderStack = new Stack<HostHolder>();
        Dictionary<Type, HostHolder> cache = new Dictionary<Type, HostHolder>();

        HostHolder ActualHolder => holderStack.Peek();
        INavigationHost ActualContainer => ActualHolder.Item;

        public Page GetMainPage(INavigableViewModel mainViewModel) {
            Page mainPage = pageLocator.GetPage(mainViewModel);
            holderStack.Clear();
            cache.Clear();
            INavigationHost container = mainPage as INavigationHost ?? throw new Exception("The root page attached to the rootViewModel must implement the 'INavigationContainer' interface.");
            HostHolder actualHolder = new HostHolder(container);
            holderStack.Push(actualHolder);
            cache.Add(mainViewModel.GetType(), actualHolder);

            mainViewModel.Present();
            return mainPage;
        }

        public async Task NavigateTo(INavigableViewModel viewModel) {
            await semaphore.WaitAsync();
            try {
                HostHolder hostHolder;
                if (!cache.TryGetValue(viewModel.GetType(), out hostHolder)) {
                    Page page = null;
                    await Device.InvokeOnMainThreadAsync(() => {
                        page = pageLocator.GetPage(viewModel);
                    });
                    OnHidePage(ActualContainer.CurrentPage);
                    viewModel.Present();
                    await ActualContainer.NavigateTo(page);
                    if (page is INavigationHost host) {
                        hostHolder = new HostHolder(host);
                        cache.Add(viewModel.GetType(), hostHolder);
                    } else {
                        return;
                    }
                } else {
                    Page page = hostHolder.Item as Page ?? throw new Exception("All holders the system stores must be pages.");
                    OnHidePage(ActualContainer.CurrentPage);
                    viewModel.Present();
                    await ActualContainer.NavigateTo(page);
                }
                ActualHolder.Inner = hostHolder;
                holderStack.Push(hostHolder);
            } finally {
                semaphore.Release();
            }
        }

        public async Task Replace(INavigableViewModel viewModel, INavigableViewModel newViewModel) {
            await semaphore.WaitAsync();
            try {
                HostHolder hostHolder;
                if (viewModel != null && cache.TryGetValue(viewModel.GetType(), out hostHolder)) {
                    HostHolder lastHolder = holderStack.Pop();
                    while (!Equals(lastHolder, hostHolder)) {
                        OnHidePage(lastHolder.Item.CurrentPage);
                        lastHolder = holderStack.Pop();
                    }
                }
                HostHolder newHostHolder;
                Page page = null;
                if (!cache.TryGetValue(newViewModel.GetType(), out newHostHolder)) {
                    await Device.InvokeOnMainThreadAsync(() => {
                        page = pageLocator.GetPage(newViewModel);
                    });
                    if (!(page is INavigationHost host)) {
                        OnHidePage(ActualContainer.CurrentPage);
                        await OnReplacePage(page, newViewModel);
                        return;
                    }
                    newHostHolder = new HostHolder(host);
                    cache.Add(newViewModel.GetType(), newHostHolder);
                } else {
                    page = newHostHolder.Item as Page ?? throw new Exception("All holders the system stores must be pages.");
                }
                OnHidePage(ActualContainer.CurrentPage);
                await OnReplacePage(page, newViewModel);
                ActualHolder.Inner = newHostHolder;
                holderStack.Push(newHostHolder);
            } finally {
                semaphore.Release();
            }
        }

        public async Task NavigateBack() {
            Page page = await ActualContainer.NavigateBack();
            if (page == null) {
                holderStack.Pop();
                await ActualContainer.NavigateBack();
            }
        }

        async Task OnReplacePage(Page page, INavigableViewModel viewModel) {
            viewModel.Present();
            await ActualHolder.Item.ReplaceWith(page);
        }

        void OnHidePage(Page page) {
            INavigableViewModel viewModel = page.BindingContext as INavigableViewModel;
            viewModel?.Hide();
        }
    }
}