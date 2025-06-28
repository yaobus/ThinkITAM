using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace ThinkITAM.Functions.Converters
{
    public class PortTypeToIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string portType)
            {
                switch (portType.ToUpper())
                {
                    case "E":
                        return PackIconKind.Ethernet;
                    case "F":
                        return PackIconKind.CreditCardOutline;
                    case "D":
                        return PackIconKind.Harddisk;
                    case "M":
                        return PackIconKind.Ethernet;
                    case "Eth":
                        return PackIconKind.Ethernet;
                    case "FC":
                        return PackIconKind.CreditCardOutline;
                    case "SC":
                        return PackIconKind.CreditCardOutline;
                    case "LC":
                        return PackIconKind.CreditCardOutline;



                    default:
                        return PackIconKind.Help; // 或者其他默认图标
                }
            }
            return PackIconKind.Help; // 或者其他默认图标
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // 如果不需要双向转换，可以抛出异常
        }
    }
}
