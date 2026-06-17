using SharedCalc = ThinkITAM.Shared.Network.SubnetCalculator;
using SharedModel = ThinkITAM.Shared.Network.SubnetModel;
using System.Collections.ObjectModel;

namespace ThinkITAM.Functions.IPAddressHelper
{
    public class SubnetModel : SharedModel
    {
    }

    public class SubnetCalculator
    {
        /// <summary>
        /// 返回网段地址和子网范围
        /// </summary>
        public static (string, ObservableCollection<string>) CalculateSubnets(string ipAddress, int subnetMask)
            => SharedCalc.CalculateSubnets(ipAddress, subnetMask);

        /// <summary>
        /// 子网掩码转换为子网掩码长度
        /// </summary>
        public static int SubnetMaskToLength(string subnetMask)
            => SharedCalc.SubnetMaskToLength(subnetMask);
    }
}