using System.Globalization;
using System.Windows.Data;

namespace ScreTran;

class BoolToInvertedBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = (bool)value;
        return !boolValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = (bool)value;
        return !boolValue;
    }
}
