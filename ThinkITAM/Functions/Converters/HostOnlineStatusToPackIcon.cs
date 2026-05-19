using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace ThinkITAM.Functions.Converters;


public class HostOnlineStatusToPackIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 检查传入的值是否为布尔类型
        if (value is bool && (bool)value)
        {
           
            return PackIconKind.LanConnect;
            
        }
        else
        {
           
            return PackIconKind.LanDisconnect;
        }
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}