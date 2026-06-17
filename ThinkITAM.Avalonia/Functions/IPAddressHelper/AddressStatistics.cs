using ThinkITAM.DataBridge;
using ThinkITAM.Shared.Network;

namespace ThinkITAM.Functions.IPAddressHelper
{
    /// <summary>
    /// IP地址统计辅助类
    /// 从 WPF 原项目迁移，IPAddressCalculations/SubnetCalculator 委托到 ThinkITAM.Shared.Network
    /// </summary>
    public class AddressStatistics
    {
        /// <summary>
        /// 获取网段使用率百分比
        /// </summary>
        public static int GetUseValue(string tableName, string netMask, string? network = null)
        {
            int maskLength = IPAddressCalculations.SubnetMaskToCidr(netMask);
            var addressCount = IPAddressCalculations.AddressCount(maskLength) - 2;

            int value;
            if (maskLength < 24 && network != null)
            {
                var info = SubnetCalculator.CalculateSubnets(network, maskLength);
                int index = 0;
                int useNum = 0;
                foreach (var sub in info.Item2)
                {
                    string newName = tableName + $"_Sub{index}";
                    useNum += GetNetWorkUsedAddress(newName);
                    index++;
                }
                value = Convert.ToInt32((useNum * 100) / addressCount);
            }
            else
            {
                int useNum = GetNetWorkUsedAddress(tableName);
                value = Convert.ToInt32((useNum * 100) / addressCount);
            }

            return value;
        }

        /// <summary>
        /// 获取单个网段已用地址数量（支持子网拆分）
        /// </summary>
        public static int GetNetWorkUsedAddress(string tableName, string netMask, string? network = null)
        {
            var maskLength = IPAddressCalculations.SubnetMaskToCidr(netMask);
            var count = 0;

            if (network != null && maskLength < 24)
            {
                var info = SubnetCalculator.CalculateSubnets(network, maskLength);
                var index = 0;
                var useNum = 0;
                foreach (var sub in info.Item2)
                {
                    string newName = tableName + $"_Sub{index}";
                    useNum += GetNetWorkUsedAddress(newName);
                    index++;
                }
                count = useNum;
            }
            else
            {
                count = GetNetWorkUsedAddress(tableName);
            }

            return count;
        }

        /// <summary>
        /// 获取单个网段已用地址数量（直接查询数据表）
        /// </summary>
        public static int GetNetWorkUsedAddress(string tableName)
        {
            var sql = $"SELECT COUNT(*) FROM {tableName} WHERE AddressStatus NOT IN (0, 1, 4);";

            if (GlobalVariables.DbService == null) return 0;

            var result = GlobalVariables.DbService.ExecuteScalar(sql);
            if (result == null) return 0;
            return Convert.ToInt32(result);
        }
    }
}
