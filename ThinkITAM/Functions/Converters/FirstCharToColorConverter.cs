using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters
{
    public class FirstCharToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                var firstChar = str[0];

                switch (firstChar)
                {
                    case '3'://RACK

                        return ColorConverterClass.ColorToBrush("#139487");
                    case '2'://DEVICE
                        return ColorConverterClass.ColorToBrush("#4F9153");
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
