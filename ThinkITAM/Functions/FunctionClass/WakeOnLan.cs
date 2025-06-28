using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ThinkITAM.Functions.FunctionClass
{
    public class WakeOnLan
    {
        /// <summary>
        /// 异步唤醒指定 MAC 地址的主机
        /// </summary>
        /// <param name="macAddress">MAC 地址，格式如 "00:11:22:33:44:55"</param>
        /// <param name="port">WOL 端口，默认为 9</param>
        /// <returns></returns>
        public static async Task WakeUpAsync(string macAddress, int port = 9)
        {
            // 去除非十六进制字符并转换为字节数组
            string macCleaned = macAddress.Replace(":", "").Replace("-", "").ToUpper();
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                string hex = macCleaned.Substring(i * 2, 2);
                macBytes[i] = Convert.ToByte(hex, 16);
            }

            // 构建 Magic Packet（6个 FF + 16次重复的MAC地址）
            byte[] magicPacket = new byte[102];
            for (int i = 0; i < 6; i++) magicPacket[i] = 0xFF;
            for (int i = 1; i <= 16; i++)
            {
                Array.Copy(macBytes, 0, magicPacket, i * 6, 6);
            }

            // 使用 UDP 广播 Magic Packet
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Connect(IPAddress.Broadcast, port); // 发送到广播地址
                await udpClient.SendAsync(magicPacket, magicPacket.Length);
            }
        }



        /// <summary>
        /// 异步发送Magic Packet
        /// </summary>
        /// <param name="macAddress">目标MAC地址(格式如00-11-22-33-44-55或00:11:22:33:44:55)</param>
        /// <param name="ipAddress">目标IP地址</param>
        /// <param name="port">端口号(默认9)</param>
        /// <param name="sendToSpecificIp">true=发送到指定IP, false=发送到广播地址</param>
        /// <param name="subnetMask">可选子网掩码(如255.255.255.0)，为null时自动检测</param>
        /// <returns>Task表示异步操作</returns>
        public static async Task SendMagicPacketAsync(
            string macAddress,
            string ipAddress,
            int port = 9,
            bool sendToSpecificIp = true,
            string subnetMask = null)
        {
            // 验证参数
            if (string.IsNullOrWhiteSpace(macAddress))
                throw new ArgumentException("MAC地址不能为空");

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP地址不能为空");

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "端口号必须在1-65535之间");

            // 解析MAC地址
            byte[] macBytes;
            try
            {
                macBytes = PhysicalAddress.Parse(macAddress.Replace(':', '-')).GetAddressBytes();
            }
            catch (FormatException)
            {
                throw new ArgumentException("无效的MAC地址格式");
            }

            // 构建魔术包
            byte[] packet = new byte[102];

            // 前6个字节为0xFF
            for (int i = 0; i < 6; i++)
            {
                packet[i] = 0xFF;
            }

            // 重复16次MAC地址
            for (int i = 1; i <= 16; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    packet[i * 6 + j] = macBytes[j];
                }
            }

            // 确定目标地址
            IPAddress targetIp;
            if (sendToSpecificIp)
            {
                // 直接发送到指定IP
                if (!IPAddress.TryParse(ipAddress, out targetIp))
                {
                    throw new ArgumentException("无效的IP地址格式");
                }
            }
            else
            {
                // 发送到广播地址
                IPAddress mask = null;

                if (!string.IsNullOrWhiteSpace(subnetMask))
                {
                    if (!IPAddress.TryParse(subnetMask, out mask))
                    {
                        throw new ArgumentException("无效的子网掩码格式");
                    }
                }

                targetIp = GetBroadcastAddress(ipAddress, mask);

            }
            Console.WriteLine(targetIp);
            // 使用UDP异步发送
            using (UdpClient client = new UdpClient())
            {
                await client.SendAsync(packet, packet.Length, new IPEndPoint(targetIp, port));
            }
        }

        /// <summary>
        /// 根据IP地址和子网掩码获取广播地址
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <param name="subnetMask">子网掩码(可选)</param>
        private static IPAddress GetBroadcastAddress(string ipAddress, IPAddress subnetMask = null)
        {
            var ip = IPAddress.Parse(ipAddress);
            var mask = subnetMask ?? GetSubnetMask(ip);

            // 计算广播地址
            byte[] ipBytes = ip.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();

            if (ipBytes.Length != maskBytes.Length)
            {
                throw new ArgumentException("IP地址和子网掩码的地址族不匹配");
            }

            byte[] broadcastBytes = new byte[ipBytes.Length];

            for (int i = 0; i < broadcastBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            }

            return new IPAddress(broadcastBytes);
        }

        /// <summary>
        /// 获取子网掩码(自动检测)
        /// </summary>
        private static IPAddress GetSubnetMask(IPAddress ipAddress)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicast in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily == ipAddress.AddressFamily &&
                        ipAddress.Equals(unicast.Address))
                    {
                        return unicast.IPv4Mask;
                    }
                }
            }

            // 如果没有找到，使用默认掩码
            return ipAddress.AddressFamily == AddressFamily.InterNetworkV6
                ? IPAddress.Parse("ffff:ffff:ffff:ffff::")
                : IPAddress.Parse("255.255.255.0");
        }
    }
}



