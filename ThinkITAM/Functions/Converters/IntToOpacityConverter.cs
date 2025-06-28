using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class IntToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue == -1 ? 0.3 : 1.0;
            }

            // 默认返回1.0（不透明）
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}