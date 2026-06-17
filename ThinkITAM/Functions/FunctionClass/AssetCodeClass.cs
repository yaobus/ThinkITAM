using System.Windows;
using SharedImpl = ThinkITAM.Shared.Asset.AssetCodeClass;

namespace ThinkITAM.Functions.FunctionClass
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
            => SharedImpl.GenerateChecksum(input);


        public static bool CheckAssetCode(string input)
        {
            bool result = SharedImpl.CheckAssetCode(input);

            if (result)
            {
                MessageBox.Show("校验成功");
            }
            else
            {
                MessageBox.Show("校验失败");
            }

            return result;
        }

    }
}
