using System.Net;
using System.Text.RegularExpressions;

namespace ThinkITAM.Functions.FunctionClass;
public class IPParserHelper
{

    public static List<string> ParseIPAddresses(string input)
    {
        var ipList = new HashSet<string>(); // 使用HashSet避免重复IP

        var newInput = input.Replace("，", ",");

        // 标准化输入：将逗号和空格替换为统一的分隔符
        string normalizedInput = Regex.Replace(newInput, @"\s*,\s*|\s+", ",");

        // 分割输入字符串得到各个元素
        var elements = normalizedInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var element in elements)
        {
            var trimmedElement = element.Trim();

            if (trimmedElement.Contains("/"))
            {
                // 处理CIDR表示法，例如 "192.168.1.0/24"
                var cidrIps = ParseCIDR(trimmedElement);



                foreach (var ip in cidrIps)
                {
                    ipList.Add(ip);
                }
            }
            else if (trimmedElement.Contains("-"))
            {
                // 处理IP范围，例如 "192.168.1.1-192.168.1.100"
                var rangeIps = ParseIPRange(trimmedElement);
                foreach (var ip in rangeIps)
                {
                    ipList.Add(ip);
                }
            }
            else if (IsValidIPAddress(trimmedElement))
            {
                // 单个IP地址
                ipList.Add(trimmedElement);
            }
            else
            {


                return null;

            }
        }

        return ipList.ToList(); // 返回排序后的列表
    }

    private static bool IsValidIPAddress(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }

    private static List<string> ParseCIDR(string cidr)
    {
        var result = new List<string>();
        try
        {
            var parts = cidr.Split('/');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int prefixLength) || prefixLength < 0 || prefixLength > 32)
            {
                throw new ArgumentException("无效的CIDR格式，请检查是否符合标准（如192.168.1.0/24）。");
            }

            var baseAddress = IPAddress.Parse(parts[0]);
            var baseBytes = BitConverter.ToUInt32(baseAddress.GetAddressBytes().Reverse().ToArray(), 0);
            var mask = ~(uint.MaxValue >> prefixLength);

            uint start = baseBytes & mask;
            uint end = start | ~mask;

            for (uint i = start; i <= end; i++)
            {
                var ipBytes = BitConverter.GetBytes(i).Reverse().ToArray();
                result.Add(new IPAddress(ipBytes).ToString());
            }
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"无法解析CIDR: {cidr}. 错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);

            PortScannerHelper.ShowMessageDialog("端口输入有误", $"无法解析CIDR: {cidr}. 错误: {ex.Message}");

        }

        return result;
    }

    private static List<string> ParseIPRange(string range)
    {
        var result = new List<string>();
        try
        {
            var parts = range.Split('-');
            if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out var startIp) || !IPAddress.TryParse(parts[1], out var endIp))
            {
                throw new ArgumentException("无效的IP范围格式，请确保格式正确（如192.168.1.1-192.168.1.100）。");
            }

            var startBytes = BitConverter.ToUInt32(startIp.GetAddressBytes().Reverse().ToArray(), 0);
            var endBytes = BitConverter.ToUInt32(endIp.GetAddressBytes().Reverse().ToArray(), 0);

            if (startBytes > endBytes)
            {
                throw new ArgumentException("起始IP必须小于或等于结束IP。");
            }

            for (uint i = startBytes; i <= endBytes; i++)
            {
                var ipBytes = BitConverter.GetBytes(i).Reverse().ToArray();
                result.Add(new IPAddress(ipBytes).ToString());
            }
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"无法解析IP范围: {range}. 错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            PortScannerHelper.ShowMessageDialog("地址输入有误", $"无法解析IP范围: {range}. 错误: {ex.Message}");


        }

        return result;
    }
}


