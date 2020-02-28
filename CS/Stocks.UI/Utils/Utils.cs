using DevExpress.XamarinForms.Editors;

namespace Stocks.UI.Utils {
    public static class TextEditUtils {
        public static void SetCursorInEnd(this TextEdit edit) {
            edit.Focus();
            edit.CursorPosition = edit.Text.Length;
        }
    }
}
