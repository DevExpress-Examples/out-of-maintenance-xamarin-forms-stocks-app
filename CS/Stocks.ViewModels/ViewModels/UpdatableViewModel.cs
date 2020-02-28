using System;
using System.Threading;

namespace Stocks.ViewModels {
    public abstract class UpdatableViewModel<TContent> : InitializableViewModel<TContent>, IDisposable {
        private readonly object locker = new object();
        bool isDisposed;
        Timer timer;
        
        protected virtual int UpdateDelay { get; } = 0;
        protected virtual int UpdatePeriod { get; } = 15_000;

        public void Dispose() {
            Dispose(true);
        }

        protected void RequestUpdate() {
            StartUpdates();
        }

        protected virtual void Dispose(bool disposing) {
            if (isDisposed) return;
            StopUpdates();
            isDisposed = true;
        }

        public void StartUpdates() {
            StopUpdates();
            timer = new Timer(TimerCallback, null, UpdateDelay, UpdatePeriod);
        }

        public void StopUpdates() {
            CancelLoadContent();
            if (timer == null) return;
            timer.Dispose();
            timer = null;
        }

        void TimerCallback(object state) => LoadContent();

        protected override void OnPresent() => StartUpdates();
        protected override void OnHide() => StopUpdates();
    }
}
