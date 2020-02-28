using System.Windows.Input;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class EULAViewModel : NotificationObject {
        readonly ILocalStorage localStorage;
        bool shouldShow;

        public string Title { get; } = "License Agreement";

        public ICommand AcceptCommand { get; }
       
        public EULAViewModel(ILocalStorage localStorage) {
            this.localStorage = localStorage;
            this.shouldShow = localStorage.ShouldShowEULA;
            AcceptCommand = new DelegateCommand(() => { ShouldShow = false; });
        }

        public bool ShouldShow {
            get => shouldShow;
            private set => SetProperty(ref shouldShow, value, onChanged: (_, v) => localStorage.ShouldShowEULA = ShouldShow);
        }
    }
}

