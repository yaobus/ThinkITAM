using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
   public class AddressStatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 尝试将传入的值转换为整数
            if (value is int intValue)
            {
                // 如果整数值等于2，返回true；否则返回false
                return intValue == 2;
            }
            // 如果无法转换为整数，则默认返回false
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要支持双向绑定，请在这里实现从bool到int的转换逻辑
            // 如果传入的值是true，返回2；如果是false，返回0或其他你认为合适的默认值
            if (value is bool boolValue && boolValue)
            {
                return 2;
            }
            return 0; // 或者其他非2的值，依据你的业务逻辑
        }
    }
}
