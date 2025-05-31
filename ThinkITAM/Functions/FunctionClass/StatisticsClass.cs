using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.ViewModels.NetworkManage;

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
    }
}