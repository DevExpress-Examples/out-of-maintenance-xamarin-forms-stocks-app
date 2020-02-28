using System;

using Xamarin.Forms;

namespace Stocks.UI.Views {
    public abstract class SelectableView : ContentView {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(SelectableView), default(bool), propertyChanged: OnIsSelectedPropertyChanged);
        public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(SelectableView), Color.Default, propertyChanged: OnAnyForegroundColorChanged);
        public static readonly BindableProperty SelectedForegroundColorProperty = BindableProperty.Create(nameof(SelectedForegroundColor), typeof(Color), typeof(SelectableView), Color.Default, propertyChanged: OnAnyForegroundColorChanged);

        public Color ActualForegroundColor => IsSelected && IsSet(SelectedForegroundColorProperty) ? SelectedForegroundColor : ForegroundColor;
        public bool IsSelected {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public Color ForegroundColor {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        public Color SelectedForegroundColor {
            get => (Color)GetValue(SelectedForegroundColorProperty);
            set => SetValue(SelectedForegroundColorProperty, value);
        }

        protected override void ChangeVisualState() {
            if (IsSelected && IsEnabled) {
                VisualStateManager.GoToState(this, "Selected");
            } else {
                base.ChangeVisualState();
            }
        }

        protected virtual void OnForegroundChanged() {

        }

        void UpdateState() {
            ChangeVisualState();
            OnForegroundChanged();
        }
        
        static void OnIsSelectedPropertyChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is SelectableView view)) return;
            view.UpdateState();
        }

        static void OnAnyForegroundColorChanged(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is SelectableView view)) return;
            view.OnForegroundChanged();
        }
    }
}

