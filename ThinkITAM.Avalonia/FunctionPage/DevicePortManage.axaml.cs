using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.Others;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;

namespace ThinkITAM.FunctionPage;

public partial class DevicePortManage : UserControl
{
    public DevicePortManage()
    {
        InitializeComponent();
        PortListView.ItemsSource = DataBridge.DataBridge.PortDetailedInfos;
    }

    private void DevicePortManage_OnLoaded(object? sender, RoutedEventArgs e)
    {
        LoadTags();
        DataBridge.DataBridge.PortSelectCount.CollectionChanged += PortSelectCount_CollectionChanged;
        LoadAssetTreeViewInfos();

        DataBridge.DataBridge.ChangedDevicePorts.CollectionChanged += ChangedDevicePorts_CollectionChanged;
    }

    private async void ChangedDevicePorts_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (DataBridge.DataBridge.SelectDeviceTableInfo != null)
        {
            await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);
            AnalysisPortInfos2();
        }
    }

    private void PortSelectCount_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (window == null) return;

        var addDevice = new Windows.DevicePortManage.DeviceCreateGuideWindow();
        addDevice.ShowDialog(window).ContinueWith(_ =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                LoadAssetTreeViewInfos();
            });
        });
    }

    /// <summary>
    /// 加载资产树形视图
    /// </summary>
    private async void LoadAssetTreeViewInfos(string? keyWord = null)
    {
        AssetTreeView.Items?.Clear();

        string filter = string.Empty;
        if (!string.IsNullOrWhiteSpace(keyWord))
        {
            filter = $"AND ( AssetNumber LIKE '%{keyWord}%' OR Model LIKE '%{keyWord}%' OR Description LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%')";
        }

        string sqlTemp = $"SELECT COUNT(*) FROM Devices WHERE (Del != 1 OR Del IS NULL) {filter}";
        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0) return;

        string query = $"SELECT DISTINCT AssetType FROM Devices WHERE (Del != 1 OR Del IS NULL) {filter};";
        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        int index = 0;
        foreach (var row in rows)
        {
            index++;
            string assetTypeInfo = row["AssetType"].ToString();

            sqlTemp = $"SELECT * FROM Devices WHERE AssetType = '{assetTypeInfo}' AND (Del != 1 OR Del IS NULL) {filter}";
            var rows2 = GlobalVariables.DbService.ExecuteQuery(sqlTemp);

            int index2 = 0;

            foreach (var row2 in rows2)
            {
                index2++;
                var deviceInfo = new DeviceTypeViewModel
                {
                    Index = index2,
                    DeviceType = row2["DeviceType"].ToString(),
                    AssetId = row2["AssetId"].ToString(),
                    AssetType = assetTypeInfo,
                    Description = row2["Description"].ToString(),
                    Model = row2["Model"].ToString(),
                    AssetNumber = row2["AssetNumber"].ToString(),
                    ToolTip = $"[{row2["Description"]}]-[{row2["Model"]}]-[{row2["AssetNumber"]}]"
                };

                // 创建 TreeViewItem 并设置 DataContext
                var deviceItem = new TreeViewItem();
                deviceItem.DataContext = deviceInfo;

                var headerPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"[{deviceInfo.DeviceType}] {deviceInfo.AssetNumber}",
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Avalonia.Thickness(5, 0)
                });
                deviceItem.Header = headerPanel;

                AssetTreeView.Items?.Add(deviceItem);
                await Task.Delay(10);
            }
        }
    }

    private async void AssetTreeView_OnSelectedItemChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (AssetTreeView.SelectedItem is TreeViewItem selectedItem)
        {
            var info = selectedItem.DataContext as DeviceTypeViewModel;
            if (info != null)
            {
                DataBridge.DataBridge.SelectDeviceTableInfo = info;
                LoadCustomTag();
                await LoadPortInfos(info);
                AnalysisPortInfos2();
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
        }
    }

    /// <summary>
    /// 加载端口信息
    /// </summary>
    private async Task LoadPortInfos(DeviceTypeViewModel tableInfo)
    {
        string tableName = $"De_{tableInfo.AssetId}";

        if (string.IsNullOrEmpty(tableName)) return;

        DataBridge.DataBridge.PortDetailedInfos.Clear();

        string query = $"SELECT * FROM {tableName};";
        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            var info = new PortDetailedInfo
            {
                UID = Convert.ToInt32(row["UID"]),
                PortType = row["PortType"]?.ToString() ?? "",
                PortSpeed = row["PortSpeed"]?.ToString() ?? "",
                PortTag = row["PortTag"]?.ToString() ?? "",
                PortSlotNumber = Convert.ToInt32(row["PortSlotNumber"]),
                PortId = row["PortId"]?.ToString() ?? "",
                Status = Convert.ToInt32(row["PortStatus"]?.ToString() ?? "0"),
                Mode = row["Mode"]?.ToString() ?? "",
                PortName = row["PortName"]?.ToString() ?? "",
                VlanId = row["VlanId"]?.ToString() ?? "",
                TagA = row["TagA"]?.ToString() ?? "",
                TagB = row["TagB"]?.ToString() ?? "",
                TagC = row["TagC"]?.ToString() ?? "",
                TagD = row["TagD"]?.ToString() ?? "",
                TagE = row["TagE"]?.ToString() ?? "",
                TagF = row["TagF"]?.ToString() ?? "",
                AssetId = row["AssetId"]?.ToString() ?? ""
            };

            // 端口颜色处理
            if (row["PortColor"] == DBNull.Value || string.IsNullOrEmpty(row["PortColor"]?.ToString()))
                info.PortColor = 0;
            else
                info.PortColor = Convert.ToInt32(row["PortColor"]);

            // 在线状态
            if (row["OnTheLine"] == DBNull.Value || string.IsNullOrEmpty(row["OnTheLine"]?.ToString()))
                info.OnTheLine = -1;
            else
                info.OnTheLine = Convert.ToInt32(row["OnTheLine"]);

            DataBridge.DataBridge.PortDetailedInfos.Add(info);
        }
    }

    /// <summary>
    /// 解析端口配置 - 按槽位分组
    /// </summary>
    private void AnalysisPortInfos2()
    {
        var groupedItems = DataBridge.DataBridge.PortDetailedInfos
            .GroupBy(item => new { item.PortSlotNumber, item.PortType });

        foreach (var group in groupedItems)
        {
            foreach (var item in group)
            {
                item.FullPortId = $"{item.PortSlotNumber}{item.PortId}";
            }
        }
    }

    private string? settingTags;

    /// <summary>
    /// 加载端口自定义标签
    /// </summary>
    private void LoadCustomTag()
    {
        var tagWindow = "DevicePortTag";
        if (DataBridge.DataBridge.SelectDeviceTableInfo != null)
            tagWindow += DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;

        string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{tagWindow}'";
        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        string? tags;
        if (num > 0)
            tags = DbClass.LoadWindowTag(tagWindow);
        else
            tags = DbClass.LoadWindowTag("DevicePortTag");

        if (tags != null)
        {
            settingTags = tags;
        }
    }

    /// <summary>
    /// 加载网段信息备注标签
    /// </summary>
    private void LoadTags()
    {
        settingTags = DbClass.LoadWindowTag("AddDevice");
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchKeyWord.Text = null;
        LoadAssetTreeViewInfos();
    }

    private void SearchButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(SearchKeyWord.Text))
            LoadAssetTreeViewInfos(SearchKeyWord.Text);
        else
            LoadAssetTreeViewInfos();
    }

    private async void AddSlot_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataBridge.DataBridge.SelectDeviceTableInfo == null) return;

        var window = this.VisualRoot as Window;
        if (window == null) return;

        var newWindow = new Windows.DevicePortManage.AddDeviceSlotWindow();
        if (await newWindow.ShowDialog<bool>(window))
        {
            await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);
            AnalysisPortInfos2();
        }
    }

    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataBridge.DataBridge.SelectDeviceTableInfo == null) return;

        var window = this.VisualRoot as Window;
        if (window == null) return;

        var newWindow = new Windows.DevicePortManage.DeviceEditWindow();
        newWindow.ShowDialog(window).ContinueWith(_ =>
        {
            Dispatcher.UIThread.Post(() => LoadAssetTreeViewInfos());
        });
    }

    private async void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataBridge.DataBridge.SelectDeviceTableInfo == null) return;

        // 检查端口是否在链路上
        var sql = $"SELECT COUNT(OnTheLine) FROM De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId} WHERE OnTheLine IS NOT NULL AND OnTheLine != -1 AND OnTheLine != ''";
        int count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        if (count > 0)
        {
            var parentWindow = this.VisualRoot as Window;
            if (parentWindow != null)
                await Services.DialogService.ShowWarning($"设备上有{count}个端口已经在链路上，无法删除！", "无法删除");
            return;
        }

        // 确认删除
        var confirm = await Services.DialogService.ShowDeleteConfirm(DataBridge.DataBridge.SelectDeviceTableInfo.AssetNumber ?? "该设备");
        if (!confirm) return;

        var query = $"DELETE FROM Devices WHERE AssetId = '{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}'";
        GlobalVariables.DbService.ExecuteNonQuery(query);

        query = $"DROP TABLE De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}";
        GlobalVariables.DbService.ExecuteNonQuery(query);

        query = $"UPDATE Asset SET Deploy = NULL WHERE AssetId = '{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}'";
        GlobalVariables.DbService.ExecuteNonQuery(query);

        DataBridge.DataBridge.SelectDeviceTableInfo = null;
        LoadAssetTreeViewInfos();
    }
}
