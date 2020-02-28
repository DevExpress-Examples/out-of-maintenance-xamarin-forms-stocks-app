using System;
using System.Windows.Input;

namespace Stocks.ViewModels {
    public class DelegateCommand<T> : ICommand {
        readonly Action<T> executeHandler;
        readonly Func<T, bool> canExecuteHandler;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null) {
            this.executeHandler = execute;
            this.canExecuteHandler = canExecute;
        }

        public bool CanExecute(object parameter) {
            if (!(parameter is T value)) return false;
            return canExecuteHandler?.Invoke(value) ?? true;
        }

        public void Execute(object parameter) {
            if (!(parameter is T value)) return;
            executeHandler.Invoke(value);
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand : ICommand {
        readonly Action<object> executeHandler;
        readonly Func<object, bool> canExecuteHandler;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null) {
            this.executeHandler = execute;
            this.canExecuteHandler = canExecute;
        }

        public DelegateCommand(Action execute, Func<bool> canExecute = null) {
            this.executeHandler = v => execute();
            if (canExecute != null) {
                this.canExecuteHandler = v => canExecute();
            }
        }

        public void Execute(object parameter) => executeHandler.Invoke(parameter);
        public bool CanExecute(object parameter) => canExecuteHandler?.Invoke(parameter) ?? true;

        public void RaiseCanExecuteChanged() {
            EventHandler handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
