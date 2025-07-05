using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters
{
    public class BackgroundColorMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 确保至少有一个有效的输入值
            if (values.Length >= 2)
            {
                string rackId = values[0] as string;
                string deviceId = values[1] as string;

                // 优先使用DeviceId
                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    char firstChar = deviceId[0];
                    switch (firstChar)
                    {
                        default:
                            return ColorConverterClass.ColorToBrush("#00c853"); // 终端设备
                    }
                }
                else if (!string.IsNullOrWhiteSpace(rackId)) // 如果没有deviceId，则检查rackId
                {
                    char firstChar = rackId[0];
                    switch (firstChar)
                    {
                        case '2':
                            return ColorConverterClass.ColorToBrush("#e2473f"); // 设备
                        case '3':
                            return ColorConverterClass.ColorToBrush("#00a0dd"); // 机架
                        case '4':
                            return ColorConverterClass.ColorToBrush("#00c853"); // 终端设备
                        case '8':
                            return ColorConverterClass.ColorToBrush("#f39700"); // 面板
                        default:
                            return Brushes.Transparent;
                    }
                }
            }

            return Brushes.Transparent; // 默认返回透明色
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

