using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddressExportWizardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddressExportWizardWindow : Window
    {
        public AddressExportWizardWindow()
        {
            InitializeComponent();
            NetworkInfosDataGrid.ItemsSource = networkInfos;
        }


        private ObservableCollection<NetworkInfoViewMode>
            networkInfos = new ObservableCollection<NetworkInfoViewMode>();
        /// <summary>
        /// 加载网段信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressExportWizardWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadNetworkInfo();

        }


        /// <summary>
        /// 加载网段信息
        /// </summary>
        private void LoadNetworkInfo()
        {
            networkInfos.Clear();

            string query = "SELECT * FROM Network WHERE Del IS NULL OR Del != 1 ;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                var info = new NetworkInfoViewMode();

                info.Index = index;
                info.NetworkId = row["NetworkId"].ToString();
                info.Name = row["Name"].ToString();
                info.Network = row["Network"].ToString();
                info.Netmask = row["Netmask"].ToString();
                info.Description = row["Description"].ToString();
                
                networkInfos.Add(info);
            }

        }
    }
}
