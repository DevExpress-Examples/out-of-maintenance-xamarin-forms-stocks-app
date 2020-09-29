using Xamarin.Forms;
using DevExpress.XamarinForms.DemoEditors;

namespace Stocks.UI.Views {
	public class BarIconItem : ContentButton {
		public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(BarIconItem), Color.Default, propertyChanged: OnForegroundColorPropertyChanged);
        public static readonly BindableProperty PressedForegroundColorProperty = BindableProperty.Create(nameof(PressedForegroundColor), typeof(Color), typeof(BarIconItem), Color.Default, propertyChanged: OnForegroundColorPropertyChanged);
        public static readonly BindableProperty IconSourceProeprty = BindableProperty.Create(nameof(IconSource), typeof(string), typeof(BarIconItem), string.Empty, propertyChanged: OnIconSourcePropertyChanged);

		IconView icon;

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
        public string IconSource {
			get => (string)GetValue(IconSourceProeprty);
			set => SetValue(IconSourceProeprty, value);
		}

		public BarIconItem() {
			icon = new IconView {
				WidthRequest = 24,
				HeightRequest = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			Content = icon;
            Pressed += OnPressed;
            Released += OnReleased;
		}

        private void OnReleased(object sender, System.EventArgs e) {
            icon.ForegroundColor = ForegroundColor;
        }

        private void OnPressed(object sender, System.EventArgs e) {
            icon.ForegroundColor = PressedForegroundColor;
        }

        void UpdateForegroundColor() {
            icon.ForegroundColor = IsPressed ? PressedForegroundColor : ForegroundColor;
		}

		void UpdateIconSource() {
			icon.ImageSource = IconSource;
		}

		static void OnForegroundColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is BarIconItem item)) return;
			item.UpdateForegroundColor();
		}

		static void OnIconSourcePropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
			if (!(bindableObject is BarIconItem item)) return;
			item.UpdateIconSource();
		}
	}
}
