using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ThinkITAM.Functions.Converters;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Functions.FunctionClass
{
    public class PingTesterClass
    {

        public static async Task PingAddressesAsync(ObservableCollection<IpAddressInfoListViewMode> addressInfos)
        {
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(256); // Limit to 256 concurrent tasks

            foreach (var info in addressInfos)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var ping = new Ping())
                        {
                            string ip = DataBridge.DataBridge.SelectNetwork + info.Address;
                            var reply = await ping.SendPingAsync(ip, 1000);

                            if (reply.Status == IPStatus.Success)//在线
                            {
                                info.PingTime = reply.RoundtripTime.ToString() + "ms";

                                // 使用Dispatcher.Invoke更新UI线程
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    //info.PingStatusColor = ColorConverterClass.ConvertColorCodeToBrush("#00d1ff");
                                });
                            }
                            else//不在线
                            {
                                info.PingTime = "-1";

                                // 使用Dispatcher.Invoke更新UI线程
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                   info.PingStatusColor = ColorConverterClass.ColorToBrush("#323232");
                                });
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

   


        /// <summary>
        /// Ping测试传入的IP地址数组，返回能ping通的地址数组
        /// </summary>
        /// <param name="ipAddresses"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetPingableIpsAsync(List<string> ipAddresses)
        {
            var pingableIps = new List<string>();
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(256); // Limit to 256 concurrent tasks

            foreach (var ipAddress in ipAddresses)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var reply = await ping.SendPingAsync(ipAddress, 1000);

                            if (reply.Status == IPStatus.Success) // 在线
                            {
                                lock (pingableIps) // 确保线程安全
                                {
                                    pingableIps.Add(ipAddress);
                                }
                            }
                        }
                    }
                    catch (PingException)
                    {
                        // 忽略Ping失败的情况
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return pingableIps;
        }
    }

}



