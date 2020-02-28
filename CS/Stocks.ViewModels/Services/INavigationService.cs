using System.Threading.Tasks;
using Stocks.ViewModels;

namespace Stocks.Services {
    public interface INavigationService {
        Task NavigateTo(INavigableViewModel viewModel);
        Task Replace(INavigableViewModel viewModel, INavigableViewModel newViewModel);
        Task NavigateBack();
    }
}
