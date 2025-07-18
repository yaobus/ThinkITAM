using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkITAM.DatabaseOperation;

namespace ThinkITAM.Functions.IPAddressHelper;
public class AddressStatistics
{
    /// <summary>
    /// 获取网段使用率
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static int GetUseValue(string tableName, string netMask, string network = null)
    {

        int maskLength = IPAddressCalculations.SubnetMaskToCidr(netMask);

        var addressCount = IPAddressCalculations.AddressCount(maskLength) - 2;

        //Console.WriteLine($"addressCount{addressCount}");

        int value = 0;

        if (maskLength < 24)//如果是大型网段
        {
            var info = SubnetCalculator.CalculateSubnets(network, maskLength);

            int index = 0;
            int useNum = 0;

            foreach (var sub in info.Item2)
            {
                //创建新的数据表名
                string newName = tableName + $"_Sub{index}";

                useNum += GetNetWorkUsedAddress(newName);

                index++;
            }

            value = Convert.ToInt32((useNum * 100) / addressCount);

        }
        else
        {
            int useNum = GetNetWorkUsedAddress(tableName);

            // Console.WriteLine($"useNum{useNum}");


            value = Convert.ToInt32((useNum * 100) / addressCount);
        }






        return value;

    }



    /// <summary>
    /// 获取单个网段已用地址数量，可计算任意大小的网段
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static int GetNetWorkUsedAddress(string tableName, string netMask, string network = null)
    {


        var maskLength = IPAddressCalculations.SubnetMaskToCidr(netMask);



        var count = 0;

        if (maskLength < 24)//如果是大型网段
        {
            var info = SubnetCalculator.CalculateSubnets(network, maskLength);

            var index = 0;
            var useNum = 0;

            foreach (var sub in info.Item2)
            {
                //创建新的数据表名
                string newName = tableName + $"_Sub{index}";

                useNum += GetNetWorkUsedAddress(newName);

                index++;
            }

            count = useNum;


        }
        else
        {
            var useNum = GetNetWorkUsedAddress(tableName);

            // Console.WriteLine($"useNum{useNum}");


            count = useNum;
        }






        return count;

    }


    /// <summary>
    /// 获取单个网段已用地址数量,只能计算256个地址以下的网段
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static int GetNetWorkUsedAddress(string tableName)
    {
        var sql = $"SELECT COUNT(*) FROM {tableName} WHERE AddressStatus NOT IN (0, 1, 4);";//查询已分配的地址数量


        return DbClass.ExecuteScalarTableNum(sql);
    }
}
