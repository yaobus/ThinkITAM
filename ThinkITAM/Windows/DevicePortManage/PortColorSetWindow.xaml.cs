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
using ThinkITAM.ViewModes.DevicePortManage;
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModes.NetworkManage;
using Microsoft.Win32;
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

        private DbClass dbClass;

        private int? index;
        private bool saved=false;
        private void IpColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            index = portInfo.PortColor;
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

            string portTag = info.PortTag;
            int? portSlotNumber = info.PortSlotNumber;
            string portId = info.PortId;
            string portType = info.PortType;
            string mode = info.Mode;
            int? status = info.Status;
            string vlanId = info.VlanId;
            string portName = info.PortName;
            int? portColor = info.PortColor;

            int? onTheLine = info.OnTheLine;
            string tagA = info.TagA;
            string tagB = info.TagB;
            string tagC = info.TagC;
            string tagD = info.TagD;
            string tagE = info.TagE;
            string tagF = info.TagF;
            string assetId = DataBridge.DataBridge.LinkAssetId;

            string sql = $"UPDATE \"{tableName}\" SET \"Status\" = {status}, \"Mode\" = '{mode}', \"PortName\" = '{portName}', \"VlanId\" = '{vlanId}', \"OnTheLine\" = '{onTheLine}', \"PortColor\" = '{portColor}', \"TagA\" = '{tagA}', \"TagB\" = '{tagB}', \"TagC\" = '{tagC}', \"TagD\" = '{tagD}', \"TagE\" = '{tagE}', \"TagF\" = '{tagF}' , \"AssetId\" = '{assetId}' WHERE  (\"UID\" = '{portInfo.UID}' )";

            dbClass.ExecuteQuery(sql);
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
