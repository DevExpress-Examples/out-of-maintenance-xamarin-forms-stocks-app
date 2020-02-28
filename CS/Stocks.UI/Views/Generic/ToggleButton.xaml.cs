using System;
using DevExpress.XamarinForms.Editors;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class ToggleButton : ContentButton {
        public static readonly BindableProperty CheckedIconSourceProperty = BindableProperty.Create(nameof(CheckedIconSource), typeof(string), typeof(ToggleButton), propertyChanged: OnAnyIconSourceChanged);
        public static readonly BindableProperty UncheckedIconSourceProperty = BindableProperty.Create(nameof(UncheckedIconSource), typeof(string), typeof(ToggleButton), propertyChanged: OnAnyIconSourceChanged);

        public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(ToggleButton), Color.Default, propertyChanged: OnAnyForegroundColorChanged);
        public static readonly BindableProperty PressedForegroundColorProperty = BindableProperty.Create(nameof(PressedForegroundColor), typeof(Color), typeof(ToggleButton), Color.Default, propertyChanged: OnAnyForegroundColorChanged);

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ToggleButton), default(bool), BindingMode.TwoWay, propertyChanged: OnIsCheckedChanged);


        public string CheckedIconSource {
            get => (string)GetValue(CheckedIconSourceProperty);
            set => SetValue(CheckedIconSourceProperty, value);
        }
        public string UncheckedIconSource {
            get => (string)GetValue(UncheckedIconSourceProperty);
            set => SetValue(UncheckedIconSourceProperty, value);
        }

        public Color ForegroundColor {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        public Color PressedForegroundColor {
            get => IsSet(PressedForegroundColorProperty)
                ? (Color)GetValue(PressedForegroundColorProperty)
                : ForegroundColor;
            set => SetValue(PressedForegroundColorProperty, value);
        }

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public ToggleButton() {
            InitializeComponent();
            UpdateIcon();
            Pressed += OnPressed;
            Released += OnReleased;
            Clicked += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e) {
            IsChecked = !IsChecked;
        }

        private void OnReleased(object sender, EventArgs e) {
            icon.ForegroundColor = ForegroundColor;
        }

        private void OnPressed(object sender, EventArgs e) {
            icon.ForegroundColor = PressedForegroundColor;
        }

        void UpdateIcon() {
            icon.ImageSource = IsChecked ? CheckedIconSource : UncheckedIconSource;
            UpdateForegroundColor();
        }

        void UpdateForegroundColor() {
            icon.ForegroundColor = IsPressed ? PressedForegroundColor : ForegroundColor;
        }

        static void OnIsCheckedChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is ToggleButton toggle)) return;
            toggle.UpdateIcon();
        }

        static void OnAnyIconSourceChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is ToggleButton toggle)) return;
            toggle.UpdateIcon();
        }

        static void OnAnyForegroundColorChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is ToggleButton toggle)) return;
            toggle.UpdateForegroundColor();
        }
    }
}
