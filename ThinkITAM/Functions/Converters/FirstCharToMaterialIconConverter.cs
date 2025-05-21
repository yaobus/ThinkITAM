using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters
{
    public class FirstCharToMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                var firstChar = str[0];

                switch (firstChar)
                {
                    case '3':


                        return PackIconKind.VideoInputComponent ;
                    case '2':
                       
                        return PackIconKind.Laptop ;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
