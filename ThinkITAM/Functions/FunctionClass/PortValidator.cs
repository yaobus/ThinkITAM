using System.Text.RegularExpressions;

namespace ThinkITAM.Functions.FunctionClass
{
    public class PortValidator
    {

        /// <summary>
        /// 验证输入字符串是否为合法端口号
        /// 如果合法，返回对应的整数；否则返回 null
        /// </summary>
        public static int? ValidatePort(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // 使用正则表达式验证是否是纯数字
            if (!Regex.IsMatch(input, @"^\d+$"))
                return null;

            // 转换为整数
            if (!int.TryParse(input, out int port))
                return null;

            // 检查端口范围是否合法（0 ~ 65535）
            if (port < 0 || port > 65535)
                return null;

            return port;
        }
    }
}
