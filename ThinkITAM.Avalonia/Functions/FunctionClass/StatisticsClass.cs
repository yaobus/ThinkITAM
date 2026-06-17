using System.Net;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.Shared.Network;

namespace ThinkITAM.Functions.FunctionClass
{
    /// <summary>
    /// 全局统计工具类
    /// 从 WPF 原项目迁移，将 DatabaseOperation.DbClass 替换为 GlobalVariables.DbService
    /// </summary>
    public class StatisticsClass
    {
        /// <summary>
        /// 统计已有网段数量，不包含已删除的网段
        /// </summary>
        public static int StatisticsNetworkCount()
        {
            var sql = "SELECT COUNT(*) FROM Network WHERE (Del !=1 OR Del IS NULL)";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计已有IP地址数量（所有网段的可用地址总数）
        /// </summary>
        public static int StatisticsIpAddressCount()
        {
            var sql = "SELECT * FROM Network WHERE Del !=1 OR Del IS NULL";
            var rows = GlobalVariables.DbService!.ExecuteQuery(sql);

            int count = 0;
            foreach (var row in rows)
            {
                count += Convert.ToInt32(IPAddressCalculations.GetAvailableAddresses(
                    IPAddressCalculations.SubnetMaskToCidr(row["Netmask"].ToString()!)));
                count += 2;
            }
            return count;
        }

        /// <summary>
        /// 统计可用地址总数（即所有网段已用地址数之和）
        /// </summary>
        public static int StatisticsAvailableAddressCount()
        {
            string query = "SELECT * FROM Network WHERE Del != 1 OR Del IS NULL;";
            var rows = GlobalVariables.DbService!.ExecuteQuery(query);
            var count = 0;
            foreach (var row in rows)
            {
                var networkId = row["NetworkId"].ToString()!;
                var netmask = row["Netmask"].ToString()!;
                var network = row["Network"].ToString()!;
                count += AddressStatistics.GetNetWorkUsedAddress($"Net_{networkId}", netmask, network);
            }
            return count;
        }

        /// <summary>
        /// 统计设备数量，不包含已删除的设备
        /// </summary>
        public static int StatisticsDevicesCount()
        {
            var sql = "SELECT COUNT(*) FROM Devices WHERE (Del !=1 OR DEL IS NULL)";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计机架数量，不包含已删除的机架
        /// </summary>
        public static int StatisticsRackCount()
        {
            var sql = "SELECT COUNT(*) FROM Racks WHERE (Del !=1 OR DEL IS NULL)";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计终端数量，不包含已删除的终端
        /// </summary>
        public static int StatisticsComputerCount()
        {
            var sql = "SELECT COUNT(*) FROM Computer WHERE (Del !=1 OR DEL IS NULL)";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计链路数量
        /// </summary>
        public static int StatisticsLinkCount()
        {
            var sql = "SELECT COUNT(*) FROM Link";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计链路节点数量
        /// </summary>
        public static int StatisticsNodeCount()
        {
            var sql = "SELECT COUNT(*) FROM LinkDetail";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(sql));
        }

        /// <summary>
        /// 统计可用设备端口数量（所有设备中 PortStatus=0 的端口数）
        /// </summary>
        public static int StatisticAvailableDevicePortCount()
        {
            var sql = "SELECT * FROM Devices WHERE (Del !=1 OR DEL IS NULL)";
            var rows = GlobalVariables.DbService!.ExecuteQuery(sql);
            var count = 0;
            foreach (var row in rows)
            {
                var assetId = row["AssetId"].ToString()!;
                var query = $"SELECT COUNT(*) FROM De_{assetId} WHERE PortStatus = 0";
                count += Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
            }
            return count;
        }

        /// <summary>
        /// 统计可用机架端口数量（所有机架中未连接的端口数）
        /// </summary>
        public static int StatisticAvailableRackPortCount()
        {
            var sql = "SELECT * FROM Racks WHERE (Del !=1 OR DEL IS NULL)";
            var rows = GlobalVariables.DbService!.ExecuteQuery(sql);
            var count = 0;
            foreach (var row in rows)
            {
                var assetId = row["RackId"].ToString()!;
                var query = $"SELECT COUNT(*) FROM Ra_{assetId} WHERE OnTheLine IS NULL";
                count += Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
            }
            return count;
        }

        /// <summary>
        /// 统计资产类型数量（不重复的 AssetType 数）
        /// </summary>
        public static int StatisticAssetTypeCount()
        {
            var query = "SELECT COUNT(DISTINCT(AssetType)) FROM AssetTag";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(query));
        }

        /// <summary>
        /// 统计资产总数
        /// </summary>
        public static int StatisticAssetCount()
        {
            var query = "SELECT COUNT(*) FROM Asset WHERE Del !=1 OR DEL IS NULL";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(query));
        }

        /// <summary>
        /// 统计未部署资产总数（Deploy 字段为 NULL 的）
        /// </summary>
        public static int StatisticsAssetUnDeploy()
        {
            var query = "SELECT COUNT(*) FROM Asset WHERE Deploy IS NULL AND (Del !=1 OR DEL IS NULL)";
            return Convert.ToInt32(GlobalVariables.DbService!.ExecuteScalar(query));
        }
    }
}
