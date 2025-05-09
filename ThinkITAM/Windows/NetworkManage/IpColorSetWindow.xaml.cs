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
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// PortColorSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IpColorSetWindow : Window
    {
        public IpColorSetWindow(IpAddressInfoListViewMode info)
        {
            InitializeComponent();
            ipInfo = info;
            this.DataContext= info;
        }

        private IpAddressInfoListViewMode ipInfo;

        private DbClass dbClass;
        private void IpColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(DataBridge.DataBridge.NetworkTableName);

            //var item = PortColor.SelectedItem as ListBoxItem;

            Console.WriteLine(ipInfo.AddressColor);

            //ColorGrid.Background = item.Background;

            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();



        }


        /// <summary>
        /// 选择颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PortColor.SelectedIndex;

            int x = 0;


            if (index != -1)
            {
                foreach (var selectedItem in PortColor.Items)
                {
                    var item = selectedItem as ListBoxItem;

                    if (item != null && index == x)
                    {
                        item.Opacity = 1;
                        item.BorderBrush = SystemColors.ActiveBorderBrush;
                        item.BorderThickness = new Thickness(2);

                        ColorGrid.Background = item.Background;
                    }
                    else
                    {
                        item.Opacity = 0.1;
                        item.BorderBrush = null;
                        item.BorderThickness = new Thickness(0);
                    }

                    x++;
                }

            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            int colorCode = PortColor.SelectedIndex;

            string sql = $"UPDATE {DataBridge.DataBridge.NetworkTableName}  SET  AddressColor  = '{colorCode}' WHERE Address = {ipInfo.Address}";


            dbClass.ExecuteQuery(sql);

            DataBridge.DataBridge.IpAddressInfoLists[ipInfo.Address].AddressColor = PortColor.SelectedIndex;
            
            


            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


    }
}
