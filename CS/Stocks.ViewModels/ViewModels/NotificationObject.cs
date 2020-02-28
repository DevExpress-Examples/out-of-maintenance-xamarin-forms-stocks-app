using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stocks.ViewModels {
    public class NotificationObject : INotifyPropertyChanging, INotifyPropertyChanged {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName]string propertyName = null, Action<T, T> onChanging = null, Action<T, T> onChanged = null) {
            if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
            T oldValue = backingField;
            onChanging?.Invoke(oldValue, value);
            RaisePropertyChanging(propertyName);
            backingField = value;
            onChanged?.Invoke(oldValue, value);
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanging(string propertyName) {
            PropertyChangingEventHandler handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
