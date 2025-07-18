using System.Net;
using DocumentFormat.OpenXml.EMMA;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;

namespace ThinkITAM.Functions.FunctionClass
{
    public class StatisticsClass
    {

        /// <summary>
        ///  统计已有网段数量，不包含已删除的网段
        /// </summary>
        /// <returns></returns>
        public static int StatisticsNetworkCount()
        {
            var sql = $"SELECT COUNT(*) FROM Network WHERE (Del !=1 OR Del IS NULL)";


            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        }


        /// <summary>
        ///  统计已有IP地址数量，不包含已删除的网段
        /// </summary>
        /// <returns></returns>
        public static int StatisticsIpAddressCount()
        {
            var sql = $"SELECT * FROM Network WHERE Del !=1 OR Del IS NULL";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int count = 0;
            foreach (var row in rows)
            {

                count += Convert.ToInt32(IPAddressCalculations.GetAvailableAddresses(
                    IPAddressCalculations.SubnetMaskToCidr(row["Netmask"].ToString())));
                count += 2;
            }


            return count;
        }

        /// <summary>
        /// 统计可用地址总数（即IP地址总数-已用地址总数）
        /// </summary>
        /// <returns></returns>
        public static int StatisticsAvailableAddressCount()
        {
            //第一步，取出所有网段ID和网段地址以及子网掩码

            //第二步，计算网段的表名

            //第三步计算网段已用地址数

            string query = $"SELECT * FROM Network  WHERE Del != 1 OR Del IS NULL ;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            var count = 0;
            foreach (var row in rows)
            {
                var networkId = row["NetworkId"].ToString();

                var netmask = row["Netmask"].ToString();

                var network = row["Network"].ToString();

                IPAddress mask = IPAddress.Parse(netmask);


                count += AddressStatistics.GetNetWorkUsedAddress($"Net_{networkId}", netmask, network);
            }




            return count;
        }




        /// <summary>
        /// 统计设备数量，不包含已删除的设备
        /// </summary>
        /// <returns></returns>
        public static int StatisticsDevicesCount()
        {

            var sql = $"SELECT COUNT(*) FROM Devices WHERE (Del !=1 OR DEL IS NULL)";

            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
        }


        /// <summary>
        ///  统计机架数量，不包含已删除的机架
        /// </summary>
        /// <returns></returns>
        public static int StatisticsRackCount()
        {

            var sql = $"SELECT COUNT(*) FROM Racks WHERE (Del !=1 OR DEL IS NULL)";

            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        }


        /// <summary>
        /// 统计终端数量，不包含已删除的终端
        /// </summary>
        /// <returns></returns>
        public static int StatisticsComputerCount()
        {
            var sql = $"SELECT COUNT(*) FROM Computer WHERE (Del !=1 OR DEL IS NULL)";

            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
        }


        /// <summary>
        /// 统计节点数量
        /// </summary>
        /// <returns></returns>
        public static int StatisticsLinkCount()
        {
            var sql = $"SELECT COUNT(*) FROM Link";

            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
        }


        /// <summary>
        /// 统计节点数量
        /// </summary>
        /// <returns></returns>
        public static int StatisticsNodeCount()
        {
            var sql = $"SELECT COUNT(*) FROM LinkDetail";

            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
        }


        /// <summary>
        /// 统计可用设备端口数量
        /// </summary>
        /// <returns></returns>
        public static int StatisticAvailableDevicePortCount()
        {
            var sql = $"SELECT * FROM Devices WHERE (Del !=1 OR DEL IS NULL)";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);
            var count = 0;
            foreach (var row in rows)
            {
                var assetId = row["AssetId"].ToString();

                var query = $"SELECT COUNT(*) FROM De_{assetId} WHERE PortStatus = 0";

                count += Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
            }


            return count;
        }



        /// <summary>
        /// 统计可用机架端口数量
        /// </summary>
        /// <returns></returns>
        public static int StatisticAvailableRackPortCount()
        {
            var sql = $"SELECT * FROM Racks WHERE (Del !=1 OR DEL IS NULL)";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);
            var count = 0;
            foreach (var row in rows)
            {
                var assetId = row["RackId"].ToString();

                var query = $"SELECT COUNT(*) FROM Ra_{assetId} WHERE OnTheLine IS NULL ";

                count += Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
            }


            return count;
        }


        /// <summary>
        /// 统计资产类型数量
        /// </summary>
        /// <returns></returns>
        public static int StatisticAssetTypeCount()
        {
            var query = $"SELECT  COUNT(DISTINCT(AssetType))  FROM AssetTag ";
            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
        }


        /// <summary>
        /// 统计资产总数
        /// </summary>
        /// <returns></returns>
        public static int StatisticAssetCount()
        {
            var query = $"SELECT COUNT(*) FROM Asset WHERE Del !=1 OR DEL IS NULL ";
            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
        }


        /// <summary>
        /// 统计未部署资产总数
        /// </summary>
        /// <returns></returns>
        public static int StatisticsAssetUnDeploy()
        {
            var query = $"SELECT COUNT(*) FROM Asset WHERE Deploy IS NULL AND (Del !=1 OR DEL IS NULL )";
            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));
        }
    }
}