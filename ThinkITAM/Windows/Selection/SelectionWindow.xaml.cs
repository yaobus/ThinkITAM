using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionPage;
using ThinkITAM.Functions.FunctionClass;
using Windows.Data.Pdf;


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
        this.Title += ("-" + GlobalVariables.dbConfig.NickName);

        switch (DataBridge.DataBridge.NowOpenedDataBaseType)
        {
            case "mysql":
                SetCharset();
                break;
        }

        

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


                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("NetWork", "SortIndex", "INT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("DeviceRoom", "SortIndex", "INT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("DeviceCabinet", "SortIndex", "INT");
                    GlobalVariables.DbService.CheckAndAddColumnIfNotExists("Buildings", "SortIndex", "INT");


                    //GlobalVariables.DbService.CheckAndAddColumnIfNotExists("AssetTag", "Autocomplete", "INT");

                    //扩展网段表单字段
                    ExtendSegmentTable();

                    //修改字段名称
                    ModifyFieldNameSqlite();

                    Properties.Settings.Default.VersionNumber = DataBridge.DataBridge.VersionNumber;
                    Properties.Settings.Default.Save();

                    break;

                case "mysql":
                case "mariadb":

                    //GlobalVariables.DbService.CheckAndAddColumnIfNotExists("AssetTag", "Autocomplete", "INT");
                    ModifyFieldNameMysql();

                    break;
            }


        }




    }


    /// <summary>
    /// 修改数据库字段编码字符集为utf8mb4
    /// </summary>
    private void SetCharset()
    {
       var versionStr = GlobalVariables.DbService.ExecuteScalar("SELECT VERSION();").ToString();


       // 2. 判断是否为 MariaDB
       bool isMariaDb = versionStr.IndexOf("mariadb", StringComparison.OrdinalIgnoreCase) >= 0;

       if (isMariaDb)
       {
           // MariaDB 不使用 utf8mb4_0900_ai_ci，跳过
           return;
       }

       // 3. 提取主版本号（如 "8.0.33" -> Version(8,0,33)）
       string cleanVersion = versionStr.Split('-', ' ')[0]; // 去掉后缀如 "-log"
       if (!Version.TryParse(cleanVersion, out Version version))
       {
          
           Console.WriteLine(versionStr);

       }

       // 4. 仅当 MySQL 且版本 >= 8.0 时执行 ALTER
       if (version.Major >= 8)
       {
           string alterSql = $@"
                ALTER DATABASE `{GlobalVariables.dbConfig.DatabaseName}`
                CHARACTER SET utf8mb4
                COLLATE utf8mb4_0900_ai_ci;";

           Console.WriteLine(alterSql);

           Console.WriteLine(GlobalVariables.DbService.ExecuteNonQuery(alterSql));

            return; // 成功执行
       }

       return ; // 未满足条件，未执行




    }




    /// <summary>
    /// 对网段表添加字段
    /// </summary>
    private void ExtendSegmentTable()
    {
        var infos = Functions.FunctionClass.NetworkHelper.GetAllNetworkInfo();

        foreach (var info in infos)
        {
            GlobalVariables.DbService.CheckAndAddColumnIfNotExists(info.TableName, "FullAddress", "TEXT");
        }
    }

    /// <summary>
    /// 修改SQLITE字段名称
    /// </summary>
    private void ModifyFieldNameSqlite()
    {

        var query = "ALTER TABLE WindowTag RENAME COLUMN Window TO WindowName;";
        

        try
        {
            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
           
        }


        //修改组织架构字段
        query = "ALTER TABLE Organization RENAME COLUMN Groups TO UserGroups;";
        try
        {

            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        // 修改自定义字段字段
        query = "ALTER TABLE CustomSetting RENAME COLUMN Option TO CustomOption;";

        try
        {

            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }
    }

    /// <summary>
    /// 修改mysql字段名称
    /// </summary>
    private void ModifyFieldNameMysql()
    {
        var query = "ALTER TABLE WindowTag RENAME COLUMN `Window` TO WindowName;";

        try
        {

            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        query = "ALTER TABLE Organization RENAME COLUMN `Groups` TO UserGroups;";

        try
        {

            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        query = "ALTER TABLE CustomSetting RENAME COLUMN `Option` TO CustomOption;";

        try
        {

            GlobalVariables.DbService.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

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
        t.Add("AssetLog");
        t.Add("NoteBook");
        var message = string.Empty;

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
            this.Width = 1920;
            this.Height = 1080;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

    /// <summary>
    /// 清空全局变量
    /// </summary>
    private void ClearGlobalValue(int index=0)
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

        ClearGlobalValue(index);

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
                var presetPage = new PresetPage();

                presetPage.Style = (Style)FindResource("PresetPageStyle");

                FunctionPanel.Children.Add(presetPage);

                break;
            case 1:
                //加载帮助文档


                //加载帮助文档
                var result = MessageBox.Show("是否打开本地帮助?\r选否将会在默认浏览器打开在线帮助", "选择帮助文档", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {

                    // 2. 获取嵌入资源或者本地文件内容
                    string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Document\\ThinkITAM使用手册.pdf");



                    try
                    {
                        // 检查文件是否存在
                        if (File.Exists(sourceFilePath))
                        {
                            // 启动默认程序打开PDF
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = sourceFilePath,
                                UseShellExecute = true  // 必须为 true 才能使用默认程序
                            });
                        }
                        else
                        {
                            MessageBox.Show("帮助文件不存在！", "文件未找到", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 捕获可能的异常（如无默认程序、权限问题等）
                        MessageBox.Show($"无法打开帮助文件：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {


                    if (result == MessageBoxResult.No)
                    {
                        OpenUrlClass.OpenUrlInSpecificBrowser("https://thinkitam.goeasy.work/", null);
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


