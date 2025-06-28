using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.FunctionClass
{

    /// <summary>
    /// 状态号转换为对应的颜色
    /// </summary>
    public class StatusToBackgroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                // 根据status值返回不同的背景色
                switch (status)
                {
                    case 1:
                        return Converters.ColorConverterClass.ColorToBrush("#f2b632");
                    case 2:
                        return Converters.ColorConverterClass.ColorToBrush("#ff6f61");
                    case 3:
                        return Converters.ColorConverterClass.ColorToBrush("#4F4E48");
                    default:
                        return Converters.ColorConverterClass.ColorToBrush("#21a675");
                }
            }
            return Converters.ColorConverterClass.ColorToBrush("#21a675");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 状态号转换为对应的名称
    /// </summary>
    public class StatusToNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                // 根据status值返回不同的背景色
                switch (status)
                {
                    case 1:
                        return "已分配未启用";
                    case 2:
                        return "已分配已启用";
                    case 3:
                        return "故障";
                    default:
                        return "未分配";
                }
            }
            return "未分配";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 将 状态号转换为对应的背景色
    /// </summary>
    public class CustomColorIndexToColor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                // 根据status值返回不同的背景色
                switch (status)
                {
                    case 1:
                        return Converters.ColorConverterClass.ColorToBrush("#FF0080");
                    case 2:
                        return Converters.ColorConverterClass.ColorToBrush("#F15A24");
                    case 3:
                        return Converters.ColorConverterClass.ColorToBrush("#FBC02D");

                    case 4:
                        return Converters.ColorConverterClass.ColorToBrush("#64DD17");
                    case 5:
                        return Converters.ColorConverterClass.ColorToBrush("#00A8FF");
                    case 6:
                        return Converters.ColorConverterClass.ColorToBrush("#008080");
                    case 7:
                        return Converters.ColorConverterClass.ColorToBrush("#362391");

                    default://0
                        return Brushes.Transparent;
                }
            }
            return "未分配";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
