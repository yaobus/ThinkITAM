using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Index;

namespace ThinkITAM.FunctionPage;

/// <summary>
/// Dashboard 仪表盘页面
/// 展示网络/设备/链路/资产四大统计卡片 + 置顶书签
/// 从 WPF 原项目 Dashboard.xaml.cs 迁移
/// </summary>
public partial class Dashboard : UserControl
{
    public Dashboard()
    {
        InitializeComponent();

        IndexTagsPanel.ItemsSource = dashboardIndexTags;
    }

    private async void Dashboard_OnLoaded(object? sender, RoutedEventArgs e)
    {
        await LoadStatisticsInfo();
        await LoadIndexTags();
    }

    private ViewModels.Dashboard.DashboardViewModel dashboard = new ViewModels.Dashboard.DashboardViewModel();

    /// <summary>
    /// 加载统计信息
    /// </summary>
    private async Task LoadStatisticsInfo()
    {
        // 如果数据库服务尚未初始化，跳过加载
        if (GlobalVariables.DbService == null) return;

        await Task.Run(() =>
        {
            try
            {
                dashboard.NetworkCount = StatisticsClass.StatisticsNetworkCount();
                dashboard.IpAddressCount = StatisticsClass.StatisticsIpAddressCount();
                dashboard.AvailableAddressCount = dashboard.IpAddressCount - StatisticsClass.StatisticsAvailableAddressCount();

                dashboard.DeviceCount = StatisticsClass.StatisticsDevicesCount();
                dashboard.ComputerCount = StatisticsClass.StatisticsComputerCount();
                dashboard.AvailableDevicePortCount = StatisticsClass.StatisticAvailableDevicePortCount();

                dashboard.LinkCount = StatisticsClass.StatisticsLinkCount();
                dashboard.LinkNodeCount = StatisticsClass.StatisticsNodeCount();
                dashboard.RackUnusedPortCount = StatisticsClass.StatisticAvailableRackPortCount();

                dashboard.AssetTypeCount = StatisticsClass.StatisticAssetTypeCount();
                dashboard.AssetCount = StatisticsClass.StatisticAssetCount();
                dashboard.AssetUnDeploy = StatisticsClass.StatisticsAssetUnDeploy();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dashboard] 加载统计数据失败: {ex.Message}");
            }
        });

        this.DataContext = dashboard;
    }

    private ObservableCollection<DashboardIndexTagViewModel> dashboardIndexTags =
        new ObservableCollection<DashboardIndexTagViewModel>();

    /// <summary>
    /// 加载置顶书签分组信息
    /// </summary>
    private async Task LoadIndexTags(string searchKeyWord = "")
    {
        if (GlobalVariables.DbService == null) return;

        try
        {
            var sql0 = "SELECT * FROM BookmarkGroupOrder WHERE Del != 1 OR Del IS NULL ORDER BY DisplayOrder ASC";
            var rows0 = GlobalVariables.DbService.ExecuteQuery(sql0);

            foreach (var row0 in rows0)
            {
                var info = new DashboardIndexTagViewModel();
                info.GroupName = row0["TypeGroup"].ToString()!;

                var sql = $"SELECT * FROM Bookmark WHERE TypeGroup = '{info.GroupName}' AND PinToStart = 1 AND (Del != 1 OR Del IS NULL);";
                var tagRows = GlobalVariables.DbService.ExecuteQuery(sql);

                var tags = new ObservableCollection<IndexTagViewModel>();

                foreach (var tagRow in tagRows)
                {
                    var tagInfo = new IndexTagViewModel();
                    tagInfo.IndexId = tagRow["IndexId"].ToString()!;
                    tagInfo.Group = info.GroupName;
                    tagInfo.Name = tagRow["Name"].ToString()!;
                    tagInfo.Protocol = tagRow["Protocol"].ToString()!;
                    tagInfo.Host = tagRow["Host"].ToString()!;
                    tagInfo.Port = tagRow["Port"].ToString()!;
                    tagInfo.Browser = tagRow["Browser"].ToString()!;

                    string url = $"{tagInfo.Protocol}{tagInfo.Host}";
                    tagInfo.Url = tagInfo.Port.Length == 0 ? url : $"{url}:{tagInfo.Port}";

                    try { tagInfo.PinToStart = Convert.ToInt32(tagRow["PinToStart"]); }
                    catch { tagInfo.PinToStart = 0; }

                    try { tagInfo.Color = Convert.ToInt32(tagRow["Color"]); }
                    catch { tagInfo.Color = 0; }

                    tags.Add(tagInfo);
                }

                info.IndexTags = tags;
                if (info.IndexTags.Count > 0)
                {
                    dashboardIndexTags.Add(info);
                }

                await Task.Delay(10);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dashboard] 加载书签失败: {ex.Message}");
        }
    }
}
