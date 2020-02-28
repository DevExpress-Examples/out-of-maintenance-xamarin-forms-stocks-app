using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models.Utils;

namespace Stocks.ViewModels.Utils {
    public class AsyncDelegateCommand : IAsyncCommand {
        bool isExecuting;
        readonly Func<Task> executeHandler;
        readonly Func<bool> canExecuteHandler;
        readonly IErrorHandler errorHandler;

        public event EventHandler CanExecuteChanged;

        public AsyncDelegateCommand(Func<Task> execute, IErrorHandler errorHandler) {
            this.executeHandler = execute;
            this.canExecuteHandler = null;
            this.errorHandler = errorHandler;
        }

        public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute = null, IErrorHandler errorHandler = null) {
            this.executeHandler = execute;
            this.canExecuteHandler = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute() {
            return !isExecuting && (canExecuteHandler?.Invoke() ?? true);
        }

        public async Task ExecuteAsync() {
            if (CanExecute()) {
                try {
                    RaiseCanExecuteChanged();
                    isExecuting = true;
                    await executeHandler();
                }
                finally {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

#region Explicit implementations
        bool ICommand.CanExecute(object parameter) {
            return CanExecute();
        }

        void ICommand.Execute(object parameter) {
            ExecuteAsync().InvokeAndForgetSafeAsync(errorHandler);
        }
#endregion
    }


    public class AsyncDelegateCommand<T> : IAsyncCommand<T> {
        bool isExecuting;
        readonly Func<T, Task> executeHandler;
        readonly Func<T, bool> canExecuteHandler;
        readonly IErrorHandler errorHandler;

        public event EventHandler CanExecuteChanged;

        public AsyncDelegateCommand(Func<T, Task> execute, IErrorHandler errorHandler) {
            this.executeHandler = execute;
            this.canExecuteHandler = null;
            this.errorHandler = errorHandler;
        }

        public AsyncDelegateCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null) {
            executeHandler = execute;
            canExecuteHandler = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter) {
            return !isExecuting && (canExecuteHandler?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter) {
            if (CanExecute(parameter)) {
                try {
                    isExecuting = true;
                    RaiseCanExecuteChanged();
                    await executeHandler(parameter);
                }
                finally {
                    isExecuting = false;
                }
            }
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

#region Explicit implementations
        bool ICommand.CanExecute(object parameter) {
            if (!(parameter is T value)) return false;
            return CanExecute(value);
        }

        void ICommand.Execute(object parameter) {
            if (!(parameter is T value)) return;
            ExecuteAsync(value).InvokeAndForgetSafeAsync(errorHandler);
        }
#endregion
    }
}
