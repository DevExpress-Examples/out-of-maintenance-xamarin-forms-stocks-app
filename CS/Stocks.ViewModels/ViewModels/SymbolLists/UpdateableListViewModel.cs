using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Stocks.ViewModels {
    public abstract class UpdateableListViewModel<TItem> : UpdatableViewModel<IList<TItem>> {
        protected void ClearItems() => SetHasContentState(null);

        protected override void SetHasContentState(IList<TItem> c) {
            if (c == null) {
                base.SetHasContentState(null);
            } else {
                if (Content == null || Content.Count != c.Count) {
                    base.SetHasContentState(new BindingList<TItem>(c));
                } else {
                    for (int i = 0; i < Content.Count; i++) {
                        Content[i] = c[i];
                    }
                    base.SetHasContentState(Content);
                }
            }
        }

        protected override bool HasContent() => base.HasContent() && Content.Any();
    }
}

