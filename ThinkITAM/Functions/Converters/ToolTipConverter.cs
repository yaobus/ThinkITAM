using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 检查值是否为 null 或者空字符串
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "使用默认浏览器打开";
            }
            else
            {
                return "使用指定浏览器打开";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // 如果不需要双向绑定，可以抛出异常
        }
    }
}
