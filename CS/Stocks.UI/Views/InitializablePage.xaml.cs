using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Stocks.UI.Converters;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class InitializablePage : ContentPage {
        const string StatePropertyName = "State";
        static readonly InitializableViewModelStateToContentVisibilityConverter StateToContentViewVisibilityConverter = new InitializableViewModelStateToContentVisibilityConverter();

        public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(InitializablePage), propertyChanged: OnPlaceholderTextPropertyChanged);
        public static readonly BindableProperty PlaceholderIconSourceProperty = BindableProperty.Create(nameof(PlaceholderIconSource), typeof(ImageSource), typeof(InitializablePage), propertyChanged: OnPlaceholderIconSourcePropertyChanged);
        public static readonly BindableProperty PlaceholderActionButtonCaptionProperty = BindableProperty.Create(nameof(PlaceholderActionButtonCaption), typeof(string), typeof(InitializablePage), propertyChanged: OnPlaceholderActionButtonCaptionPropertyChanged);
        public static readonly BindableProperty PlaceholderActionButtonCommandProperty = BindableProperty.Create(nameof(PlaceholderActionButtonCommand), typeof(ICommand), typeof(InitializablePage), propertyChanged: OnPlaceholderActionButtonCommandPropertyChanged);
        public static readonly BindableProperty RetryButtonCaptionProperty = BindableProperty.Create(nameof(RetryButtonCaption), typeof(string), typeof(InitializablePage), propertyChanged: OnRetryButtonCaptionPropertyChanged);
        public static readonly BindableProperty RetryButtonCommandProperty = BindableProperty.Create(nameof(RetryButtonCommand), typeof(ICommand), typeof(InitializablePage), propertyChanged: OnRetryButtonCommandPropertyChanged);

        View contentView;

        public string PlaceholderText { get => (string)GetValue(PlaceholderTextProperty); set => SetValue(PlaceholderTextProperty, value); }
        public ImageSource PlaceholderIconSource { get => (ImageSource)GetValue(PlaceholderIconSourceProperty); set => SetValue(PlaceholderIconSourceProperty, value); }
        public string PlaceholderActionButtonCaption { get => (string)GetValue(PlaceholderActionButtonCaptionProperty); set => SetValue(PlaceholderActionButtonCaptionProperty, value); }
        public ICommand PlaceholderActionButtonCommand { get => (ICommand)GetValue(PlaceholderActionButtonCommandProperty); set => SetValue(PlaceholderActionButtonCommandProperty, value); }
        public string RetryButtonCaption { get => (string)GetValue(RetryButtonCaptionProperty); set => SetValue(RetryButtonCaptionProperty, value); }
        public ICommand RetryButtonCommand { get => (ICommand)GetValue(RetryButtonCommandProperty); set => SetValue(RetryButtonCommandProperty, value); }

        public virtual bool ShouldHandleDeviceLock { get; } = true;

        public View ContentView {
            get => contentView;
            set {
                if (contentView == value) return;
                View oldView = contentView;
                contentView = value;
                OnContentViewChanged(oldView);
            }
        }

        public InitializablePage() {
            InitializeComponent();
            OnPlaceholderTextChanged();
            OnPlaceholderIconSourceChanged();
            OnPlaceholderActionButtonCaptionChanged();
            OnPlaceholderActionButtonCommandChanged();
        }

        void OnPlaceholderTextChanged() {
            string text = PlaceholderText;
            placeholderLabel.IsVisible = !string.IsNullOrEmpty(text);
            placeholderLabel.Text = text;
        }
        void OnPlaceholderIconSourceChanged() {
            placeholderIcon.Source = PlaceholderIconSource;
        }
        void OnPlaceholderActionButtonCaptionChanged() {
            placeholderActionButton.Text = PlaceholderActionButtonCaption;
        }
        void OnPlaceholderActionButtonCommandChanged() {
            ICommand command = PlaceholderActionButtonCommand;
            placeholderActionButton.IsVisible = command != null;
            placeholderActionButton.Command = command;
        }
        void OnRetryButtonCaptionChanged() {
            retryButton.Text = RetryButtonCaption;
        }
        void OnRetryButtonCommandChanged() {
            retryButton.Command = RetryButtonCommand;
        }
        void OnContentViewChanged(View oldContentView) {
            var topView = container.Children.Last();
            if (topView == oldContentView) {
                topView.RemoveBinding(IsVisibleProperty);
                container.Children.Remove(topView);
                
            }
            ContentView.SetBinding(IsVisibleProperty, new Binding(StatePropertyName, BindingMode.OneWay, StateToContentViewVisibilityConverter));
            container.Children.Add(ContentView);
        }

        static void OnPlaceholderTextPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnPlaceholderTextChanged();
        }
        static void OnPlaceholderIconSourcePropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnPlaceholderIconSourceChanged();
        }
        static void OnPlaceholderActionButtonCaptionPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnPlaceholderActionButtonCaptionChanged();
        }
        static void OnPlaceholderActionButtonCommandPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnPlaceholderActionButtonCommandChanged();
        }
        static void OnRetryButtonCaptionPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnRetryButtonCaptionChanged();
        }
        static void OnRetryButtonCommandPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is InitializablePage page)) return;
            page.OnRetryButtonCommandChanged();
        }
    }
}
