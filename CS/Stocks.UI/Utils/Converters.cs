using System;
using System.Globalization;
using System.Linq;
using DevExpress.XamarinForms.Charts;
using Stocks.Models;
using Stocks.UI.Utils;
using Stocks.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stocks.UI.Converters {
    public class ChangeTypeToImageNameConverter : IValueConverter {
        public string NoneValue { get; set; } = string.Empty;
        public string RisingValue { get; set; } = string.Empty;
        public string FallingValue { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is ChangeType changeType) || targetType != typeof(string)) return null;
            switch (changeType) {
                case ChangeType.Rising: return RisingValue;
                case ChangeType.Falling: return FallingValue;
                case ChangeType.None: return NoneValue;
                default: throw new NotSupportedException($"The pecified {changeType} is not supported.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class ChangeTypeToImageNameConverterExtension : IMarkupExtension<ChangeTypeToImageNameConverter> {
        public string RisingValue { get; set; } = string.Empty;
        public string FallingValue { get; set; } = string.Empty;
        public string NoneValue { get; set; } = string.Empty;

        public ChangeTypeToImageNameConverter ProvideValue(IServiceProvider serviceProvider)
            => new ChangeTypeToImageNameConverter {
                RisingValue = this.RisingValue,
                FallingValue = this.FallingValue,
                NoneValue = this.NoneValue
            };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class BoolToImageNameConverter : IValueConverter {
        public string TrueValue { get; set; } = string.Empty;
        public string FalseValue { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolValue) || targetType != typeof(string)) return null;
            return boolValue ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class BoolToImageNameConverterExtension : IMarkupExtension<BoolToImageNameConverter> {
        public string TrueValue { get; set; } = string.Empty;
        public string FalseValue { get; set; } = string.Empty;

        public BoolToImageNameConverter ProvideValue(IServiceProvider serviceProvider)
            => new BoolToImageNameConverter {
                TrueValue = this.TrueValue,
                FalseValue = this.FalseValue
            };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    class InverseBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolValue) || targetType != typeof(bool)) return null;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);
    }
    class InverseBoolConverterExtension : IMarkupExtension<InverseBoolConverter> {
        public InverseBoolConverter ProvideValue(IServiceProvider serviceProvider) => new InverseBoolConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    class TimeFrameToDateTimeAxisTextFormatterConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is TimeFrame timeFrame) || targetType != typeof(IAxisLabelTextFormatter)) return null;
            return new DateTimeAxisTextFormatter(timeFrame);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    class TimeFrameToDateTimeAxisTextFormatterConverterExtension : IMarkupExtension<TimeFrameToDateTimeAxisTextFormatterConverter> {
        public TimeFrameToDateTimeAxisTextFormatterConverter ProvideValue(IServiceProvider serviceProvider) => new TimeFrameToDateTimeAxisTextFormatterConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    class TimeFrameToDateTimeMeasureUnitConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is TimeFrame timeFrame) || targetType != typeof(DateTimeMeasureUnit)) return null;
            switch (timeFrame) {
                case TimeFrame.M1:
                case TimeFrame.M2:
                case TimeFrame.M3:
                case TimeFrame.M4:
                case TimeFrame.M5:
                case TimeFrame.M6:
                case TimeFrame.M10:
                case TimeFrame.M12:
                case TimeFrame.M15:
                case TimeFrame.M20:
                case TimeFrame.M30:
                    return DateTimeMeasureUnit.Minute;
                case TimeFrame.H1:
                case TimeFrame.H2:
                case TimeFrame.H3:
                case TimeFrame.H4:
                    return DateTimeMeasureUnit.Hour;
                case TimeFrame.D: 
                    return DateTimeMeasureUnit.Day;
                case TimeFrame.W: 
                    return DateTimeMeasureUnit.Week;
                case TimeFrame.MN: 
                    return DateTimeMeasureUnit.Month;
                default:
                    throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    class TimeFrameToDateTimeMeasureUnitConverterExtension : IMarkupExtension<TimeFrameToDateTimeMeasureUnitConverter> {
        public TimeFrameToDateTimeMeasureUnitConverter ProvideValue(IServiceProvider serviceProvider) => new TimeFrameToDateTimeMeasureUnitConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }


    public class ListTypeToListCaptionConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is SymbolListType listType) || (targetType != typeof(string))) return null;
            return listType.GetDescription().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class ListTypeToListCaptionConverterExtension : IMarkupExtension<ListTypeToListCaptionConverter> {
        public ListTypeToListCaptionConverter ProvideValue(IServiceProvider serviceProvider) => new ListTypeToListCaptionConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

	public class GroupedColumnValueToCaptionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is DateTime date)
			    return date.ToShortDateString();
			return value ?? string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class GroupedColumnValueToCaptionConverterExtension : IMarkupExtension<GroupedColumnValueToCaptionConverter> {
		public GroupedColumnValueToCaptionConverter ProvideValue(IServiceProvider serviceProvider) => new GroupedColumnValueToCaptionConverter();
		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
	}


	public class ToUppercaseStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(string)) return null;
            return value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class ToUppercaseStringConverterExtension : IMarkupExtension<ToUppercaseStringConverter> {
        public ToUppercaseStringConverter ProvideValue(IServiceProvider serviceProvider) => new ToUppercaseStringConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }


    public class ObjectToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class ObjectToBoolConverterExtension : IMarkupExtension<ObjectToBoolConverter> {
        public ObjectToBoolConverter ProvideValue(IServiceProvider serviceProvider) => new ObjectToBoolConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class ObjectComparerConverter : IValueConverter {
        public object Reference { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.Equals(Reference);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class ObjectComparerConverterExtension : IMarkupExtension<ObjectComparerConverter> {
        public object Reference { get; set; }
        public ObjectComparerConverter ProvideValue(IServiceProvider serviceProvider) => new ObjectComparerConverter { Reference = this.Reference };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class InverseObjectComparerConverter : IValueConverter {
        public object Reference { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !value.Equals(Reference);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class InverseObjectComparerConverterExtension : IMarkupExtension<InverseObjectComparerConverter> {
        public object Reference { get; set; }
        public InverseObjectComparerConverter ProvideValue(IServiceProvider serviceProvider) => new InverseObjectComparerConverter { Reference = this.Reference };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class ToFilenameConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is string title) || (targetType != typeof(string))) return null;
            return $"{title.ToLower().Replace(' ', '_')}.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class ToFilenameConverterExtension : IMarkupExtension<ToFilenameConverter> {
        public ToFilenameConverter ProvideValue(IServiceProvider serviceProvider) => new ToFilenameConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class BoolToStringConverter : IValueConverter {
        public string TrueString { get; set; }
        public string FalseString { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolValue) || (targetType != typeof(string))) return null;
            return boolValue ? TrueString : FalseString;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    class BoolToStringConverterExtension : IMarkupExtension<BoolToStringConverter> {
        public string TrueString { get; set; }
        public string FalseString { get; set; }

        public BoolToStringConverter ProvideValue(IServiceProvider serviceProvider) => new BoolToStringConverter {
            TrueString = TrueString,
            FalseString = FalseString
        };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class BoolToColorConverter : IValueConverter {
        public Color TrueColor { get; set; }
        public Color FalseColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolValue) || (targetType != typeof(Color))) return null;
            return boolValue ? TrueColor : FalseColor;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    class BoolToColorConverterExtension : IMarkupExtension<BoolToColorConverter> {
        public Color TrueColor { get; set; }
        public Color FalseColor { get; set; }

        public BoolToColorConverter ProvideValue(IServiceProvider serviceProvider) => new BoolToColorConverter {
            TrueColor = TrueColor,
            FalseColor = FalseColor
        };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class RowIndexToColorConverter : IValueConverter {
        public Color[] Palette { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is int intValue) || (targetType != typeof(Color))) return null;
            return Palette[intValue % Palette.Length];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    class RowIndexToColorConverterExtension : IMarkupExtension<RowIndexToColorConverter> {
        public Color[] Palette { get; set; }

        public RowIndexToColorConverter ProvideValue(IServiceProvider serviceProvider) => new RowIndexToColorConverter() { Palette = Palette };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }


    public class StringToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            //Next conditions do not work correctly:
            //if (!(value is string stringValue) || targetType != typeof(bool)) {
            //    return null;
            //}
            return !string.IsNullOrEmpty(value as string);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class StringToBoolConverterExtension : IMarkupExtension<StringToBoolConverter> {
        public StringToBoolConverter ProvideValue(IServiceProvider serviceProvider) => new StringToBoolConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class StringToInverseBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            //Next conditions do not work correctly:
            //if (!(value is string stringValue) || targetType != typeof(bool)) { 
            //    return null;
            //}
            return string.IsNullOrEmpty(value as string);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class StringToInverseBoolConverterExtension : IMarkupExtension<StringToInverseBoolConverter> {
        public StringToInverseBoolConverter ProvideValue(IServiceProvider serviceProvider) => new StringToInverseBoolConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class EnumToItemsSource : IMarkupExtension<object> {
        private readonly Type type;

        public EnumToItemsSource(Type type) {
            this.type = type;
        }

        public object ProvideValue(IServiceProvider serviceProvider) {
            return Enum.GetValues(type)
                .Cast<object>()
                .Select(e => new { Value = (int)e, DisplayName = e.ToString() });
        }
    }

    public class InitializableViewModelStateToPlaceholderVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is InitializableViewModelState state) || (targetType != typeof(bool))) return null;
            var result =
                   state == InitializableViewModelState.Initialized
                || state == InitializableViewModelState.Loading;
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class InitializableViewModelStateToPlaceholderVisibilityConverterExtension : IMarkupExtension<InitializableViewModelStateToPlaceholderVisibilityConverter> {
        public InitializableViewModelStateToPlaceholderVisibilityConverter ProvideValue(IServiceProvider serviceProvider) => new InitializableViewModelStateToPlaceholderVisibilityConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class InitializableViewModelStateToContentVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is InitializableViewModelState state) || (targetType != typeof(bool))) return null;
            var result = state == InitializableViewModelState.HasContent
                || state == InitializableViewModelState.Reloading;
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class InitializableViewModelStateToContentVisibilityConverterExtension : IMarkupExtension<InitializableViewModelStateToContentVisibilityConverter> {
        public InitializableViewModelStateToContentVisibilityConverter ProvideValue(IServiceProvider serviceProvider) => new InitializableViewModelStateToContentVisibilityConverter();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

    public class BoolToImageSourceConverter : IValueConverter {
        public ImageSource TrueImageSource { get; set; }
        public ImageSource FalseImageSource { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolValue) || (targetType != typeof(ImageSource))) return null;
            return boolValue ? TrueImageSource : FalseImageSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class BoolToImageSourceConverterExtension : IMarkupExtension<BoolToImageSourceConverter> {
        public ImageSource TrueImageSource { get; set; }
        public ImageSource FalseImageSource { get; set; }
        public BoolToImageSourceConverter ProvideValue(IServiceProvider serviceProvider) => new BoolToImageSourceConverter {
            TrueImageSource = TrueImageSource,
            FalseImageSource = FalseImageSource
        };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}

