using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SectionSteelCalculationTool.ValueConverters {
    public class BooleanMultiConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return values.Cast<bool>().Any(b => b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
