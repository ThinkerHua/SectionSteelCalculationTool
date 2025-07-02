using System.Globalization;
using System.Windows.Data;

namespace SectionSteelCalculationTool.ValueConverters {
    public class BooleanReverseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool) value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool) value;
    }
}
