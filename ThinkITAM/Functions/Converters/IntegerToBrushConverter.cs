using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ThinkITAM.Functions.Converters
{
    public class IntegerToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int colorIndex)
            {
                switch (colorIndex)
                {
                    case 1:
                        return ColorToBrush("#FF0080");
                    case 2:
                        return ColorToBrush("#F15A24");
                    case 3:
                        return ColorToBrush("#FBC02D");
                    case 4:
                        return ColorToBrush("#64DD17");
                    case 5:
                        return ColorToBrush("#00A8FF");
                    case 6:
                        return ColorToBrush("#008080");
                    case 7:
                        return ColorToBrush("#362391");
                    case 8:
                        return ColorToBrush("#212121");
                    default:
                        return Brushes.Transparent;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private SolidColorBrush ColorToBrush(string hexColor)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
        }
    }
}
