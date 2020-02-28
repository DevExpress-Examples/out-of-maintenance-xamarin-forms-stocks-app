namespace Stocks.ViewModels {
    public interface ISearchViewModel : IInitializableViewModel {
        string Fragment { get; set; }
        string SearchResult { get; }

        void SearchImmediately();
    }
}
