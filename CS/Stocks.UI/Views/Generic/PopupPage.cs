using Xamarin.Forms;

namespace Stocks.UI.Views {
    public abstract class PopupPage : ContentPage {
        public static readonly BindableProperty SystemPaddingProperty = BindableProperty.Create(nameof(SystemPadding), typeof(Thickness), typeof(PopupPage), default(Thickness), BindingMode.OneWayToSource);

        public Thickness SystemPadding {
            get { return (Thickness)GetValue(SystemPaddingProperty); }
            private set { SetValue(SystemPaddingProperty, value); }
        }

        protected PopupPage() {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
        }

        protected override bool OnBackButtonPressed() {
            return false;
        }

        protected override void LayoutChildren(double x, double y, double width, double height) {
            var systemPadding = SystemPadding;
            var left = systemPadding.Left;
            var top = systemPadding.Top;
            x += left;
            y += top;
            width -= left + systemPadding.Right;
            height -= top + systemPadding.Bottom;
            base.LayoutChildren(x, y, width, height);
        }
    }
}

