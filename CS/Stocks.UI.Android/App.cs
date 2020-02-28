using System;
using Android.App;
using Android.Runtime;

namespace Stocks.UI.Android {

#if DEBUG
    [Application(Debuggable=true)]
#else
	[Application(Debuggable = false)]
#endif
	public class MainApp : Application {
		public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		public override void OnCreate() {
			base.OnCreate();
        }
    }
}
