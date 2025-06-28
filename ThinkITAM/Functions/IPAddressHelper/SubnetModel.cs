using System.Collections.ObjectModel;
using System.Net;


namespace ThinkITAM.Functions.IPAddressHelper
{
    public class SubnetModel
    {
        public string BaseSubnet
        {
            get; set;
        }
        public ObservableCollection<string> SubnetRanges
        {
            get; set;
        }
    }

    public class SubnetCalculator
    {


        /// <summary>
        /// 返回网段地址和子网范围
        /// </summary>
        /// <param name="ipAddress">网段地址</param>
        /// <param name="subnetMask">子网掩码位数</param>
        /// <returns></returns>
        public static (string, ObservableCollection<string>) CalculateSubnets(string ipAddress, int subnetMask)
        {

            string baseSubnet = CalculateBaseSubnet(ipAddress, subnetMask);

            ObservableCollection<string> subnetsRanges = CalculateSubnetsRanges(ipAddress, subnetMask);

            return (baseSubnet, subnetsRanges);



        }

        /// <summary>
        /// 子网掩码转换为子网掩码长度
        /// </summary>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static int SubnetMaskToLength(string subnetMask)
        {
            // 将子网掩码字符串按点分割成四个部分
            string[] parts = subnetMask.Split('.');

            // 检查分割后的数组长度是否为4
            if (parts.Length != 4)
            {
                return -1;
            }

            int length = 0;

            // 遍历每个部分
            foreach (string part in parts)
            {
                // 将字符串部分转换为整数
                int octet = int.Parse(part);

                // 检查每个部分是否在0到255之间
                if (octet < 0 || octet > 255)
                {
                    return -1;
                }

                // 计算每个八位字节中1的个数并累加到长度中
                while (octet != 0)
                {
                    length += octet & 1;
                    octet >>= 1;
                }
            }

            return length;
        }


        /// <summary>
        /// 计算子网分段
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        private static string CalculateBaseSubnet(string ipAddress, int subnetMask)
        {
            string[] ipParts = ipAddress.Split('.');
            int[] ipIntParts = Array.ConvertAll(ipParts, int.Parse);

            // Calculate the base subnet
            ipIntParts[3] &= (0xFF << (32 - subnetMask));
            string baseSubnet = string.Join(".", ipIntParts);

            return baseSubnet + "/" + subnetMask;
        }

        private static ObservableCollection<string> CalculateSubnetsRanges(string ipAddress, int subnetMask)
        {
            ObservableCollection<string> subnetsRanges = new ObservableCollection<string>();

            string[] ipParts = ipAddress.Split('.');

            string baseIpAddress = ipParts[0] + "." + ipParts[1];

            int subnetCount = (int)IPAddressCalculations.AddressCount(subnetMask) / 256;

            int subnetIncrement = 256;

            for (int i = 0; i < subnetCount; i++)
            {
                int startAddress = i * subnetIncrement;
                int endAddress = (i + 1) * subnetIncrement - 1;

                int startOffset = startAddress % 256;
                int endOffset = endAddress % 256;


                IPAddress ip;

                if (IPAddress.TryParse(ipAddress, out ip))
                {
                    IPAddress networkAddress = ip.GetNetworkAddress(IPAddressCalculations.SubnetMaskFromPrefixLength(subnetMask));

                    string networkStr = networkAddress.ToString();

                    string[] baseSplit = networkStr.Split('.');

                    int part3 = Convert.ToInt32(baseSplit[2]);

                    string subnetRange = $"{baseIpAddress}.{part3 + i}.{startOffset}-{baseIpAddress}.{part3 + i}.{endOffset}";

                    subnetsRanges.Add(subnetRange);
                }






            }

            return subnetsRanges;
        }
    }
}