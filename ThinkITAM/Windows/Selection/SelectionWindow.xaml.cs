using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionPage;


namespace ThinkITAM.Windows.Selection;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class SelectionWindow : Window
{
    public SelectionWindow()
    {
        InitializeComponent();
        WindowsShow();
    }

    /// <summary>
    /// 窗口是否加载完毕
    /// </summary>
    private int WindowLoadStatus = 0;


    private async void SelectionWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        //标题栏显示版本号
        //this.Title += $" Ver {DataBridge.DataBridge.Version}";

        LoadingIndicator.Visibility = Visibility.Visible;
        await Task.Run(async () =>
        {
            await InitializeDatabase();
            CheckDatabase();
        });

        LoadingIndicator.Visibility = Visibility.Collapsed;
        WindowLoadStatus = 1;
        //BottomControl.SelectedIndex = -1;


        //加载初始页面
        Dashboard dashboard = new Dashboard();


        dashboard.Style = (Style)FindResource("DashboardStyle");

        FunctionPanel.Children.Add(dashboard);


    }

    /// <summary>
    /// 检查数据库字段是否完整
    /// </summary>
    private void CheckDatabase()
    {
        //1.0.16版本,Computer表添加LinkIp字段

        if (Properties.Settings.Default.VersionNumber <= DataBridge.DataBridge.VersionNumber)
        {

            switch (DataBridge.DataBridge.NowOpenedDataBaseType)
            {
                case "sqlite":
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "LinkIp", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Organization", "UserUnit", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("UserInfo", "UserUnit", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Asset", "UserUnit", "TEXT");

                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagA", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagB", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagC", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagD", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagE", "TEXT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Computer", "TagF", "TEXT");

                    Properties.Settings.Default.VersionNumber = DataBridge.DataBridge.VersionNumber;
                    Properties.Settings.Default.Save();

                    break;

                case "mysql":
                case "mariadb":


                    break;
            }


        }




    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    private async Task InitializeDatabase()
    {


        List<string> t = new List<string>();

        t.Add("Network");
        t.Add("UserInfo");
        t.Add("WindowTag");
        t.Add("Hierarchy");
        t.Add("Organization");
        t.Add("AssetTag");
        t.Add("Address");
        t.Add("Asset");
        t.Add("Browser");
        t.Add("PortList");

        t.Add("ModelPreset");
        t.Add("Devices");
        t.Add("DeviceRoom");
        t.Add("Protocol");

        t.Add("DeviceCabinet");
        t.Add("Racks");
        t.Add("Buildings");
        t.Add("ScanPorts");

        t.Add("Notes");
        t.Add("CustomSetting");
        t.Add("Bookmark");
        t.Add("Link");

        t.Add("LinkDetail");
        t.Add("WakeOnLan");
        t.Add("Computer");
        t.Add("Models");
        t.Add("BookmarkGroupOrder");

        string message = string.Empty;

        foreach (var table in t)
        {

            // Console.WriteLine(table);

            var result = DbClass.CreateTableIfNotExists(table);

            message += $"{table}表创建结果：{result}\n";


        }


    }

    /// <summary>
    /// 设置窗口显示方式，分辨率小于1080P就全屏显示，否则居中显示
    /// </summary>
    private void WindowsShow()
    {

        // 获取屏幕的宽度和高度
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;

        // 设定最大分辨率
        double maxResolutionWidth = 1920;
        double maxResolutionHeight = 1080;

        // 如果分辨率小于等于1920x1080，则最大化窗口
        if (screenWidth <= maxResolutionWidth && screenHeight <= maxResolutionHeight)
        {
            WindowState = WindowState.Maximized;
        }
        else // 否则，居中显示窗口
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

    /// <summary>
    /// 清空全局变量
    /// </summary>
    private void ClearGlobleValue()
    {
        DataBridge.DataBridge.NetworkTableName = null;
        DataBridge.DataBridge.OperationType = 0;
        DataBridge.DataBridge.IpAddressInfoLists.Clear();
        DataBridge.DataBridge.LoadedNetworkSegment = null;
        DataBridge.DataBridge.LinkViewList.Clear();
        DataBridge.DataBridge.PortPanelLinkViewList.Clear();
    }

    private void TopControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {







    }



    /// <summary>
    /// 扫描资产二维码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QrcodeScan_OnClick(object sender, RoutedEventArgs e)
    {
        Windows.Scan.ScanWindow scan = new Windows.Scan.ScanWindow();
        scan.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        scan.ShowDialog();
    }



    private void MenuList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = MenuList.SelectedIndex;
        ClearGlobleValue();

        MenuList2.SelectedIndex = -1;

        switch (index)
        {
            case 0: //导航栏开关
                double currentWidth = (double)MenuList.ActualWidth;

                //MenuList.Width = currentWidth == 160.0 ? 45.0 : 160.0;

                if (currentWidth != 140.0)
                {
                    MenuList.Width = 140.0;
                    MenuList2.Width = 140.0;
                    MenuIcon.Kind = PackIconKind.MenuOpen;
                }
                else
                {
                    MenuList.Width = 45.0;
                    MenuList2.Width = 45.0;
                    MenuIcon.Kind = PackIconKind.MenuClose;
                }

                MenuList.SelectedIndex = -1;
                break;

            case 1:

                FunctionPanel.Children.Clear();
                Dashboard dashboard = new Dashboard();

                dashboard.Style = (Style)FindResource("DashboardStyle");

                FunctionPanel.Children.Add(dashboard);
                break;

            case 2:
                FunctionPanel.Children.Clear();
                NetworkAddressManagePage addressManage = new NetworkAddressManagePage();

                addressManage.Style = (Style)FindResource("NetworkAddressManagePageStyle");

                FunctionPanel.Children.Add(addressManage);
                break;

            case 3:
                FunctionPanel.Children.Clear();
                FunctionPage.DevicePortManage devicePortManage = new FunctionPage.DevicePortManage();

                devicePortManage.Style = (Style)FindResource("DevicePortManageStyle");

                FunctionPanel.Children.Add(devicePortManage);
                break;


            case 4:
                FunctionPanel.Children.Clear();
                IndexPage indexPage = new IndexPage();

                indexPage.Style = (Style)FindResource("IndexPageStyle");

                FunctionPanel.Children.Add(indexPage);
                break;


            case 5:

                FunctionPanel.Children.Clear();
                LinkUserControl linkUserControl = new LinkUserControl();

                linkUserControl.Style = (Style)FindResource("LinkUserControlPageStyle");

                FunctionPanel.Children.Add(linkUserControl);


                break;


            case 6:
                FunctionPanel.Children.Clear();
                var assetManage = new FunctionPage.AssetManage();

                assetManage.Style = (Style)FindResource("AssetPageStyle");

                FunctionPanel.Children.Add(assetManage);

                break;


            case 7:
                FunctionPanel.Children.Clear();
                var portPanel = new FunctionPage.PortPanel();

                portPanel.Style = (Style)FindResource("PortPanelStyle");

                FunctionPanel.Children.Add(portPanel);
                break;

            case 8:
                FunctionPanel.Children.Clear();

                ComputerPage computer = new ComputerPage();

                computer.Style = (Style)FindResource("ComputerStyle");

                FunctionPanel.Children.Add(computer);
                break;

            case 9:
                FunctionPanel.Children.Clear();

                ToolsPage toolsPage = new ToolsPage();

                toolsPage.Style = (Style)FindResource("ToolsPageStyle");

                FunctionPanel.Children.Add(toolsPage);
                break;




        }


    }


    private void MenuList2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = MenuList2.SelectedIndex;
        MenuList.SelectedIndex = -1;
        switch (index)
        {
            case 0:

                FunctionPanel.Children.Clear();
                PresetPage presetPage = new PresetPage();

                presetPage.Style = (Style)FindResource("PresetPageStyle");

                FunctionPanel.Children.Add(presetPage);

                break;
            case 1:
                //加载帮助文档

                // 1. 弹出保存对话框让用户选择路径
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF 文件 (*.pdf)|*.pdf";
                saveFileDialog.FileName = "ThinkITAM使用手册.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string destinationPath = saveFileDialog.FileName;

                    // 2. 获取嵌入资源或者本地文件内容
                    string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Document\\ThinkITAM使用手册.pdf");

                    try
                    {
                        // 3. 复制文件到目标位置
                        File.Copy(sourceFilePath, destinationPath, overwrite: true);

                        // 4. 打开资源管理器并定位到该文件夹
                        Process.Start("explorer.exe", $"/select,\"{destinationPath}\"");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法导出帮助文档：{ex.Message}");
                    }
                }

                break;
            case 2:

                FunctionPanel.Children.Clear();
                About about = new About();

                about.Style = (Style)FindResource("AboutStyle");

                FunctionPanel.Children.Add(about);



                break;
        }





    }
}


