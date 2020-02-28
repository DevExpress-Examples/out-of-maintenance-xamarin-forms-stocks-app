using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public abstract class DetailedListViewModel<T> : UpdateableListViewModel<T> where T : ISymbol {
        public IDialogService DialogService { get; private set; }

        public ICommand SelectItemCommand { get; }
        public ICommand ShowSearchDialogCommand { get; }

        protected IViewModelFactory Factory { get; }
        protected INavigationService NavigationService { get; }
        protected bool IsInEditProcess { get; set; }

        public DetailedListViewModel(IViewModelFactory factory, INavigationService navigationService): base() {
            Factory = factory;
            NavigationService = navigationService;

            SelectItemCommand = new AsyncDelegateCommand<T>(ExecuteShowDetailCommand, this);
            ShowSearchDialogCommand = new AsyncDelegateCommand(ExecuteShowSymbolSearchDialogCommand, () => State != InitializableViewModelState.HasError, this);
        }

        public void Inject(IDialogService dialogService) {
            DialogService = dialogService;
        }

        protected abstract SymbolSearchViewModel CreateSymbolSearchViewModel();

        async Task ExecuteShowDetailCommand(T symbol) {
            await NavigateToDetail(symbol.Ticker);
        }

        async Task ExecuteShowSymbolSearchDialogCommand() {
            if (State == InitializableViewModelState.HasError) return;
            await ShowSymbolSearchDialog();
        }

        async Task NavigateToDetail(string ticker) {
            if (IsInEditProcess) return;
            IsInEditProcess = true;
            SymbolViewModel symbolViewModel = Factory.CreateSymbolDetailViewModel(ticker);
            await NavigationService.NavigateTo(symbolViewModel);
            IsInEditProcess = false;
        }

        async Task ShowSymbolSearchDialog() {
            SymbolSearchViewModel symbolSearchViewModel = CreateSymbolSearchViewModel();
            await NavigationService.NavigateTo(symbolSearchViewModel);
        }
    }

    public interface ISymbol {
        string Ticker { get; }
        string CompanyName { get; }
    }
}
