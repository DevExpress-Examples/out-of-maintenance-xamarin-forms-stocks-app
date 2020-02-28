using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Stocks.ViewModels.Utils {
    public interface IAsyncCommand: ICommand {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public interface IAsyncCommand<T> : ICommand {
        Task ExecuteAsync(T value);
        bool CanExecute(T value);
    }
}

