using System;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class ValueDifferenceView : ContentView {
        public static readonly BindableProperty ChangeTypeProperty = BindableProperty.Create(nameof(ChangeType), typeof(ChangeType), typeof(ValueDifferenceView), null, propertyChanged: OnChangeTypePropertyChanged);
        public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(ValueDifferenceView), Color.Default, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty RisingForegroundColorProperty = BindableProperty.Create(nameof(RisingForegroundColor), typeof(Color), typeof(ValueDifferenceView), Color.Default, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty FallingForegroundColorProperty = BindableProperty.Create(nameof(FallingForegroundColor), typeof(Color), typeof(ValueDifferenceView), Color.Default, propertyChanged: OnColorPropertyChanged);


        public ChangeType ChangeType {
            get => (ChangeType)GetValue(ChangeTypeProperty);
            set => SetValue(ChangeTypeProperty, value);
        }

        public Color ForegroundColor {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        public Color RisingForegroundColor {
            get => (Color)GetValue(RisingForegroundColorProperty);
            set => SetValue(RisingForegroundColorProperty, value);
        }
        public Color FallingForegroundColor {
            get => (Color)GetValue(FallingForegroundColorProperty);
            set => SetValue(FallingForegroundColorProperty, value);
        }
        public Color ActualForegroundColor {
            get {
                switch (ChangeType) {
                    case ChangeType.None : return ForegroundColor;
                    case ChangeType.Rising: return RisingForegroundColor;
                    case ChangeType.Falling: return FallingForegroundColor;
                    default: throw new ArgumentException($"Cannot handle the specified '{ChangeType}' value of the ChangeType enum.");
                }
            }
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
        }

        protected override void ChangeVisualState() {
            if (IsEnabled) {
                switch (ChangeType) {
                    case ChangeType.Falling: VisualStateManager.GoToState(this, "Falling"); break;
                    case ChangeType.Rising: VisualStateManager.GoToState(this, "Rising"); break;
                    case ChangeType.None: base.ChangeVisualState(); break;
                }
            } else {
                base.ChangeVisualState();
            }
        }

        protected virtual void OnActualForegroundColorChanged() {

        }

        void UpdateActualState() {
            ChangeVisualState();
            OnActualForegroundColorChanged();
        }

        static void OnChangeTypePropertyChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is ValueDifferenceView row)) return;
            row.UpdateActualState();
        }

        static void OnColorPropertyChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is ValueDifferenceView row)) return;
            row.OnActualForegroundColorChanged();
        }
    }
}
