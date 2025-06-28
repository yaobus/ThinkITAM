using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
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
            this.DataContext = info;
        }

        private IpAddressInfoListViewMode ipInfo;


        private void IpColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(DataBridge.DataBridge.NetworkTableName);

            //var item = PortColor.SelectedItem as ListBoxItem;

            Console.WriteLine(ipInfo.AddressColor);

            //ColorGrid.Background = item.Background;




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

            Console.WriteLine(sql);



            GlobalVariables.DbService.ExecuteNonQuery(sql);

            DataBridge.DataBridge.IpAddressInfoLists[ipInfo.Index].AddressColor = PortColor.SelectedIndex;




            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


    }
}
