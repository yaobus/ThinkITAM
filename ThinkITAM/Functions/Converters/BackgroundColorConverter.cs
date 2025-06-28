using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                char firstChar = str[0];

                switch (firstChar)
                {
                    case '2':
                        return ColorConverterClass.ColorToBrush("#e2473f");//设备
                    case '3':
                        return ColorConverterClass.ColorToBrush("#00a0dd");//机架
                    case '4':
                        return ColorConverterClass.ColorToBrush("#00c853"); //终端设备
                    case '8':
                        return ColorConverterClass.ColorToBrush("#f39700"); //面板

                }
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
