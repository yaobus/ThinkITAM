using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Functions.FunctionClass;
public class DeviceInfoUpdater
{
    private const int TimeoutMilliseconds = 500; // 超时设置为0.5秒

    public static async Task UpdateDeviceInfoListAsync(ObservableCollection<IpAddressInfoListViewMode> devices)
    {
        var tasks = devices
            // .Where(device => device.PingTime != "-1")
            .Select(device => UpdateDeviceInfoAsync(device))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    private static async Task UpdateDeviceInfoAsync(IpAddressInfoListViewMode device)
    {
        string ip = DataBridge.DataBridge.SelectNetwork + device.Address;
        var hostNameTask = GetHostNameFromIpAsync(ip);
        var macAddressTask = GetMacAddress(ip);

        var completedHostNameTask = await Task.WhenAny(hostNameTask, Task.Delay(TimeoutMilliseconds));
        var completedMacAddressTask = await Task.WhenAny(macAddressTask, Task.Delay(TimeoutMilliseconds));

        if (completedHostNameTask == hostNameTask)
        {
            device.NowHostName = await hostNameTask;
        }
        else
        {
            device.NowHostName = "N/A";
        }

        if (completedMacAddressTask == macAddressTask)
        {
            device.NowMacAddress = await macAddressTask;
        }
        else
        {
            device.NowMacAddress = "N/A";
        }
    }

    //public static async Task<string> GetHostNameFromIpAsync(string ipAddress)
    //{
    //    return await Task.Run(() =>
    //    {
    //        try
    //        {
    //            IPHostEntry entry = Dns.GetHostEntry(ipAddress);
    //            return entry.HostName;
    //        }
    //        catch (Exception)
    //        {
    //            return "N/A";
    //        }
    //    });
    //}

    //public static async Task<string> GetMacAddress(string ipAddress)
    //{
    //    return await Task.Run(() =>
    //    {
    //        IPAddress ip = IPAddress.Parse(ipAddress);
    //        byte[] macAddressBytes = new byte[6];
    //        int macAddrLen = macAddressBytes.Length;
    //        uint macAddrLenUint = (uint)macAddrLen;

    //        int result = NativeMethods.SendARP((int)ip.Address, 0, macAddressBytes, ref macAddrLenUint);
    //        if (result != 0)
    //            return "N/A";

    //        string macAddress = BitConverter.ToString(macAddressBytes, 0, macAddrLen).Replace("-", ":");
    //        return macAddress;
    //    });
    //}



    public static async Task<string> GetHostNameFromIpAsync(string ipAddress)
    {
        try
        {
            // 使用异步方法避免阻塞UI线程
            var hostEntry = await Dns.GetHostEntryAsync(ipAddress).ConfigureAwait(false);
            return hostEntry.HostName;
        }
        catch (SocketException)
        {
            return "N/A";
        }
        catch (ArgumentException)
        {
            return "N/A";
        }
    }



    public static async Task<string> GetMacAddress(string ipAddress)
    {
        return await Task.Run(() =>
        {
            try
            {
                // 使用ARP表查询
                var ip = IPAddress.Parse(ipAddress);
                var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(nic => nic.GetIPProperties().UnicastAddresses)
                    .Where(addr => addr.Address.Equals(ip))
                    .Select(addr => addr.Address)
                    .FirstOrDefault();

                if (macAddress == null)
                {
                    // 如果直接查询失败，尝试发送ARP请求
                    var arp = new System.Diagnostics.Process();
                    arp.StartInfo.FileName = "arp";
                    arp.StartInfo.Arguments = $"-a {ipAddress}";
                    arp.StartInfo.UseShellExecute = false;
                    arp.StartInfo.RedirectStandardOutput = true;
                    arp.StartInfo.CreateNoWindow = true;
                    arp.Start();

                    var output = arp.StandardOutput.ReadToEnd();
                    arp.WaitForExit();

                    // 解析ARP输出
                    var lines = output.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.Contains(ipAddress))
                        {
                            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2)
                            {
                                return parts[1];
                            }
                        }
                    }

                    return "N/A";
                }

                var interfaceMac = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return interfaceMac ?? "N/A";
            }
            catch (Exception)
            {
                return "N/A";
            }
        }).ConfigureAwait(false);
    }






    internal static class NativeMethods
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        internal static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
    }

    private static string IntToIp(int ipInt)
    {
        return new IPAddress(BitConverter.GetBytes(ipInt).Reverse().ToArray()).ToString();
    }
}







