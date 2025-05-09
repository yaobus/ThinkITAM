using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.FunctionPage;
using MaterialDesignThemes.Wpf;


namespace ThinkITAM;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class FunctionWindow : Window
{
    public FunctionWindow()
    {
        InitializeComponent();
        WindowsShow();
    }

    /// <summary>
    /// 窗口是否加载完毕
    /// </summary>
    private int WindowLoadStatus = 0;

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        WindowLoadStatus = 1;
        //BottomControl.SelectedIndex = -1;

        //加载初始页面
        Dashboard dashboard = new Dashboard();

        dashboard.Style = (Style)FindResource("DashboardStyle");

        FunctionPanel.Children.Add(dashboard);


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
        ChildrenWindows.Scan.ScanWindow scan = new ChildrenWindows.Scan.ScanWindow();
        scan.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        scan.ShowDialog();
    }



    private void MenuList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = MenuList.SelectedIndex;
        ClearGlobleValue();


       

        switch (index)
        {
            case 0: //导航栏开关
                double currentWidth = (double)MenuList.ActualWidth;
                
                //MenuList.Width = currentWidth == 160.0 ? 45.0 : 160.0;
                
                if (currentWidth != 140.0)
                {
                    MenuList.Width = 140.0;
                    MenuIcon.Kind = PackIconKind.MenuOpen;
                }
                else
                {
                    MenuList.Width = 45.0;
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
                DevicePortManage devicePortManage = new DevicePortManage();

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
                AssetManage assetManage = new AssetManage();

                assetManage.Style = (Style)FindResource("AssetPageStyle");

                FunctionPanel.Children.Add(assetManage);

                break;

            case 7:
                FunctionPanel.Children.Clear();
                PresetPage presetPage = new PresetPage();

                presetPage.Style = (Style)FindResource("PresetPageStyle");

                FunctionPanel.Children.Add(presetPage);
                break;

            case 8:
                FunctionPanel.Children.Clear();
                PortPanel portPanel = new PortPanel();

                portPanel.Style = (Style)FindResource("PortPanelStyle");

                FunctionPanel.Children.Add(portPanel);
                break;

            case 9:
                FunctionPanel.Children.Clear();

                ToolsPage toolsPage = new ToolsPage();

                toolsPage.Style = (Style)FindResource("ToolsPageStyle");

                FunctionPanel.Children.Add(toolsPage);
                break;


                

        }


    }
}


