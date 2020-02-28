using System;
using DevExpress.XamarinForms.DataGrid;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class PortfolioItemRow : ContentView {
        public static readonly BindableProperty ShortPositionColorProperty = BindableProperty.Create(nameof(ShortPositionColor), typeof(Color), typeof(PortfolioItemRow), Color.Default, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty LongPositionColorProperty = BindableProperty.Create(nameof(LongPositionColor), typeof(Color), typeof(PortfolioItemRow), Color.Default, propertyChanged: OnColorPropertyChanged);

        public static readonly BindableProperty RisingValueColorProperty = BindableProperty.Create(nameof(RisingValueColor), typeof(Color), typeof(PortfolioItemRow), Color.Default, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty FallingValueColorProperty = BindableProperty.Create(nameof(FallingValueColor), typeof(Color), typeof(PortfolioItemRow), Color.Default, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty NoneValueColorProperty = BindableProperty.Create(nameof(NoneValueColor), typeof(Color), typeof(PortfolioItemRow), Color.Default, propertyChanged: OnColorPropertyChanged);

        public Color ShortPositionColor {
            get => (Color)GetValue(ShortPositionColorProperty);
            set => SetValue(ShortPositionColorProperty, value);
        }
        public Color LongPositionColor {
            get => (Color)GetValue(LongPositionColorProperty);
            set => SetValue(LongPositionColorProperty, value);
        }
        public Color RisingValueColor {
            get => (Color)GetValue(RisingValueColorProperty);
            set => SetValue(RisingValueColorProperty, value);
        }
        public Color FallingValueColor {
            get => (Color)GetValue(FallingValueColorProperty);
            set => SetValue(FallingValueColorProperty, value);
        }
        public Color NoneValueColor {
            get => (Color)GetValue(NoneValueColorProperty);
            set => SetValue(NoneValueColorProperty, value);
        }

        public PortfolioItemRow() {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
            UpdateActualColors();
        }

        void UpdateActualColors() {
            if (!(BindingContext is CellData cellData)) return;
            if (!(cellData.Item is PortfolioItemViewModel viewModel)) return;
            countView.IndicatorColor = SelectColorByPositionType(viewModel.Type);
            
            Color priceChangeColor = SelectColorByChangeType(viewModel.PriceChangeType);
            priceChangeIcon.ForegroundColor = priceChangeColor;
            priceChangeLabel.TextColor = priceChangeColor;

            Color totalChangeColor = SelectColorByChangeType(viewModel.TotalChangeType);
            totalChangeIcon.ForegroundColor = totalChangeColor;
            totalChangeLabel.TextColor = totalChangeColor;
        }
        
        Color SelectColorByPositionType(PositionType type) {
            switch(type) {
                case PositionType.Long : return LongPositionColor;
                case PositionType.Short: return ShortPositionColor;
                default: throw new NotSupportedException("The specified 'PositionType' is not supported.");
            }
        }
        Color SelectColorByChangeType(ChangeType type) {
            switch (type) {
                case ChangeType.Rising: return RisingValueColor;
                case ChangeType.Falling: return FallingValueColor;
                case ChangeType.None: return NoneValueColor;
                default: throw new NotSupportedException("The specified 'ChangeType' is not supported.");
            }
        }

        static void OnColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is PortfolioItemRow row)) return;
            row.UpdateActualColors();
        }
    }
}
