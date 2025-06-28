using System.Net;
using System.Net.Sockets;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.Functions.FunctionClass
{
    /// <summary>
    /// 指定IP地址的端口状态检查
    /// </summary>
    public class PortScanner
    {
        #region BACKUP

        //public static async Task<List<PortCheckResult>> CheckPortsAsync(string host, List<int> ports, int timeoutMilliseconds = 1000)
        //{
        //    var tasks = new List<Task<PortCheckResult>>();
        //    foreach (var port in ports)
        //    {
        //        tasks.Add(Task.Run(() => CheckPortAsync(host, port, timeoutMilliseconds)));
        //    }

        //    await Task.WhenAll(tasks);

        //    return new List<PortCheckResult>(tasks.Select(task => task.Result));
        //}

        //public static async Task<List<PortCheckResult>> CheckPortsAsync(List<string> hosts, List<int> ports, int timeoutMilliseconds = 1000)
        //{
        //    var tasks = new List<Task<PortCheckResult>>();
        //    foreach (var host in hosts)
        //    {
        //        foreach (var port in ports)
        //        {
        //            tasks.Add(Task.Run(() => CheckPortAsync(host, port, timeoutMilliseconds)));
        //        }
        //    }

        //    await Task.WhenAll(tasks);

        //    //return tasks.Select(task => task.Result).ToList();

        //    // 对结果按照Host（IP或域名）进行排序
        //    return tasks.Select(task => task.Result)
        //        .OrderBy(result => result.Host)
        //        .ToList();
        //}

        #endregion

        /// <summary>
        /// 主机端口检测
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="ports"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        public static async Task<List<PortCheckResult>> CheckPortsAsync(List<string> hosts, List<int> ports, int timeoutMilliseconds = 1000)
        {
            var tasks = new List<Task<PortCheckResult>>();
            foreach (var host in hosts)
            {
                foreach (var port in ports)
                {
                    tasks.Add(Task.Run(() => CheckPortAsync(host, port, timeoutMilliseconds)));
                }
            }

            await Task.WhenAll(tasks);

            // 对结果按照Host（IP）进行排序，考虑IP可能是域名的情况
            // 对结果按照Host（IP）进行排序
            return tasks.Select(task => task.Result)
                .OrderBy(result =>
                {
                    if (IPAddress.TryParse(result.Host, out IPAddress ipAddress))
                    {
                        return ipAddress.GetAddressBytes();
                    }
                    else
                    {
                        // 如果不是有效的IP地址，则返回一个特殊的byte数组，以保证它们被排到最后
                        return new byte[] { byte.MaxValue };
                    }
                }, new ByteArrayComparer())
                .ThenBy(result => result.Host) // 确保非IP地址（如域名）之间也能有序
                .ToList();
        }

        /// <summary>
        /// 主机端口检测，带进度报告
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="ports"></param>
        /// <param name="progress"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        public static async Task<List<PortCheckResult>> CheckPortsAsync(List<string> hosts, List<int> ports, IProgress<int> progress, int timeoutMilliseconds = 1000)
        {
            var tasks = new List<Task<PortCheckResult>>();
            int totalTasks = hosts.Count * ports.Count;
            int completedTasks = 0;

            // 创建任务
            foreach (var host in hosts)
            {
                foreach (var port in ports)
                {
                    tasks.Add(Task.Run(() => CheckPortAsync(host, port, timeoutMilliseconds)));
                }
            }

            // 准备存储结果的列表
            var results = new List<PortCheckResult>();

            // 对于每一个完成的任务，添加到结果列表并更新进度
            foreach (var task in tasks)
            {
                results.Add(await task);
                completedTasks++;
                // 计算并报告进度百分比
                int progressPercentage = (completedTasks * 100) / totalTasks;
                progress.Report(progressPercentage);
            }

            // 对结果进行排序
            return results.OrderBy(result =>
                {
                    if (IPAddress.TryParse(result.Host, out IPAddress ipAddress))
                    {
                        return ipAddress.GetAddressBytes();
                    }
                    else
                    {
                        // 如果不是有效的IP地址，则返回一个特殊的byte数组，以保证它们被排到最后
                        return new byte[] { byte.MaxValue };
                    }
                }, new ByteArrayComparer())
                .ThenBy(result => result.Host) // 确保非IP地址（如域名）之间也能有序
                .ToList();
        }




        /// <summary>
        /// 单个主机单个端口测试
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="timeoutMilliseconds">超时设置，毫秒</param>
        /// <returns></returns>
        private static async Task<PortCheckResult> CheckPortAsync(string host, int port, int timeoutMilliseconds)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMilliseconds));

                    if (!success)
                    {

                        return new PortCheckResult { Host = host, Port = port, IsOpen = false };

                    }

                    tcpClient.EndConnect(result); // 确保调用 EndConnect 来关闭连接尝试



                    return new PortCheckResult { Host = host, Port = port, IsOpen = true };

                }
            }
            catch
            {

                return new PortCheckResult { Host = host, Port = port, IsOpen = false };
            }
        }





        /// <summary>
        /// 自定义比较器用于比较byte数组
        /// </summary>
        private class ByteArrayComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int length = Math.Min(x.Length, y.Length);
                for (int i = 0; i < length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return x[i].CompareTo(y[i]);
                    }
                }

                return x.Length.CompareTo(y.Length);
            }
        }
    }

}
