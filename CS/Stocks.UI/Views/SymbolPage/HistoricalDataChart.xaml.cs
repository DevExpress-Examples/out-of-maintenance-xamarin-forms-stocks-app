using System.Collections.Generic;
using Xamarin.Forms;
using DevExpress.XamarinForms.Charts;
using System;

namespace Stocks.UI.Views {
    public partial class HistoricalDataChart : ContentView {
        public static BindableProperty IsRenderingSuspendedProperty = BindableProperty.Create(nameof(IsRenderingSuspended), typeof(bool), typeof(HistoricalDataChart), default(bool), propertyChanged: OnIsRenderingSuspendedProperty);

        public bool IsRenderingSuspended {
            get => (bool)GetValue(IsRenderingSuspendedProperty);
            set => SetValue(IsRenderingSuspendedProperty, value);
        }

        public HistoricalDataChart() {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
            this.RemoveBinding(IsRenderingSuspendedProperty);
            this.SetBinding(IsRenderingSuspendedProperty, new Binding { Path = "IsUpdateLocked" });
        }

        void UpdateRendering() {
            if (IsRenderingSuspended) {
                ohlcChart.SuspendRender();
            } else {
                ohlcChart.ResumeRender();
            }
        }

        static void OnIsRenderingSuspendedProperty(BindableObject bo, object oldValue, object newValue) {
            if (!(bo is HistoricalDataChart dataChart)) return;
            dataChart.UpdateRendering();
        }
    }

    class VolumeLabelTextFormatter: IAxisLabelTextFormatter {
        static readonly IDictionary<double, string> Abbreviations = new Dictionary<double, string> {
            { 1_000_000_000.0, "B" },
            { 1_000_000.0, "M" },
            { 1_000.0, "K" },
            { 1.0, "" }
        };
        const string LabelFormat = "{0:#.#}{1}";

        public string Format(object value) {
            if (!(value is double doubleValue)) return string.Empty;
            foreach (KeyValuePair<double, string> abbreviation in Abbreviations) {
                double abbreviatedValue = doubleValue / abbreviation.Key;
                if (Math.Abs(abbreviatedValue) >= 1.0) return string.Format(LabelFormat, abbreviatedValue, abbreviation.Value);
            }
            return string.Empty;
        }
    }
}
