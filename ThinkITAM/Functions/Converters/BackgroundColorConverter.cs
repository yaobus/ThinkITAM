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
                        return ColorConverterClass.ColorToBrush("#da5c53");//设备
                    case '3':
                        return ColorConverterClass.ColorToBrush("#4aa3ba");//机架
                    case '8':
                        return ColorConverterClass.ColorToBrush("#4bbb8b"); ;//面板

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
