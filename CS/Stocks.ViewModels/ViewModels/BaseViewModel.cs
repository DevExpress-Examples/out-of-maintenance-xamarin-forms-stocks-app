using System;

namespace Stocks.ViewModels {
    public abstract class BaseViewModel : NotificationObject, INavigableViewModel {
        readonly object locker = new object();
        bool isPresented;

        public virtual string Title { get; } = String.Empty;

        public bool IsPresented {
            get => isPresented;
            private set => SetProperty(ref isPresented, value);
        }

        public void Present() {
            lock(locker) {
                if (IsPresented) return;
                IsPresented = true;
                OnPresent();
            }
        }

        public void Hide() {
            lock (locker) {
                if (!IsPresented) return;
                IsPresented = false;
                OnHide();
            }
        }

        protected virtual void OnPresent() {  }
        protected virtual void OnHide() {  }
    }
}
