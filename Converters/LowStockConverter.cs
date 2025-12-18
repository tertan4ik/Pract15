using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pract_15.Converters
{
    public class LowStockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock && stock < 10)
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD129"));

            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#00B4C4"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
