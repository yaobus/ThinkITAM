using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters;
public class IndexToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 检查值是否为-1，如果不是，则返回true以启用按钮；如果是，则返回false禁用按钮。
        return !(value is int index && index == -1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
