using System;
using System.Threading.Tasks;

namespace Stocks.Models.Utils {
    public interface IErrorHandler {
        void HandleError(Exception exception);
    }

    public static class TaskUtils {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void InvokeAndForgetSafeAsync(this Task task, IErrorHandler handler = null) {
            try {
                await task;
            } catch (Exception ex) {
                handler?.HandleError(ex);
            }
        }
    }
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
}

