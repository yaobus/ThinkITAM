namespace ThinkITAM.Shared.Asset
{
    public class AssetCodeClass
    {


        /// <summary>
        /// 生成校验位
        /// 主要用于创建资产码,首位0为机房，1为机柜，2为设备,3为机架，4为通用终端(计算机、IP电话等设备),7位笔记，8为建筑，9为人员，X为程序内容索引
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateChecksum(string input)
        {

            // 计算前15位字符的ASCII码值的总和
            int sum = 0;
            foreach (char c in input)
            {
                sum += (int)c;
            }

            // 使用26作为除数，取余得到校验值
            int checksumValue = sum % 26;

            // 将校验值转换为对应的字符（这里假设A对应0，B对应1，依此类推）
            char checksumChar = (char)('A' + checksumValue);

            // 返回带有校验字符的16位字符串
            return input + checksumChar;
        }


        /// <summary>
        /// 校验资产码是否正确
        /// </summary>
        /// <param name="input">完整的资产码（含校验位）</param>
        /// <returns>校验是否通过</returns>
        public static bool CheckAssetCode(string input)
        {
            string sourceStr = input.Substring(0, input.Length - 1);

            return GenerateChecksum(sourceStr) == input;
        }

    }
}
