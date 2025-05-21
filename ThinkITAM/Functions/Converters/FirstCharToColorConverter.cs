using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class FirstCharToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                var firstChar = str[0];

                Console.WriteLine(firstChar);

                switch (firstChar)
                {
                    case '3':
                        return Brushes.DarkCyan;
                    case '2':
                        return Brushes.Tomato;
                }
            }

            // 可选：返回默认颜色，如灰色
            // return Brushes.Gray;

            // 或者返回 null，让绑定系统使用默认值或触发回退值
            return Brushes.OliveDrab;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
