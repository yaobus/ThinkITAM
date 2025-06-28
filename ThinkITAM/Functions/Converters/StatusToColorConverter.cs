using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters;
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = (int)value;
        switch (status)
        {
            case 0: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#939597"));
            case 1: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#21a675"));
            case 2: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff6f61"));
            case 3: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f2b632"));
            case 4: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5B9F"));
            default: return new SolidColorBrush(Colors.Gray); // 默认颜色
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
