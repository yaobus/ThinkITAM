using System.Net;

namespace ThinkITAM.Functions.IPAddressHelper
{
    public static class IPAddressCalculations
    {

        public static long AddressCount(int maskLength)
        {
            return (long)Math.Pow(2, 32 - maskLength);
        }

        public static IPAddress SubnetMaskFromPrefixLength(int prefixLength)
        {
            uint subnet = 0xffffffff;
            subnet <<= (32 - prefixLength);
            byte[] bytes = BitConverter.GetBytes(subnet);
            Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipBytes = address.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            byte[] result = new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                result[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(result);
        }

        public static IPAddress GetFirstUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily)
        {
            byte[] bytes = networkAddress.GetAddressBytes();
            bytes[bytes.Length - 1] += 1; // Increment last byte
            return new IPAddress(bytes);
        }

        public static IPAddress GetLastUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily, int maskLength)
        {
            byte[] bytes = networkAddress.GetAddressBytes();
            int usableAddresses = (int)Math.Pow(2, 32 - maskLength) - 2; // Calculate the number of usable addresses
            int lastByteIndex = bytes.Length - 1;
            int carry = usableAddresses / 256;
            bytes[lastByteIndex] += (byte)(usableAddresses % 256); // Add remainder to last byte
            for (int i = lastByteIndex - 1; i >= 0 && carry > 0; i--)
            {
                int sum = bytes[i] + carry;
                bytes[i] = (byte)(sum % 256);
                carry = sum / 256;
            }
            return new IPAddress(bytes);
        }


        public static IPAddress GetBroadcastAddress(this IPAddress networkAddress, int maskLength)
        {
            byte[] bytes = networkAddress.GetAddressBytes();
            int lastByteIndex = bytes.Length - 1;
            int subnetBits = 32 - maskLength;

            for (int i = 0; i < subnetBits; i++)
            {
                int byteIndex = i / 8;
                int bitOffset = i % 8;
                bytes[lastByteIndex - byteIndex] |= (byte)(1 << bitOffset);
            }

            return new IPAddress(bytes);
        }

        /// <summary>
        /// 输入IP地址类型的子网掩码，将子网掩码转换为CIDR表示法
        /// </summary>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static int CalculateSubnetMaskLength(IPAddress subnetMask)
        {
            byte[] bytes = subnetMask.GetAddressBytes();
            uint mask = BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0);
            int maskLength = 0;
            while (mask != 0)
            {
                maskLength++;
                mask <<= 1;
            }
            return maskLength;
        }



        /// <summary>
        /// 子网掩码计算器
        /// </summary>
        /// <param name="radix">需要返回的子网掩码进制整数，如2,10,16</param>
        /// <param name="maskBits">子网掩码位数，如24</param>
        /// <returns></returns>
        public static string GetSubnetMask(int radix, int maskBits)
        {

            // 计算子网掩码的二进制表示形式
            string binaryMask = new string('1', maskBits) + new string('0', 32 - maskBits);

            // 将二进制字符串分成四个8位部分，并转换为相应的格式
            string result = "";
            for (int i = 0; i < 4; i++)
            {
                string part = binaryMask.Substring(i * 8, 8);
                int decimalValue = Convert.ToInt32(part, 2);

                switch (radix)
                {
                    case 2:
                        result += (string.IsNullOrEmpty(result) ? "" : ".") + part;
                        break;
                    case 10:
                        result += (string.IsNullOrEmpty(result) ? "" : ".") + decimalValue.ToString();
                        break;
                    case 16:
                        result += (string.IsNullOrEmpty(result) ? "" : ".") + decimalValue.ToString("X2");
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取可用地址数
        /// </summary>
        /// <param name="maskBits"></param>
        /// <returns></returns>
        public static long GetAvailableAddresses(int maskBits)
        {

            // 计算主机部分的位数
            int hostBits = 32 - maskBits;

            // 计算可用的IP地址总数
            // 使用 1L << hostBits 来确保计算时使用长整型避免溢出
            long totalAddresses = (1L << hostBits) - 2;

            // 如果hostBits为32，即没有网络位，只有主机位，则totalAddresses会变成-2，这种情况应该返回0
            // 如果hostBits为31，即只有一个网络位，这样的划分方式实际上无法提供可用的主机地址，也应返回0
            return totalAddresses > 0 ? totalAddresses : 0;
        }

        /// <summary>
        /// 根据用户需要的地址数量，计算出最小的子网掩码位数
        /// </summary>
        /// <param name="requiredAddresses"></param>
        /// <returns></returns>
        public static int GetMinimumSubnetMaskBits(int requiredAddresses)
        {
            // 如果需要的地址数量小于2，则默认返回30，因为最小的可分配地址数为2（排除网络和广播地址）
            if (requiredAddresses < 2) return 30;

            // 加2是因为要排除网络地址和广播地址
            int neededTotalAddresses = requiredAddresses + 2;

            // 初始化掩码位数
            int maskBits = 32;

            // 计算满足所需地址数量的最小子网掩码位数
            for (int hostBits = 0; hostBits <= 32; hostBits++)
            {
                long totalAddresses = (1L << hostBits);
                if (totalAddresses >= neededTotalAddresses)
                {
                    maskBits = 32 - hostBits;
                    break;
                }
            }

            return maskBits;
        }


        /// <summary>
        /// 输入字符串类型的子网掩码，将子网掩码转换为CIDR表示法
        /// </summary>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static int SubnetMaskToCidr(string subnetMask)
        {
            // 将子网掩码转换为32位的二进制字符串
            string binaryStr = string.Join("", Array.ConvertAll(subnetMask.Split('.'), octet =>
                Convert.ToString(int.Parse(octet), 2).PadLeft(8, '0')));

            // 计算前导1的数量，这就是CIDR表示法中的网络位长度
            int cidr = binaryStr.IndexOf('0') == -1 ? binaryStr.Length : binaryStr.IndexOf('0');

            return cidr;
        }



        /// <summary>
        /// 判断是否是合法IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsValidIp(string ipAddress)
        {
            IPAddress address;
            return IPAddress.TryParse(ipAddress, out address);
        }
    }
}
