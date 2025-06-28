using System.Globalization;
using System.Windows.Data;

namespace ThinkITAM.Functions.Converters;
public class RackIdToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string rackId = value as string;
        if (!string.IsNullOrEmpty(rackId) && rackId.Length > 0 && rackId[0] == '8')
        {
            return "建筑:";
        }
        else
        {
            return "机房:";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}

public class RackIdToSlotOrFloorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string rackId = value as string;
        if (!string.IsNullOrEmpty(rackId) && rackId.Length > 0 && rackId[0] == '8')
        {
            return "楼层:";
        }
        else
        {
            return "槽位:";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}