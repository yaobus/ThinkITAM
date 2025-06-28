using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters;

/// <summary>
/// 布尔值反转转换器
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool)
        {
            return !(bool)value;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}