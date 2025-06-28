using System.Text.RegularExpressions;

namespace ThinkITAM.Functions.FunctionClass
{
    public class MacAddressParser
    {

        public static List<string> ParseMacAddresses(string input)
        {

            string newInput = input.Replace("：", ":");

            // 正则表达式匹配不同的MAC地址格式
            string pattern = @"(?:[0-9A-Fa-f]{2}([-:]))(?:[0-9A-Fa-f]{2}\1){4}[0-9A-Fa-f]{2}|(?:[0-9A-Fa-f]{2}){6}";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(newInput);

            List<string> formattedMacAddresses = new List<string>();

            foreach (Match match in matches)
            {
                string macAddress = match.Value;
                // 去除所有分隔符
                string normalizedMac = Regex.Replace(macAddress, "[-:]", "");
                // 格式化为标准格式
                string formattedMac = string.Join(":",
                    Enumerable.Range(0, normalizedMac.Length / 2)
                        .Select(i => normalizedMac.Substring(i * 2, 2)));
                formattedMacAddresses.Add(formattedMac.ToUpper());
            }

            return formattedMacAddresses;
        }

    }
}
