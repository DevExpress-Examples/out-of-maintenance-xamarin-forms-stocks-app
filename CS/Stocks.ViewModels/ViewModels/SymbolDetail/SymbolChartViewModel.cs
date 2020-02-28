using System;
using System.Windows.Input;
using System.ComponentModel;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class SymbolChartViewModel : BaseViewModel, IInitializableViewModel, IDisposable {
        bool isDisposed = false;
        public SymbolHistoricalPricesViewModel historicalValues;
        public SymbolHistoricalPricesHeaderViewModel header;

        public override string Title => "Chart";

        public SymbolHistoricalPricesViewModel HistoricalValues => historicalValues;
        public SymbolHistoricalPricesHeaderViewModel Header => header;

        public ICommand ReloadContentCommand { get; }        

        public InitializableViewModelState State => HistoricalValues.State;
        public Exception Error => HistoricalValues.Error;

        public SymbolChartViewModel(string ticker, IViewModelFactory viewModelFactory) {
            historicalValues = viewModelFactory.CreateSymbolHistoricalPricesViewModel(ticker);
            header = viewModelFactory.CreateSymbolHistoricalPricesHeaderViewModel(ticker);
            historicalValues.PropertyChanged += OnHistoricalValuesPropertyChanged;

            ReloadContentCommand = new DelegateCommand(ExecuteReloadContentCommand);
        }

        private void OnHistoricalValuesPropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(SymbolHistoricalPricesViewModel.State): RaisePropertyChanged(nameof(State)); break;
                case nameof(SymbolHistoricalPricesViewModel.Error): RaisePropertyChanged(nameof(Error)); break;
            }
        }

        protected override void OnPresent() {
            base.OnPresent();
            header.Present();
            historicalValues.Present();
        }

        protected override void  OnHide() {
            base.OnHide();
            header.Hide();
            historicalValues.Hide();
        }

        public void Dispose() {
            Dispose(true);
        }

        protected void Dispose(bool disposing) {
            if (isDisposed) return;
            header?.Dispose();
            header = null;
            historicalValues?.Dispose();
            historicalValues = null;
            isDisposed = true;
        }

        void ExecuteReloadContentCommand() {
            header?.ReloadContentCommand?.Execute(null);
            historicalValues?.ReloadContentCommand?.Execute(null); 
        }
    }
}
