using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.Windows.ToolWindows
{
    /// <summary>
    /// WakeOnLanWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WakeOnLanWindow : Window
    {
        public WakeOnLanWindow()
        {
            InitializeComponent();
        }


        private void WakeOnLanWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


            HostsDataGrid.ItemsSource = hosts;

            LoadWakeOnLanHosts();
        }

        private void AddHostButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddWakeOnLan wake = new AddWakeOnLan();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                wake.Owner = window;
            }


            if (wake.ShowDialog() == true)
            {
                //加载数据
                LoadWakeOnLanHosts();
            }

        }


        private ObservableCollection<WakeOnLanHostViewModel> hosts = new ObservableCollection<WakeOnLanHostViewModel>();
        private void LoadWakeOnLanHosts()
        {
            hosts.Clear();
            string sql = $"SELECT *  FROM WakeOnLan";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);
            int index = 0;
            foreach (var row in rows)
            {
                index++;
                var host = new WakeOnLanHostViewModel();

                host.Index = index;
                host.UID = Convert.ToInt32(row["UID"]);

                if (row["Name"] != DBNull.Value)
                {
                    host.Name = row["Name"].ToString();
                }

                if (row["HostGroup"] != DBNull.Value)
                {
                    host.HostGroup = row["HostGroup"].ToString();
                }

                if (row["IpAddress"] != DBNull.Value)
                {
                    host.IpAddress = row["IpAddress"].ToString();
                }

                if (row["NetMask"] != DBNull.Value)
                {
                    host.Netmask = row["NetMask"].ToString();
                }


                if (row["Port"] != DBNull.Value)
                {
                    try
                    {
                        host.Port = Convert.ToInt32(row["Port"]);
                    }
                    catch (Exception e)
                    {
                        host.Port = 9;
                    }

                }

                if (row["PinToStart"] != DBNull.Value)
                {
                    string value = row["PinToStart"].ToString();

                    if (value == "True")
                    {
                        host.PinToStart = true;
                    }
                    else
                    {
                        host.PinToStart = false;
                    }
                }
                else
                {
                    host.PinToStart = false;
                }






                host.Mac = row["Mac"].ToString();


                hosts.Add(host);
            }


        }


        /// <summary>
        /// 精准唤醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void WakeOnLanButton0_OnClick(object sender, RoutedEventArgs e)
        {
            var info = HostsDataGrid.SelectedItem as WakeOnLanHostViewModel;

            try
            {
                await WakeOnLan.SendMagicPacketAsync(
                    macAddress: info.Mac,
                    ipAddress: info.IpAddress,
                    port: info.Port,
                    sendToSpecificIp: false,
                    subnetMask: string.IsNullOrWhiteSpace(info.Netmask) ? null : info.Netmask);

                Console.WriteLine($"已发送WOL包到 {info.Mac}所在IP地址{info.IpAddress}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败: {ex.Message}");
            }

        }

        /// <summary>
        /// 广播唤醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WakeOnLanButton_OnClick(object sender, RoutedEventArgs e)
        {
            var info = HostsDataGrid.SelectedItem as WakeOnLanHostViewModel;


            try
            {
                await WakeOnLan.SendMagicPacketAsync(
                    macAddress: info.Mac,
                    ipAddress: info.IpAddress,
                    port: info.Port,
                    sendToSpecificIp: false,
                    subnetMask: string.IsNullOrWhiteSpace(info.Netmask) ? null : info.Netmask);

                Console.WriteLine($"已发送WOL包到 {info.Mac}所在网段的广播地址");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败: {ex.Message}");
            }
        }

        private void HostsDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // 迭代视觉树以找到 DataGridRow
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            // 获取 DataGridRow
            DataGridRow row = dep as DataGridRow;
            if (row == null)
                return;

            // 获取行数据对象
            var rowData = row.Item as WakeOnLanHostViewModel;
            if (rowData != null)
            {

                Console.WriteLine(rowData.PinToStart);

                // 逻辑代码

                AddWakeOnLan wake = new AddWakeOnLan(rowData);

                var window = Window.GetWindow(this);

                if (window != null)
                {
                    wake.Owner = window;
                }


                if (wake.ShowDialog() == true)
                {
                    //加载数据
                    LoadWakeOnLanHosts();
                }

            }
        }
    }
}
