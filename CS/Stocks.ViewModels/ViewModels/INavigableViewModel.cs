namespace Stocks.ViewModels {
    public interface INavigableViewModel {
        bool IsPresented { get; }
        string Title { get; }

        void Present();
        void Hide();
    }
}
