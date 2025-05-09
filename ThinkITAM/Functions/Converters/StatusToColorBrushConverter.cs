using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters
{
    public class StatusToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                switch (status)
                {
                    case 0: //未分配
                        return ColorConverterClass.ColorToBrush("#21a675");
                    case 1: //已分配未启用
                        return ColorConverterClass.ColorToBrush("#f2b632");
                    case 2: //已分配，已启用
                        return ColorConverterClass.ColorToBrush("#ff6f61");
                    case 3: //故障
                        return ColorConverterClass.ColorToBrush("#4F4E48");
                    default:
                        return Brushes.AliceBlue; // 默认颜色
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // 如果不需要双向转换，可以抛出异常
        }
    }
}