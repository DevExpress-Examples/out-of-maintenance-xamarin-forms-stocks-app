using Xamarin.Forms;
using DevExpress.XamarinForms.DemoEditors;

namespace Stocks.UI.Views {
    public partial class RadioButton : ContentButton {
        public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(RadioButton), Color.Default, propertyChanged: OnAnyValueChanged);
        public static readonly BindableProperty PressedForegroundColorProperty = BindableProperty.Create(nameof(PressedForegroundColor), typeof(Color), typeof(RadioButton), Color.Default, propertyChanged: OnAnyValueChanged);
        public static readonly BindableProperty CheckedForegroundColorProperty = BindableProperty.Create(nameof(CheckedForegroundColor), typeof(Color), typeof(RadioButton), Color.Default, propertyChanged: OnAnyValueChanged);
        public static readonly BindableProperty CheckedBackgroundColorProperty = BindableProperty.Create(nameof(CheckedBackgroundColor), typeof(Color), typeof(RadioButton), Color.Default, propertyChanged: OnAnyValueChanged);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(RadioButton), string.Empty, propertyChanged: OnTextPropertyChanged);
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(RadioButton), default(bool), propertyChanged: OnIsCheckedPropertyChanged);

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public Color ForegroundColor {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        public Color PressedForegroundColor {
            get => IsSet(PressedForegroundColorProperty)
                ? (Color)GetValue(PressedForegroundColorProperty)
                : IsChecked
                    ? CheckedForegroundColor
                    : ForegroundColor;
            set => SetValue(PressedForegroundColorProperty, value);
        }
        public Color CheckedForegroundColor {
            get => IsSet(CheckedForegroundColorProperty)
                ? (Color)GetValue(CheckedForegroundColorProperty)
                : ForegroundColor;
            set => SetValue(CheckedForegroundColorProperty, value);
        }
        public Color CheckedBackgroundColor {
            get => IsSet(CheckedBackgroundColorProperty)
                ? (Color)GetValue(CheckedBackgroundColorProperty)
                : BackgroundColor;
            set => SetValue(CheckedBackgroundColorProperty, value);
        }

        public RadioButton() {
            InitializeComponent();
            OnTextChanged();
            OnIsCheckedChanged();
            Pressed += OnPressed;
            Released += OnReleased;
        }

        protected override void ChangeVisualState() {
            if (IsEnabled && IsChecked && !IsPressed) {
                VisualStateManager.GoToState(this, "Checked");
            } else {
                base.ChangeVisualState();
            }
        }

        void OnPressed(object sender, System.EventArgs e) {
            ChangeActualColors();
        }

        void OnReleased(object sender, System.EventArgs e) {
            ChangeActualColors();
        }

        void ChangeActualColors() {
            if (IsPressed) {
                label.TextColor = PressedForegroundColor;
                indicator.Color = PressedBackgroundColor;
            } else if (IsChecked) {
                label.TextColor = CheckedForegroundColor;
                indicator.Color = CheckedBackgroundColor;
            } else {
                label.TextColor = ForegroundColor;
                indicator.Color = BackgroundColor;
            }
        }

        void OnIsCheckedChanged() {
            indicator.IsVisible = IsChecked;
            ChangeVisualState();
            ChangeActualColors();
        }
        void OnTextChanged() {
            label.Text = Text;
        }

        static void OnAnyValueChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is RadioButton btn)) return;
            btn.ChangeActualColors();
        }

        static void OnIsCheckedPropertyChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is RadioButton btn)) return;
            btn.OnIsCheckedChanged();
        }

        static void OnTextPropertyChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is RadioButton btn)) return;
            btn.OnTextChanged();
        }
    }
}
