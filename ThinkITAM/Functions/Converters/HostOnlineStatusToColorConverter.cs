using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters;

public class HostOnlineStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 检查传入的值是否为布尔类型
        if (value is bool && (bool)value)
        {
            // 如果 IsSelected 为 true，则返回 Thickness(3)，表示边框厚度为3
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff88"));
        }
        else
        {
            // 如果 IsSelected 为 false 或者不是布尔类型，则返回 Thickness(0)，表示没有边框
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#647178"));
        }
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}