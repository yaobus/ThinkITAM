using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters;

/// <summary>
/// 将ToggleButton的状态转换为控件的显示和隐藏
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            bool isInverted = false;
            if (parameter != null)
            {
                isInverted = (parameter.ToString() == "Inverse");
            }
            boolValue = isInverted ? !boolValue : boolValue;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {


        return null;
    }
}
