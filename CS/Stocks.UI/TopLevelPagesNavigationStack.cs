using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Stocks.UI {
    public class TopLevelPagesNavigationStack {
        List<Page> pages = new List<Page>();

        public void Push(Page newPage) {
            int index = pages.IndexOf(newPage);
            if (index < 0) {
                pages.Add(newPage);
                return;
            }

            if (index == pages.Count - 1)
                return;

            pages.Remove(newPage);
            pages.Add(newPage);
        }

        public Page Pop() {
            Page lastPage = Peek();
            if (lastPage != null)
                pages.Remove(lastPage);
            return lastPage;
        }

        public Page Peek() {
            return pages.Any() ? pages.Last() : null;
        }
    }
}