using Xamarin.Forms;
using DevExpress.XamarinForms.Editors;

namespace Stocks.UI.Views {
    public partial class TextButton : ContentButton {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(TextButton), string.Empty, propertyChanged: OnTextChanged);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TextButton), Color.Default, propertyChanged: OnTextColorChanged);

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

        public TextButton() {
            InitializeComponent();
        }


        void UpdateText() {
            innerLabel.Text = Text;
        }
        void UpdateTextColor() {
            innerLabel.TextColor = TextColor;
        }

        static void OnTextChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (bindableObject is TextButton button) {
                button.UpdateText();
            }
        }
        static void OnTextColorChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (bindableObject is TextButton button) {
                button.UpdateTextColor();
            }
        }
    }
}
