using SharedCalc = ThinkITAM.Shared.Network.IPAddressCalculations;
using System.Net;

namespace ThinkITAM.Functions.IPAddressHelper
{
    public static class IPAddressCalculations
    {
        public static long AddressCount(int maskLength)
            => SharedCalc.AddressCount(maskLength);

        public static IPAddress SubnetMaskFromPrefixLength(int prefixLength)
            => SharedCalc.SubnetMaskFromPrefixLength(prefixLength);

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
            => SharedCalc.GetNetworkAddress(address, subnetMask);

        public static IPAddress GetFirstUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily)
            => SharedCalc.GetFirstUsable(networkAddress, addressFamily);

        public static IPAddress GetLastUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily, int maskLength)
            => SharedCalc.GetLastUsable(networkAddress, addressFamily, maskLength);

        public static IPAddress GetBroadcastAddress(this IPAddress networkAddress, int maskLength)
            => SharedCalc.GetBroadcastAddress(networkAddress, maskLength);

        /// <summary>
        /// 输入IP地址类型的子网掩码，将子网掩码转换为CIDR表示法
        /// </summary>
        public static int CalculateSubnetMaskLength(IPAddress subnetMask)
            => SharedCalc.CalculateSubnetMaskLength(subnetMask);

        /// <summary>
        /// 子网掩码计算器
        /// </summary>
        public static string GetSubnetMask(int radix, int maskBits)
            => SharedCalc.GetSubnetMask(radix, maskBits);

        /// <summary>
        /// 获取可用地址数
        /// </summary>
        public static long GetAvailableAddresses(int maskBits)
            => SharedCalc.GetAvailableAddresses(maskBits);

        /// <summary>
        /// 根据用户需要的地址数量，计算出最小的子网掩码位数
        /// </summary>
        public static int GetMinimumSubnetMaskBits(int requiredAddresses)
            => SharedCalc.GetMinimumSubnetMaskBits(requiredAddresses);

        /// <summary>
        /// 输入字符串类型的子网掩码，将子网掩码转换为CIDR表示法
        /// </summary>
        public static int SubnetMaskToCidr(string subnetMask)
            => SharedCalc.SubnetMaskToCidr(subnetMask);

        /// <summary>
        /// 判断是否是合法IP
        /// </summary>
        public static bool IsValidIp(string ipAddress)
            => SharedCalc.IsValidIp(ipAddress);
    }
}
