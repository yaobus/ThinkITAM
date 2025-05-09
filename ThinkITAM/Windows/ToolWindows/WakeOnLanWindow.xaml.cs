using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Others;
using Nodify;
using static MaterialDesignThemes.Wpf.Theme;

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

        private DbClass dbClass;
        private void WakeOnLanWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

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

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);

            SQLiteDataReader reader = command.ExecuteReader();

            if (reader != null)
            {
                int index = 0;

                while (reader.Read())
                {
                    index++;
                    var host = new WakeOnLanHostViewModel();
                   
                    host.Index = index;
                    host.UID = Convert.ToInt32(reader["UID"]);
                   
                    if (reader["Name"] != DBNull.Value)
                    {
                        host.Name = reader["Name"].ToString();
                    }

                    if (reader["HostGroup"] != DBNull.Value)
                    {
                        host.HostGroup= reader["HostGroup"].ToString();
                    }

                    if (reader["IpAddress"] != DBNull.Value)
                    {
                        host.IpAddress = reader["IpAddress"].ToString();
                    }

                    if (reader["NetMask"] != DBNull.Value)
                    {
                        host.Netmask = reader["NetMask"].ToString();
                    }


                    if (reader["Port"] != DBNull.Value)
                    {
                        try
                        {
                            host.Port = Convert.ToInt32(reader["Port"]);
                        }
                        catch (Exception e)
                        {
                            host.Port = 9;
                        }
                       
                    }

                    if (reader["PinToStart"]!= DBNull.Value)
                    {
                        string value = reader["PinToStart"].ToString();

                        if (value=="True")
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




                   

                    host.Mac = reader["Mac"].ToString();


                    hosts.Add(host);

                }
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
