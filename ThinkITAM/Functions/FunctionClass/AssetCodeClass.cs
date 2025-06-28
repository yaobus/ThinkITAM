using System.Windows;

namespace ThinkITAM.Functions.FunctionClass
{
    public class AssetCodeClass
    {


        /// <summary>
        /// 生成校验位
        /// 主要用于创建资产码,首位0为机房，1为机柜，2为设备,3为机架，4为通用终端(计算机、IP电话等设备)8为建筑，9为人员，X为程序内容索引
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


        public static bool CheckAssetCode(string input)
        {
            string sourceStr = input.Substring(0, input.Length - 1);

            if (GenerateChecksum(sourceStr) == input)
            {
                MessageBox.Show("校验成功");

                return true;

            }
            else
            {
                MessageBox.Show("校验失败");
                return false;
            }

        }

    }
}
