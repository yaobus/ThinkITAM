using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using ThinkITAM.Functions.FunctionClass;

namespace ThinkITAM.FunctionPage;
/// <summary>
/// Dashboard.xaml 的交互逻辑
/// </summary>
public partial class Dashboard : UserControl
{
    public Dashboard()
    {
        InitializeComponent();


        PointLabel = chartPoint =>
            string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

        DataContext = this;
    }

    public Func<ChartPoint, string> PointLabel
    {
        get; set;
    }

    private async void Dashboard_OnLoaded(object sender, RoutedEventArgs e)
    {
        await  LoadStatisticsInfo();
        //await Task.Run(() => LoadStatisticsInfo());
        this.DataContext = dashboard;

    }

    private ViewModels.Dashboard.DashboardViewModel dashboard  = new ViewModels.Dashboard.DashboardViewModel();
    
    /// <summary>
    /// 加载统计信息
    /// </summary>
    private async Task LoadStatisticsInfo()
    {

        dashboard.NetworkCount = StatisticsClass.StatisticsNetworkCount();
        dashboard.IpAddressCount = StatisticsClass.StatisticsIpAddressCount();
        dashboard.DeviceCount = StatisticsClass.StatisticsDevicesCount();
        dashboard.ComputerCount = StatisticsClass.StatisticsComputerCount();
        dashboard.LinkCount = StatisticsClass.StatisticsLinkCount();
        dashboard.LinkNodeCount = StatisticsClass.StatisticsNodeCount();
    }
}
