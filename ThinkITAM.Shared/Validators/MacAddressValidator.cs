using System.Text.RegularExpressions;

namespace ThinkITAM.Shared.Validators
{
    public class MacAddressValidator
    {

        public static string ValidateAndFormatMacAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // 正则验证MAC地址格式（允许不带分隔符、用 : 或 - 分隔）
            string pattern = @"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$";
            if (!Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                return null;

            // 提取所有16进制的两字节部分，并转为大写
            var matches = Regex.Matches(input, @"[0-9A-Fa-f]{2}", RegexOptions.IgnoreCase);
            string[] parts = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                parts[i] = matches[i].Value.ToUpper();
            }

            // 返回标准格式：XX:XX:XX:XX:XX:XX
            return string.Join(":", parts);
        }

    }
}
