using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class BoolToBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 检查传入的值是否为布尔类型
            if (value is bool && (bool)value)
            {
                // 如果 IsSelected 为 true，则返回 Thickness(3)，表示边框厚度为3
                return new Thickness(3);
            }
            else
            {
                // 如果 IsSelected 为 false 或者不是布尔类型，则返回 Thickness(0)，表示没有边框
                return new Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 尝试将传入的 Thickness 类型的值转换为布尔值
            if (value is Thickness thickness)
            {
                // 假设当任何一边的边框厚度为3时，我们将其视为true
                return thickness.Left == 3 || thickness.Top == 3 || thickness.Right == 3 || thickness.Bottom == 3;
            }

            // 如果无法转换或传入的不是 Thickness 类型，则默认返回 false
            return false;
        }
    }
}
