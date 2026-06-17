using System.Globalization;
using Avalonia.Data.Converters;

namespace ThinkITAM.Functions.Converters;

public class FirstChar2ToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}