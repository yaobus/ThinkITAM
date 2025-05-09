using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    public static async Task<string> GetHostNameFromIpAsync(string ipAddress)
    {
        return await Task.Run(() =>
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                return entry.HostName;
            }
            catch (Exception)
            {
                return "N/A";
            }
        });
    }

    public static async Task<string> GetMacAddress(string ipAddress)
    {
        return await Task.Run(() =>
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            byte[] macAddressBytes = new byte[6];
            int macAddrLen = macAddressBytes.Length;
            uint macAddrLenUint = (uint)macAddrLen;

            int result = NativeMethods.SendARP((int)ip.Address, 0, macAddressBytes, ref macAddrLenUint);
            if (result != 0)
                return "N/A";

            string macAddress = BitConverter.ToString(macAddressBytes, 0, macAddrLen).Replace("-", ":");
            return macAddress;
        });
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







