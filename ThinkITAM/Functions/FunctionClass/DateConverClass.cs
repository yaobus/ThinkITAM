using SharedImpl = ThinkITAM.Shared.Helpers.DateConverClass;

namespace ThinkITAM.Functions.FunctionClass
{
    public class DateConverClass
    {
        public static DateTime ConvertExcelDateToDateTime(double excelDate)
            => SharedImpl.ConvertExcelDateToDateTime(excelDate);
    }
}
