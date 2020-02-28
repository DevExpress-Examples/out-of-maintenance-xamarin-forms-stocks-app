using System;
using System.Threading.Tasks;

namespace Stocks.Services {
    public interface IDialogService {
        Task ShowAlert();
        Task ShowActionSheet();

        Task ShowDialog(object viewModel);
        Task ShowFullScreenDialog(object viewModel);
    }

    public interface IDialogViewModel {
        event EventHandler Closed;
    }
}
