using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SectionSteelCalculationTool.ValueConverters {
    public class EnumBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.Equals(parameter) ?? DependencyProperty.UnsetValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.Equals(true) == true ? parameter : Binding.DoNothing;
    }
}
