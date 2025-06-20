using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Index;

namespace ThinkITAM.FunctionPage;
/// <summary>
/// Dashboard.xaml 的交互逻辑
/// </summary>
public partial class Dashboard : UserControl
{
    public Dashboard()
    {
        InitializeComponent();



        DataContext = this;
    }


    private async void Dashboard_OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadStatisticsInfo();

        this.DataContext = dashboard;
        IndexTagsPanel.ItemsSource = dashboardIndexTags;
        await LoadIndexTags();

       

    }

    private ViewModels.Dashboard.DashboardViewModel dashboard = new ViewModels.Dashboard.DashboardViewModel();

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


    private ObservableCollection<DashboardIndexTagViewModel> dashboardIndexTags =
        new ObservableCollection<DashboardIndexTagViewModel>();

    /// <summary>
    /// 加载分组信息
    /// </summary>
    private async Task LoadIndexTags(string searchKeyWord = "")
    {

        var query = $"SELECT DISTINCT TypeGroup FROM  Bookmark WHERE PinToStart = 1 AND (Del != 1 OR Del IS NULL);";

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            var info = new DashboardIndexTagViewModel();
            info.GroupName = row["TypeGroup"].ToString();

            var sql = $"SELECT * FROM Bookmark WHERE TypeGroup = '{info.GroupName}' AND PinToStart = 1 AND (Del != 1 OR Del IS NULL);";

            var tagRows = GlobalVariables.DbService.ExecuteQuery(sql);

            ObservableCollection<IndexTagViewModel> tags = new ObservableCollection<IndexTagViewModel>();

            foreach (var tagRow in tagRows)
            {
                var tagInfo = new ViewModels.Index.IndexTagViewModel();
                tagInfo.IndexId = tagRow["IndexId"].ToString();
                tagInfo.Group = info.GroupName;
                tagInfo.Name = tagRow["Name"].ToString();
                tagInfo.Protocol = tagRow["Protocol"].ToString();
                tagInfo.Host = tagRow["Host"].ToString();
                tagInfo.Port = tagRow["Port"].ToString();
                tagInfo.Browser = tagRow["Browser"].ToString();

                string url = $"{tagInfo.Protocol}{tagInfo.Host}";

                if (tagInfo.Port.Length == 0)//未配置端口
                {
                    tagInfo.Url = url;
                }
                else
                {
                    tagInfo.Url = $"{url}:{tagInfo.Port}";
                }

                int pinToStart;
                try
                {
                    pinToStart = Convert.ToInt32(tagRow["PinToStart"]);
                }
                catch (Exception e)
                {
                    pinToStart = 0;
                }

                tagInfo.PinToStart = pinToStart;


                int colorIndex = 0;
                try
                {
                    colorIndex = Convert.ToInt32(tagRow["Color"]);
                }
                catch (Exception exception)
                {
                    colorIndex = 0;
                }

                tagInfo.Color = colorIndex;
                tags.Add(tagInfo);


                


            }

            info.IndexTags = tags;
            dashboardIndexTags.Add(info);
            await Task.Delay(10);

        }


    }

}
