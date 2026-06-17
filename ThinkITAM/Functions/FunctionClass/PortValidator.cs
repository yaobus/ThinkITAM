using SharedImpl = ThinkITAM.Shared.Validators.PortValidator;

namespace ThinkITAM.Functions.FunctionClass
{
    public class PortValidator
    {
        /// <summary>
        /// 验证输入字符串是否为合法端口号
        /// 如果合法，返回对应的整数；否则返回 null
        /// </summary>
        public static int? ValidatePort(string input)
            => SharedImpl.ValidatePort(input);
    }
}
