namespace ThinkITAM.Shared.Helpers
{
    public class DateConverClass
    {


        public static DateTime ConvertExcelDateToDateTime(double excelDate)
        {
            // Excel 的日期系统从 1900-01-01 开始计算天数
            // 但 Excel 错误地认为 1900 年是闰年，所以需要调整
            DateTime baseDate = new DateTime(1899, 12, 30); // 修正 Excel 的 1900 闰年问题
            return baseDate.AddDays(excelDate);
        }
    }
}
