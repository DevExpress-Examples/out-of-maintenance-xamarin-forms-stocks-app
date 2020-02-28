using System;
using System.ComponentModel;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Stocks.UI.Views;
using Stocks.UI.Views.Android;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using AGraphics = Android.Graphics;
using Android.Graphics;
using Android.App;
using Android.OS;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Stocks.UI.Views.Android {
    public class PopupPageRenderer : PageRenderer {
        public PopupPageRenderer(Context context) : base(context) {
        }

        PopupPage CurrentElement => (PopupPage)Element;

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b) {
            var activity = (Activity)Context;

            Thickness systemPadding;
            var decoreView = activity.Window.DecorView;
            var decoreHeight = decoreView.Height;
            var decoreWidht = decoreView.Width;

            var visibleRect = new Rect();
            decoreView.GetWindowVisibleDisplayFrame(visibleRect);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M) {
                var screenRealSize = new AGraphics.Point();
                activity.WindowManager.DefaultDisplay.GetRealSize(screenRealSize);

                var windowInsets = RootWindowInsets;
                var bottomPadding = Math.Min(windowInsets.StableInsetBottom, windowInsets.SystemWindowInsetBottom);

                systemPadding = new Thickness {
                    Left = Context.FromPixels(windowInsets.SystemWindowInsetLeft),
                    Top = Context.FromPixels(windowInsets.SystemWindowInsetTop),
                    Right = Context.FromPixels(windowInsets.SystemWindowInsetRight),
                    Bottom = Context.FromPixels(bottomPadding)
                };
            }
            else {
                var screenSize = new AGraphics.Point();
                activity.WindowManager.DefaultDisplay.GetSize(screenSize);
                systemPadding = new Thickness {
                    Left = Context.FromPixels(visibleRect.Left),
                    Top = Context.FromPixels(visibleRect.Top),
                    Right = Context.FromPixels(decoreWidht - visibleRect.Right),
                    Bottom = Context.FromPixels(decoreHeight - visibleRect.Bottom)
                };
            }

            CurrentElement.SetValue(PopupPage.SystemPaddingProperty, systemPadding);

            if (changed)
                CurrentElement.Layout(new Rectangle(Context.FromPixels(l), Context.FromPixels(t), Context.FromPixels(r), Context.FromPixels(b)));
            else
                CurrentElement.ForceLayout();

            base.OnLayout(changed, l, t, r, b);
        }
    }
}
