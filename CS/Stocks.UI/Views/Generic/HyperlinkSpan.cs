using System;
using Stocks.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Stocks.UI.Views {
	public class HyperlinkSpan : Span {
		public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan));

		public string Url {
			get => (string)GetValue(UrlProperty);
			set => SetValue(UrlProperty, value);
		}

		public HyperlinkSpan() {
            GestureRecognizers.Add(new TapGestureRecognizer() { Command = new DelegateCommand(() => Launcher.OpenAsync(new Uri(Url))) });
		}
	}
}