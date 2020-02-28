using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Stocks.UI.Themes;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class TitleView : RelativeLayout {
        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TitleView), string.Empty, propertyChanged: OnTitleTextChanged);
        public static readonly BindableProperty MenuIconSourceProperty = BindableProperty.Create(nameof(MenuIconSource), typeof(string), typeof(TitleView), string.Empty, propertyChanged: OnMenuIconSourcePropertyChanged);
        public static readonly BindableProperty ShowMenuItemProperty = BindableProperty.Create(nameof(ShowMenuItem), typeof(bool), typeof(TitleView), default(bool), propertyChanged: OnShowMenuItemPropertyChanged);

        BarIconItem menuItem;
        Label titleTextView;
        StackLayout startItemContainer;
        StackLayout endItemContainer;


        public string TitleText { get => (string)GetValue(TitleTextProperty); set => SetValue(TitleTextProperty, value); }
        public string MenuIconSource { get => (string)GetValue(MenuIconSourceProperty); set => SetValue(MenuIconSourceProperty, value); }
        public BarItemCollection StartItems { get; } = new BarItemCollection();
        public BarItemCollection EndItems { get; } = new BarItemCollection();
        public bool ShowMenuItem { get => (bool)GetValue(ShowMenuItemProperty); set => SetValue(ShowMenuItemProperty, value); }

        public TitleView() {
            Resources.MergedDictionaries.Add(new TitleViewStyles());

            InitializeChildren();
            LayoutChildren();

            CompressedLayout.SetIsHeadless(this, true);
            Style = (Style)Resources["TitleViewContainer"];

            StartItems.CollectionChanged += OnStartItemsChanged;
            EndItems.CollectionChanged += OnEndItemsChanged;
        }

        void OnEndItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
            HandleCollectionChanged(endItemContainer, e, 0);
        }

        void OnStartItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
            HandleCollectionChanged(startItemContainer, e, ShowMenuItem ? 1 : 0);
        }

        void InitializeChildren() {
            titleTextView = new Label {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = (Device.RuntimePlatform == Device.iOS) ? TextAlignment.Center : TextAlignment.Start,
                Style = (Style)Resources["NavigationPageTitleLabelStyle"]
            };
            startItemContainer = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                FlowDirection = Device.FlowDirection,
                Style = (Style)Resources["StartBarItemContainer"]
            };

            endItemContainer = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Style = (Style)Resources["EndBarItemContainer"],
                FlowDirection = (Device.FlowDirection == FlowDirection.LeftToRight)
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight
            };

            menuItem = new BarIconItem {
                Style = (Style)Resources["BarItemStyle"]
            };
            menuItem.Clicked += OnMenuItemClicked;
        }
        void LayoutChildren() {
            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    Children.Add(
                        titleTextView,
                        xConstraint: Constraint.Constant(0),
                        yConstraint: Constraint.Constant(0),
                        widthConstraint: Constraint.RelativeToParent((parent) => parent.Width),
                        heightConstraint: Constraint.RelativeToParent((parent) => parent.Height));
                    break;
                case Device.Android:
                    Children.Add(
                        titleTextView,
                        xConstraint: Constraint.RelativeToView(startItemContainer, (parent, view) => startItemContainer.Width + 20),
                        yConstraint: Constraint.Constant(0),
                        heightConstraint: Constraint.RelativeToParent((parent) => parent.Height));
                    break;
            }
            Children.Add(
                startItemContainer,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                    parent.Height
                ));
            Children.Add(
                endItemContainer,
                xConstraint: Constraint.RelativeToParent((parent) =>
                    parent.Width - endItemContainer.Width
                ),
                yConstraint: Constraint.Constant(0),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                    parent.Height
                ));
        }

        void HandleCollectionChanged(StackLayout itemContainer, NotifyCollectionChangedEventArgs e, int skipCount) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    HandleAddCollectionChange(itemContainer, e.NewStartingIndex + skipCount, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    HandleRemoveCollectionChange(itemContainer, e.OldStartingIndex + skipCount, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    HandleMoveCollectionChange(itemContainer, e.OldStartingIndex + skipCount, e.NewStartingIndex + skipCount, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    HandleReplaceCollectionChange(itemContainer, e.NewStartingIndex + skipCount, e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    HandleResetCollectionChange(itemContainer, skipCount);
                    break;
            }
        }

        void HandleAddCollectionChange(StackLayout itemContainer, int startingIndex, IList items) {
            var insertIndex = startingIndex;
            foreach (object item in items) {
                BarIconItem barItem = (BarIconItem)item;
                barItem.Style = (Style)Resources["BarItemStyle"];
                itemContainer.Children.Insert(insertIndex, barItem);
                insertIndex++;
            }
        }
        void HandleRemoveCollectionChange(StackLayout itemContainer, int startingIndex, IList items) {
            for (int i = 0; i < items.Count; ++i) {
                itemContainer.Children.RemoveAt(startingIndex);
            }
        }
        void HandleMoveCollectionChange(StackLayout itemContainer, int oldStartingIndex, int newStartingIndex, IList items) {
            if (newStartingIndex < oldStartingIndex) {
                for (int i = 0; i < items.Count; ++i) {
                    View item = itemContainer.Children[oldStartingIndex];
                    itemContainer.Children.RemoveAt(oldStartingIndex);
                    itemContainer.Children.Insert(newStartingIndex + i, item);
                }
            } else {
                for (int i = 0; i < items.Count; ++i) {
                    View item = itemContainer.Children[oldStartingIndex];
                    itemContainer.Children.RemoveAt(oldStartingIndex);
                    itemContainer.Children.Insert(newStartingIndex - 1, item);
                }
            }
        }
        void HandleResetCollectionChange(StackLayout itemContainer, int skipCount) {
            for (int i = skipCount; i < itemContainer.Children.Count; ++i) {
                itemContainer.Children.RemoveAt(skipCount);
            }
        }
        void HandleReplaceCollectionChange(StackLayout itemContainer, int startingIndex, IList oldItems, IList newItems) {
            for (int i = 0; i < oldItems.Count; ++i) {
                itemContainer.Children.RemoveAt(startingIndex);
            }
            for (int i = 0; i < newItems.Count; ++i) {
                var barItem = (BarIconItem)newItems[i];
                barItem.Style = (Style)Resources["BarItemStyle"];
                itemContainer.Children.Insert(startingIndex + i, barItem);
            }
        }

        void OnMenuItemClicked(object sender, EventArgs args) {
            MessagingCenter.Instance.Send<object>(this, Constants.ToggleDrawerMessage);
        }

        void UpdateTitleText() {
            titleTextView.Text = TitleText;
        }
        void UpdateShowMenuItem() {
            if (ShowMenuItem) {
                startItemContainer.Children.Insert(0, menuItem);
            } else {
                startItemContainer.Children.RemoveAt(0);
            }

        }
        void UpdateMenuItemIconSource() {
            menuItem.IconSource = MenuIconSource;
        }

        static void OnTitleTextChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is TitleView titleView)) return;
            titleView.UpdateTitleText();
        }
        static void OnShowMenuItemPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is TitleView titleView)) return;
            titleView.UpdateShowMenuItem();
        }
        static void OnMenuIconSourcePropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is TitleView titleView)) return;
            titleView.UpdateMenuItemIconSource();
        }
    }

    public class BarItemCollection : ObservableCollection<BarIconItem> { }
}