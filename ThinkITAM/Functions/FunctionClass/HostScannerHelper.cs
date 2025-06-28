using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows.Threading;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.FunctionClass;

public class HostScannerHelper
{

    private readonly ObservableCollection<HostCheckResult> _hostResults;
    private int _totalChecks;
    private int _completedChecks;
    private Action<int, int> _progressUpdateCallback;
    private Dispatcher _dispatcher;
    private VendorInfoFetcher _vendorInfoFetcher = new VendorInfoFetcher();

    public HostScannerHelper(ObservableCollection<HostCheckResult> hostResults, Action<int, int> progressUpdateCallback, Dispatcher dispatcher)
    {
        _hostResults = hostResults;
        _progressUpdateCallback = progressUpdateCallback;
        _dispatcher = dispatcher;
    }

    public async Task CheckPortsAsync(List<string> hosts, List<int> ports, bool scanMacAndHostName, bool scanPorts, int timeoutMilliseconds = 1000)
    {

        int index = 0;
        _totalChecks = (scanMacAndHostName ? hosts.Count : 0) + (scanPorts ? hosts.Count * ports.Count : 0);
        _completedChecks = 0;

        foreach (var host in hosts)
        {
            var result = new HostCheckResult
            {
                Index = ++index,
                Host = host
            };

            if (scanMacAndHostName)
            {
                try
                {
                    var hostNameTask = DeviceInfoUpdater.GetHostNameFromIpAsync(host);
                    var macAddressTask = DeviceInfoUpdater.GetMacAddress(host);

                    var completedHostNameTask = await Task.WhenAny(hostNameTask, Task.Delay(timeoutMilliseconds));
                    var completedMacAddressTask = await Task.WhenAny(macAddressTask, Task.Delay(timeoutMilliseconds));

                    if (completedHostNameTask == hostNameTask)
                    {
                        result.HostName = await hostNameTask;
                        // Console.WriteLine(result.HostName);
                    }
                    else
                    {
                        result.HostName = "N/A";
                    }

                    if (completedMacAddressTask == macAddressTask)
                    {
                        result.Mac = await macAddressTask;

                        string vendorInfo = await _vendorInfoFetcher.GetVendorInfo(result.Mac);

                        result.Vendor = vendorInfo;


                    }
                    else
                    {
                        result.Mac = "N/A";
                    }
                }
                catch (Exception ex)
                {
                    result.HostName = "N/A";
                    result.Mac = "N/A";
                }
                _completedChecks++;
                UpdateProgress(_completedChecks, _totalChecks);
            }

            if (scanPorts)
            {
                var openPorts = new List<int>();

                foreach (var port in ports)
                {
                    if (await IsPortOpenAsync(host, port, timeoutMilliseconds))
                    {
                        openPorts.Add(port);
                    }
                    _completedChecks++;
                    UpdateProgress(_completedChecks, _totalChecks);
                }

                result.OpenedPorts = string.Join(",", openPorts);
            }

            // Ensure the collection update is on the UI thread
            _dispatcher.Invoke(() => _hostResults.Add(result));
        }
    }

    private async Task<bool> IsPortOpenAsync(string host, int port, int timeoutMilliseconds)
    {
        using (var client = new TcpClient())
        {
            var connectTask = client.ConnectAsync(host, port);
            var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds));

            if (completedTask != connectTask)
            {
                return false; // Timeout occurred
            }

            await connectTask; // Ensure any exceptions are thrown

            return client.Connected;
        }
    }

    private void UpdateProgress(int current, int total)
    {
        _progressUpdateCallback(current, total);
    }

}





