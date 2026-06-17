using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ThinkITAM.Database;
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionPage;
using ThinkITAM.Services;

namespace ThinkITAM.Views;

/// <summary>
/// 主工作区窗口
/// 左侧菜单 ListBox + 右侧 ContentControl 作为页面容器
/// 移植原始 WPF SelectionWindow 的 MenuList_OnSelectionChanged switch-case 逻辑
/// </summary>
public partial class SelectionWindow : Window
{
    private int _windowLoadStatus = 0;

    public SelectionWindow()
    {
        InitializeComponent();
        NavigationService.Initialize(FunctionPanel);
    }

    /// <summary>
    /// 窗口打开后执行初始化
    /// </summary>
    private async void SelectionWindow_OnOpened(object? sender, EventArgs e)
    {
        // 标题栏显示项目名称
        if (GlobalVariables.dbConfig != null)
            Title += " - " + GlobalVariables.dbConfig.NickName;

        // 设置窗口大小策略
        WindowsShow();

        // 初始化数据库
        LoadingIndicator.IsVisible = true;
        await Task.Run(async () =>
        {
            await InitializeDatabase();
            CheckDatabase();
        });
        LoadingIndicator.IsVisible = false;
        _windowLoadStatus = 1;

        // 加载初始页面（Dashboard）
        MenuList.SelectedIndex = -1;
        var dashboard = new Dashboard();
        NavigateToPage(dashboard);
    }

    /// <summary>
    /// 窗口大小策略：<= 1920x1080 则全屏
    /// </summary>
    private void WindowsShow()
    {
        var screens = Screens;
        if (screens != null && screens.Primary != null)
        {
            var bounds = screens.Primary.Bounds;
            if (bounds.Width <= 1920 && bounds.Height <= 1080)
            {
                WindowState = WindowState.Maximized;
            }
        }
    }

    /// <summary>
    /// 初始化数据库（创建所需表）
    /// </summary>
    private async Task InitializeDatabase()
    {
        var tables = new List<string>
        {
            "Network", "UserInfo", "WindowTag", "Hierarchy", "Organization",
            "AssetTag", "Address", "Asset", "Browser", "PortList",
            "ModelPreset", "Devices", "DeviceRoom", "Protocol",
            "DeviceCabinet", "Racks", "Buildings", "ScanPorts",
            "Notes", "CustomSetting", "Bookmark", "Link",
            "LinkDetail", "WakeOnLan", "Computer", "Models",
            "BookmarkGroupOrder", "AssetLog", "NoteBook"
        };

        // 注意：DbClass.CreateTableIfNotExists 待移植
        foreach (var table in tables)
        {
            try
            {
                // TODO: 移植 DbClass.CreateTableIfNotExists 后在 Phase 3 启用
                // var result = DbClass.CreateTableIfNotExists(table);
                Console.WriteLine($"检查表: {table}");
            }
            catch { }
        }
    }

    /// <summary>
    /// 检查数据库字段完整性（版本升级时添加新字段）
    /// </summary>
    private void CheckDatabase()
    {
        if (AppSettings.VersionNumber <= DataBridge.DataBridge.VersionNumber)
        {
            if (DataBridge.DataBridge.NowOpenedDataBaseType == "sqlite")
            {
                try
                {
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "LinkIp", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Organization", "UserUnit", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("UserInfo", "UserUnit", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Asset", "UserUnit", "TEXT");

                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagA", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagB", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagC", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagD", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagE", "TEXT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Computer", "TagF", "TEXT");

                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("NetWork", "SortIndex", "INT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("DeviceRoom", "SortIndex", "INT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("DeviceCabinet", "SortIndex", "INT");
                    GlobalVariables.DbService?.CheckAndAddColumnIfNotExists("Buildings", "SortIndex", "INT");

                    AppSettings.VersionNumber = DataBridge.DataBridge.VersionNumber;
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// 清除全局状态（切换页面时清理旧页面数据）
    /// </summary>
    private void ClearGlobalValue(int index = 0)
    {
        if (index > 0)
        {
            DataBridge.DataBridge.NetworkTableName = null;
            DataBridge.DataBridge.OperationType = 0;
            DataBridge.DataBridge.IpAddressInfoLists.Clear();
            DataBridge.DataBridge.LoadedNetworkSegment = null;
            DataBridge.DataBridge.LinkViewList.Clear();
            DataBridge.DataBridge.PortPanelLinkViewList.Clear();
        }
    }

    /// <summary>
    /// 主菜单选择事件 — 页面切换
    /// 索引: 0=菜单开关, 1=Dashboard, 2=Network, 3=DevicePort, 4=IndexPage,
    ///       5=Link, 6=Asset, 7=PortPanel, 8=Computer, 9=Tools
    /// </summary>
    private void MenuList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var index = MenuList.SelectedIndex;
        if (index < 0) return;

        ClearGlobalValue(index);
        MenuList2.SelectedIndex = -1;
        DataBridge.DataBridge.SelectedFunction = index;

        switch (index)
        {
            case 0: // 导航栏收放
                double currentWidth = MenuList.Width;
                if (currentWidth != 140.0)
                {
                    MenuList.Width = 140.0;
                    MenuList2.Width = 140.0;
                    MenuIcon.Kind = Material.Icons.MaterialIconKind.MenuOpen;
                }
                else
                {
                    MenuList.Width = 45.0;
                    MenuList2.Width = 45.0;
                    MenuIcon.Kind = Material.Icons.MaterialIconKind.MenuClose;
                }
                MenuList.SelectedIndex = -1;
                break;

            case 1: // Dashboard
                NavigateToPage(new Dashboard());
                break;

            case 2: // 网络地址管理
                NavigateToPage(new NetworkAddressManagePage());
                break;

            case 3: // 设备端口管理
                NavigateToPage(new DevicePortManage());
                break;

            case 4: // 书签索引
                NavigateToPage(new IndexPage());
                break;

            case 5: // 链路管理
                NavigateToPage(new LinkUserControl());
                break;

            case 6: // 资产管理
                NavigateToPage(new AssetManage());
                break;

            case 7: // 配线架端口
                NavigateToPage(new PortPanel());
                break;

            case 8: // 计算机管理
                NavigateToPage(new ComputerPage());
                break;

            case 9: // 工具箱
                NavigateToPage(new ToolsPage());
                break;
        }
    }

    /// <summary>
    /// 底部辅助菜单选择事件
    /// 索引: 0=预设配置, 1=帮助, 2=关于
    /// </summary>
    private void MenuList2_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        int index = MenuList2.SelectedIndex;
        if (index < 0) return;

        MenuList.SelectedIndex = -1;

        switch (index)
        {
            case 0: // 预设配置
                NavigateToPage(new PresetPage());
                break;

            case 1: // 帮助文档
                OpenHelpDocument();
                MenuList2.SelectedIndex = -1;
                break;

            case 2: // 关于
                NavigateToPage(new About());
                break;
        }
    }

    /// <summary>
    /// 打开帮助文档
    /// </summary>
    private async void OpenHelpDocument()
    {
        var sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Document", "ThinkITAM使用手册.pdf");

        try
        {
            if (File.Exists(sourceFilePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = sourceFilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                await DialogService.ShowWarning("帮助文件不存在！", "文件未找到");
            }
        }
        catch (Exception ex)
        {
            await DialogService.ShowError($"无法打开帮助文件：{ex.Message}", "错误");
        }
    }

    /// <summary>
    /// 切换到指定页面
    /// </summary>
    private void NavigateToPage(UserControl page)
    {
        NavigationService.NavigateTo(page);
    }
}
