using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Functions.FunctionClass;




public class NetworkHelper
{

    /// <summary>
    /// 获取全部网段信息，用于检查网段表单是否缺少字段
    /// </summary>
    /// <param name="networkId"></param>
    /// <returns></returns>
    public static ObservableCollection<ExportNetworkInfoClass> GetAllNetworkInfo()
    {

        var lists = new List<NetworkInfoViewMode>();

        var sql = $"SELECT * FROM Network ";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);


        foreach (var row in rows)
        {
            var item = new NetworkInfoViewMode();
            item.Network = row["Network"].ToString();
            item.Netmask = row["Netmask"].ToString();
            item.NetworkId = row["NetworkId"].ToString();
            lists.Add(item);
        }




        return GetAllNetwork(lists);
    }




    /// <summary>
    /// 获取所有网段信息，用于全局搜索
    /// </summary>
    /// <returns></returns>
    public static ObservableCollection<ExportNetworkInfoClass> GetAllNetwork(List<NetworkInfoViewMode> infos)
    {

        //要导出的全部表名
        var exportNetworkInfos = new ObservableCollection<ExportNetworkInfoClass>();


        //获取网段信息及网段自定义字段信息，并导出
        foreach (var item in infos)
        {
            //获取子网掩码位数
            int netmask = SubnetCalculator.SubnetMaskToLength(item.Netmask);


            if (netmask >= 24) //小型网段
            {
                //网段信息
                var info = new ExportNetworkInfoClass();

                info.Network = item.Network;
                info.Netmask = item.Netmask;
                info.TableName = $"Net_{item.NetworkId}";

                exportNetworkInfos.Add(info);
            }
            else //大型网段
            {
                (string baseSubnet, ObservableCollection<string> subnetsRanges) =
                    SubnetCalculator.CalculateSubnets(item.Network, netmask);

                int subIndex = 0;

                foreach (string range in subnetsRanges)
                {


                    subIndex++;

                    var name = $"Net_{item.NetworkId}_Sub{subIndex - 1}";

                    //网段信息
                    var info = new ExportNetworkInfoClass();

                    info.NetworkId = item.NetworkId;
                    info.SubId = $"_Sub{subIndex - 1}";
                    info.Network = item.Network;
                    info.Netmask = item.Netmask;
                    info.TableName = name;
                    info.Range = range;


                    exportNetworkInfos.Add(info);

                }

            }





        }


        return exportNetworkInfos;
    }

    
    /// <summary>
    /// 通过表名获取网段地址
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static string GetNetWorkSegment(string tableName)
    {
        //第一步，取出网段ID
        var networkId = ExtractTenCharCode(tableName);

        //第二步，取出网段地址和子网掩码
        var info = GetNetworkInfo(networkId);

        //第三步，根据子网掩码判断网络大小
        int maskLength = SubnetCalculator.SubnetMaskToLength(info.Netmask);

        string prefix ;

        if (maskLength >= 24)//小型网段
        {
            prefix = info.Network.Substring(0, info.Network.LastIndexOf('.') + 1);
        }
        else//大型网段
        {
            //获取子网段索引
            var subIndex = ExtractSubNumber(tableName);

            (string baseSubnet, ObservableCollection<string> subnetsRanges) =SubnetCalculator.CalculateSubnets(info.Network, maskLength);


            
            var subnetwork = subnetsRanges[subIndex];

            string[] parts = subnetwork.Split('.');
            string result = string.Join(".", parts.Take(3));

            prefix = result + ".";

        }


        return prefix;
    }


    /// <summary>
    /// 从表名提取网段ID，从输入字符串中提取第一个由10个连续的大写字母和数字组成的子串。
    /// </summary>
    /// <param name="input">要搜索的字符串</param>
    /// <returns>匹配的10位字符串，未找到时返回 null</returns>
    public static string ExtractTenCharCode(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 10)
            return null;

        // 正则表达式：匹配10个连续的字母（A-Z）或数字（0-9）
        string pattern = @"[A-Z0-9]{10}";
        Match match = Regex.Match(input, pattern);

        return match.Success ? match.Value : null;
    }


    /// <summary>
    /// 从表名提取子网段索引号，输入字符串中提取 "SubX" 中的数字 X。
    /// 例如：Net_7BD947C1CN_Sub0 → 返回 0；My_Sub123 → 返回 123。
    /// </summary>
    /// <param name="tableName">要搜索的字符串</param>
    /// <returns>找到的数字，未找到时返回 null</returns>
    public static int ExtractSubNumber(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return 0;

        // 正则表达式：匹配 "Sub" 后面跟着一个或多个数字
        Match match = Regex.Match(tableName, @"Sub(\d+)");

        if (match.Success)
        {
            // 提取括号中捕获的数字部分
            string numberStr = match.Groups[1].Value;
            if (int.TryParse(numberStr, out int number))
            {
                return number;
            }
        }

        return 0; // 未匹配到
    }



    /// <summary>
    /// 获取网络基础信息
    /// </summary>
    /// <param name="networkId"></param>
    /// <returns></returns>
    private static TrimNetworkInfoClass GetNetworkInfo(string networkId)
    {
        var sql = $"SELECT * FROM Network WHERE NetworkId = '{networkId}'";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);
        
        var info = new TrimNetworkInfoClass();

        foreach (var row in rows)
        {
            info.NetworkId= networkId;
            info.Network= row["Network"].ToString();
            info.Netmask = row["Netmask"].ToString();
        }

        return info;
    }





}
