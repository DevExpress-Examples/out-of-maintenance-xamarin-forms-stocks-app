using System;
namespace Stocks.ViewModels {
    public interface IInitializableViewModel: INavigableViewModel {
        InitializableViewModelState State { get; }

        Exception Error { get; }
    }
}
