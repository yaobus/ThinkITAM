using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{

    /// <summary>
    /// 转换目标控件的宽度为隐式列宽
    /// </summary>
    public class ViewModeToColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                // 如果 ViewMode 是 Checked (即预览模式开启)，我们希望 InputTextBox 占据一部分宽度
                // 这里返回 "*" 表示等分剩余空间，或者你可以返回一个具体的 GridLength
                // 如果 ViewMode 是 Unchecked (即编辑模式)，我们希望 InputTextBox 占据全部宽度
                // 返回 "*" 在这种情况下也能实现拉伸填充，因为其他列会消失
                return new GridLength(1, GridUnitType.Star);
            }
            return new GridLength(1, GridUnitType.Star); // 默认返回 *
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
