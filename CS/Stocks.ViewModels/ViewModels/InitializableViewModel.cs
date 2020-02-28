using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models.Utils;
using Xamarin.Forms;

namespace Stocks.ViewModels {
    public abstract class InitializableViewModel<TContent> : BaseViewModel, IInitializableViewModel, IErrorHandler {
        TContent content;
        InitializableViewModelState state;
        Exception error;
        bool isRefreshing;
        CancellationTokenSource cancellationTokenSource;

        public override string Title { get; } = String.Empty;

        public TContent Content {
            get => content;
            private set => SetProperty(ref content, value, onChanged: OnContentChanged);
        }
        public Exception Error {
            get => error;
            private set => SetProperty(ref error, value);
        }
        public InitializableViewModelState State {
            get => state;
            private set => SetProperty(ref state, value, onChanged: OnStateChanged);
        }

        // For list and grid.
        public bool IsRefreshing {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public ICommand ReloadContentCommand { get; }

        public InitializableViewModel() : this(InitializableViewModelState.Loading) {
        }

        public InitializableViewModel(InitializableViewModelState initState) {
            State = initState;
            ReloadContentCommand = new DelegateCommand(LoadContent);
        }

        protected void LoadContent() {
            if (!IsPresented) return;
            LoadContentAsync().InvokeAndForgetSafeAsync(this);
        }

        protected void CancelLoadContent() {
            if (cancellationTokenSource == null) return;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        async Task LoadContentAsync() {
            CancelLoadContent();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            var oldState = State;
            cancellationToken.Register(() => {
                Device.BeginInvokeOnMainThread(() => {
                    State = oldState;
                });
            });
            await Device.InvokeOnMainThreadAsync(SetLoadingState);
            var result = await SendRequestAsync(cancellationToken);
            await Device.InvokeOnMainThreadAsync(() => {
                SetHasContentState(result);
            });
            if (cancellationTokenSource != null) {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        void IErrorHandler.HandleError(Exception exception) {
            if (exception is OperationCanceledException) return;
            Console.WriteLine($"STOCKS_ERROR: {exception}");
            SetHasErrorState(exception);
        }

        protected void SetLoadingState() {
            State = HasContent()
                ? InitializableViewModelState.Reloading
                : InitializableViewModelState.Loading;
        }
        protected virtual void SetHasContentState(TContent c) {
            Content = c;
            State = HasContent()
                ? InitializableViewModelState.HasContent
                : InitializableViewModelState.Initialized;
        }
        protected void SetHasErrorState(Exception e) {
            Error = e;
            State = InitializableViewModelState.HasError;
        }

        protected virtual void OnContentChanged(TContent oldValue, TContent newValue) {  }
        protected virtual bool HasContent() => !EqualityComparer<TContent>.Default.Equals(Content, default);
        protected virtual void OnStateChanged(InitializableViewModelState oldValue, InitializableViewModelState newValue) {
            if (newValue != InitializableViewModelState.Loading && newValue != InitializableViewModelState.Reloading) {
                IsRefreshing = false;
            }
        }

        protected abstract Task<TContent> SendRequestAsync(CancellationToken token);

        protected override void OnPresent() => LoadContent();
        protected override void OnHide() => CancelLoadContent();
    }

    public enum InitializableViewModelState {
        Initialized,
        Loading,
        HasContent,
        Reloading,
        HasError
    };
}
