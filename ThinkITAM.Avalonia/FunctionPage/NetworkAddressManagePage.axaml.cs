using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ThinkITAM.DataBridge;
using ThinkITAM.Database;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.FunctionPage;

public partial class NetworkAddressManagePage : UserControl
{
    private ObservableCollection<NetworkInfoViewMode> networkInfos = new();
    private ObservableCollection<IpAddressInfoListViewMode> ipAddressInfos = new();

    public NetworkAddressManagePage()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// 页面加载完成时加载网段列表
    /// </summary>
    private void Page_Loaded(object? sender, RoutedEventArgs e)
    {
        NetworkListView.ItemsSource = networkInfos;
        AddressDataGrid.ItemsSource = ipAddressInfos;
        LoadNetworkInfo();
        LoadShowModes();
    }

    /// <summary>
    /// 加载显示模式下拉框
    /// </summary>
    private void LoadShowModes()
    {
        ShowModeComboBox.ItemsSource = new[] { "全部显示", "只看空地址", "只看已用地址" };
        ShowModeComboBox.SelectedIndex = 0;
        ShowModeComboBox.SelectionChanged += ShowModeComboBox_OnSelectionChanged;
    }

    private void ShowModeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (NetworkListView.SelectedIndex >= 0)
            LoadIpAddressList();
    }

    /// <summary>
    /// 加载网段信息列表
    /// </summary>
    private void LoadNetworkInfo(string? keyword = null)
    {
        networkInfos.Clear();

        string sql = "SELECT * FROM Network WHERE (Del != 1 OR Del IS NULL)";

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += $" AND (Name LIKE '%{keyword}%' OR Network LIKE '%{keyword}%' OR Description LIKE '%{keyword}%' OR TagA LIKE '%{keyword}%' OR TagB LIKE '%{keyword}%' OR TagC LIKE '%{keyword}%' OR TagD LIKE '%{keyword}%' OR TagE LIKE '%{keyword}%' OR TagF LIKE '%{keyword}%')";
        }

        sql += " ORDER BY SortIndex DESC;";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);

        int index = 0;
        foreach (var row in rows)
        {
            index++;
            var info = new NetworkInfoViewMode
            {
                Index = index,
                NetworkId = row["NetworkId"].ToString(),
                TableName = "Net_" + row["NetworkId"],
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString(),
                Network = row["Network"].ToString(),
                Netmask = row["Netmask"].ToString(),
                Parent = row["Parent"]?.ToString(),
                Child = row["Child"]?.ToString(),
                TagA = row["TagA"]?.ToString(),
                TagB = row["TagB"]?.ToString(),
                TagC = row["TagC"]?.ToString(),
                TagD = row["TagD"]?.ToString(),
                TagE = row["TagE"]?.ToString(),
                TagF = row["TagF"]?.ToString(),
                AddressCount = "0/0/0"
            };

            try
            {
                info.Percentage = CalculateUsage(info.TableName, info.Netmask, info.Network);
                info.AddressCount = GetAddressCountStr(info.TableName, info.Netmask, info.Network);
            }
            catch { }

            networkInfos.Add(info);
        }

        if (networkInfos.Count > 0)
        {
            // 自动加载 IP 地址列表（同步到 DataBridge）
            DataBridge.DataBridge.IpAddressInfoLists.Clear();
        }
    }

    /// <summary>
    /// 计算网段使用率
    /// </summary>
    private double CalculateUsage(string tableName, string? netmask, string? network)
    {
        if (string.IsNullOrWhiteSpace(netmask)) return 0;

        int maskLength = SubnetMaskToCidr(netmask);
        double total = Math.Pow(2, 32 - maskLength);
        if (total <= 0) return 0;

        int used = GetUsedCount(tableName);
        return Math.Round(used * 100.0 / total, 1);
    }

    private string GetAddressCountStr(string tableName, string? netmask, string? network)
    {
        if (string.IsNullOrWhiteSpace(netmask)) return "0/0/0";

        int maskLength = SubnetMaskToCidr(netmask);
        double total = Math.Pow(2, 32 - maskLength);
        int used = GetUsedCount(tableName);
        return $"{used}/{total - used}/{total}";
    }

    private int GetUsedCount(string tableName)
    {
        string sql = $"SELECT COUNT(*) FROM {tableName} WHERE AddressStatus NOT IN (0, 1, 4)";
        return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
    }

    private int SubnetMaskToCidr(string subnetMask)
    {
        string[] parts = subnetMask.Split('.');
        if (parts.Length != 4) return 24;

        int cidr = 0;
        foreach (string part in parts)
        {
            if (int.TryParse(part, out int value))
            {
                cidr += CountBits(value);
            }
        }
        return cidr;
    }

    private int CountBits(int n)
    {
        int count = 0;
        while (n > 0)
        {
            count += n & 1;
            n >>= 1;
        }
        return count;
    }

    /// <summary>
    /// 网段列表选中改变时加载IP地址
    /// </summary>
    private void NetworkListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (NetworkListView.SelectedIndex < 0) return;

        DeleteButton.IsEnabled = true;

        var info = NetworkListView.SelectedItem as NetworkInfoViewMode;
        if (info == null) return;

        // 设置全局状态
        DataBridge.DataBridge.NetworkTableName = info.TableName ?? "";
        DataBridge.DataBridge.SelectNetworkInfo = info;
        DataBridge.DataBridge.SelectNetwork = info.Network ?? "";
        DataBridge.DataBridge.LoadedNetworkSegment = info.TableName ?? "";

        LoadIpAddressList();
    }

    /// <summary>
    /// 加载IP地址列表到DataGrid
    /// </summary>
    private void LoadIpAddressList()
    {
        ipAddressInfos.Clear();

        var info = NetworkListView.SelectedItem as NetworkInfoViewMode;
        if (info == null || string.IsNullOrWhiteSpace(info.TableName)) return;

        string tableName = info.TableName;

        // 检查表是否存在
        string checkSql = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
        if (Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(checkSql)) == 0)
            return;

        string mode = ShowModeComboBox.SelectedIndex switch
        {
            1 => "WHERE AddressStatus = 0",
            2 => "WHERE AddressStatus > 1",
            _ => ""
        };

        string sql = $"SELECT * FROM {tableName} {mode} ORDER BY Address ASC";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);

        int index = 0;
        foreach (var row in rows)
        {
            index++;
            var ipInfo = new IpAddressInfoListViewMode
            {
                Index = index,
                Address = Convert.ToInt32(row["Address"]),
                FullAddress = TryGetString(row, "FullAddress"),
                AddressStatus = Convert.ToInt32(row["AddressStatus"]),
                AddressColor = Convert.ToInt32(row["AddressColor"]),
                User = TryGetString(row, "User"),
                HostName = TryGetString(row, "HostName"),
                MacAddress = TryGetString(row, "MacAddress"),
                LinkDevice = TryGetString(row, "LinkDevice"),
                TagA = TryGetString(row, "TagA"),
                TagB = TryGetString(row, "TagB"),
                TagC = TryGetString(row, "TagC"),
                TagD = TryGetString(row, "TagD"),
                TagE = TryGetString(row, "TagE"),
                TagF = TryGetString(row, "TagF")
            };

            ipAddressInfos.Add(ipInfo);
        }
    }

    private string TryGetString(Dictionary<string, object> row, string key)
    {
        if (row.TryGetValue(key, out var val) && val != null && !(val is DBNull))
            return val.ToString() ?? "";
        return "";
    }

    /// <summary>
    /// 清除搜索关键词
    /// </summary>
    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchKeyWord.Text = "";
        LoadNetworkInfo();
    }

    /// <summary>
    /// 搜索网段
    /// </summary>
    private void SearchButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadNetworkInfo(SearchKeyWord.Text);
    }

    /// <summary>
    /// 新增网段
    /// </summary>
    private void AddNetworkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // 打开新增网段窗口
        OpenAddNetworkWindow();
    }

    /// <summary>
    /// 编辑网段
    /// </summary>
    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NetworkListView.SelectedIndex < 0) return;
        var info = NetworkListView.SelectedItem as NetworkInfoViewMode;
        if (info == null) return;

        // TODO: 打开编辑窗口
    }

    /// <summary>
    /// 删除网段
    /// </summary>
    private async void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NetworkListView.SelectedIndex < 0) return;
        var info = NetworkListView.SelectedItem as NetworkInfoViewMode;
        if (info == null) return;

        var result = await Services.DialogService.ShowConfirm(
            $"确定要删除网段「{info.Name}」吗？\n该操作将标记删除网段及其IP地址数据。",
            "确认删除");

        if (result)
        {
            string sql = $"UPDATE Network SET Del=1 WHERE NetworkId='{info.NetworkId}'";
            GlobalVariables.DbService.ExecuteNonQuery(sql);
            LoadNetworkInfo();
        }
    }

    /// <summary>
    /// 新增IP地址
    /// </summary>
    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: 打开 IP 地址分配窗口
    }

    /// <summary>
    /// 实际打开新增网段窗口的实现
    /// 在后续步骤中将被替换为完整窗口
    /// </summary>
    private async void OpenAddNetworkWindow()
    {
        // 简化版：直接在当前页面弹出输入创建
        // 完整功能将在子窗口实现
        var mainWindow = TopLevel.GetTopLevel(this) as Window;
        if (mainWindow == null) return;
        // 委托到下一阶段的 AddNetworkWindow
        // 目前先用简单对话框提示
        await Services.DialogService.ShowInfo("新增网段功能将在子窗口中实现", "提示");
    }
}
