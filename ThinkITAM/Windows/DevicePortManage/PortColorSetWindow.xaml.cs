using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;
using Microsoft.Win32;
using ThinkITAM.DataBridge;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;

namespace ThinkITAM.Windows.DevicePortManage
{
    /// <summary>
    /// PortColorSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortColorSetWindow : Window
    {
        public PortColorSetWindow(PortDetailedInfo info)
        {
            InitializeComponent();
            portInfo = info;
            this.DataContext = portInfo;
            
        }

        private PortDetailedInfo portInfo;



        private int? index;
        private bool saved=false;
        private void IpColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            index = portInfo.PortColor;




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

        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //portInfo.CustomColor = PortColor.SelectedIndex;

            await SavePortInfo(portInfo);
            saved = true;
            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            //portInfo.CustomColor = index;  
            DialogResult = false;
        }

        /// <summary>
        /// 保存端口信息
        /// </summary>
        /// <param name="info"></param>
        public async Task SavePortInfo(PortTypeClass.PortDetailedInfo info)
        {
            string tableName = $"De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}";

            int? portColor = info.PortColor;


            //string sql = $"UPDATE  {tableName}  SET  PortStatus  = {portStatus},  Mode  = '{mode}',  PortName  = '{portName}',  VlanId  = '{vlanId}',  OnTheLine  = '{onTheLine}',  PortColor  = '{portColor}',  TagA  = '{tagA}',  TagB  = '{tagB}',  TagC  = '{tagC}',  TagD  = '{tagD}',  TagE  = '{tagE}',  TagF  = '{tagF}' ,  AssetId  = '{assetId}' WHERE  ( UID  = '{portInfo.UID}' )";

            string sql = $"UPDATE  {tableName}  SET  PortColor  = '{portColor}'  WHERE  ( UID  = '{portInfo.UID}' )";


            GlobalVariables.DbService.ExecuteNonQuery(sql);
        }

        private void PortColorSetWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            if (saved == false)
            {
                portInfo.PortColor = index;
                DialogResult = false;
            }
        }
    }
}
