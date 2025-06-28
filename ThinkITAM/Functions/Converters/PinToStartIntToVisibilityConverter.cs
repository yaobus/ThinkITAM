using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    /// <summary>
    /// 书签和WOL，ping到主页时，图钉显示的状态
    /// </summary>
    public class PinToStartIntToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                // 如果整数值为1，则返回Visibility.Visible（显示），否则返回Visibility.Collapsed（隐藏）
                return intValue == 1 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Visible; // 默认返回可见
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // 如果不需要双向绑定，可以抛出未实现异常
        }

    }
}
