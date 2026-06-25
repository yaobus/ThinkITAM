using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.Windows.LinkWindows;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;

namespace ThinkITAM.Windows.DevicePortManage
{
    /// <summary>
    /// PortAllocationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortAllocationWindow : Window
    {

        // 定义一个事件，本窗口关闭时触发
        // 定义一个带布尔值参数的事件
        //public event EventHandler<BoolEventArgs> PortAllocationWindowClosed;


        private PortTypeClass.PortDetailedInfo portInfo;

        private List<PortTypeClass.PortDetailedInfo> portInfos;

        public PortAllocationWindow()
        {
            InitializeComponent();


                portInfos = DataBridge.DataBridge.PortDetailedInfos.Where(port => port.IsSelected).ToList();



                if (portInfos.Count == 1)
                {
                    this.portInfo = portInfos[0];
                    this.DataContext = portInfo;
                    SelectedPort.Text = portInfo.FullPortId;

                    portInfo.PropertyChanged += PortInfo_PropertyChanged;

                }
                else
                {
                    if (portInfos.Count == 0)
                    {
                        return;
                    }


                    //取出其中一个端口，用于存储修改后的信息
                    this.portInfo = portInfos[0];
                    this.DataContext = portInfo;
                    portInfo.PropertyChanged += PortInfo_PropertyChanged;



                    string text = null;

                    if (portInfos.Count <= 5)
                    {
                        for (int i = 0; i < portInfos.Count; i++)
                        {
                            text += portInfos[i].FullPortId + ";";

                        }


                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            text += portInfos[i].FullPortId + ";";

                        }
                        text += $"等{portInfos.Count}个端口";
                    }


                    SelectedPort.Text = text;
                }


            




        }



        private void PortAllocationWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            if (portInfo != null)
            {
                LoadPortMode();
                LoadPortStatus();
                LoadLabelTag();

                PortMode.ItemsSource = portModes;

                PortStatus.ItemsSource = portStatusList;

                //CustomColor.ItemsSource = colorList;

                //SelectedPort.Text= portInfo.FullPortId;



                DeviceName.Text = DataBridge.DataBridge.SelectDeviceTableInfo.Description;
                DeviceNumber.Text = DataBridge.DataBridge.SelectDeviceTableInfo.AssetNumber;
                Model.Text = DataBridge.DataBridge.SelectDeviceTableInfo.Model;

                if (portInfo.PortType == "D")
                {

                    if (portInfo != null)
                    {
                        switch (portInfo.Mode)
                        {
                            case "SATA":
                                PortMode.SelectedIndex = 0;
                                break;

                            case "SAS":
                                PortMode.SelectedIndex = 1;
                                break;

                            case "PCIE NVMe":
                                PortMode.SelectedIndex = 2;
                                break;

                            case "SCSI":
                                PortMode.SelectedIndex = 2;
                                break;

                            case "M.2":
                                PortMode.SelectedIndex = 2;
                                break;

                            case "U.2":
                                PortMode.SelectedIndex = 2;
                                break;


                            default:

                                PortMode.SelectedIndex = -1;
                                break;
                        }
                    }



                }
                else
                {
                    if (portInfo != null)
                    {
                        switch (portInfo.Mode)
                        {
                            case "Hybrid":
                                PortMode.SelectedIndex = 0;
                                break;

                            case "Trunk":
                                PortMode.SelectedIndex = 1;
                                break;


                            case "Access":

                                PortMode.SelectedIndex = 2;

                                break;


                            case "Route":

                                PortMode.SelectedIndex = 3;

                                break;
                            default:

                                PortMode.SelectedIndex = -1;
                                break;
                        }
                    }

                }


                //关联链路不为空，显示关联设备
                int onTheLine = -1;
                try
                {
                    onTheLine = Convert.ToInt32(portInfo.OnTheLine);
                }
                catch (Exception exception)
                {
                    onTheLine = -1;
                }

                if (onTheLine > 0)
                {
                    // 查找LinkDetail表，取出最后一个节点的assetId
                    var sql = $"SELECT * FROM LinkDetail WHERE LinkId = {onTheLine} ORDER BY SequenceNo DESC LIMIT 1;";

                    var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                    string devicesAssetId = null;

                    int portUID = -1;

                    foreach (var row in rows)
                    {
                        devicesAssetId = row["DevicesAssetId"].ToString();
                        portUID = Convert.ToInt32(row["PortUID"].ToString());

                    }

                    if (devicesAssetId != null && portUID != -1)
                    {
                        //  判断节点是否是自己
                        if (devicesAssetId == DataBridge.DataBridge.SelectDeviceTableInfo.AssetId)//是自己
                        {
                            //查询第一个节点
                            // 查找LinkDetail表，取出最后一个节点的assetId
                            var sql2 = $"SELECT * FROM LinkDetail WHERE LinkId = {onTheLine} ORDER BY SequenceNo ASC LIMIT 1;";

                            var rows2 = GlobalVariables.DbService.ExecuteQuery(sql2);

                            foreach (var row2 in rows2)
                            {
                                devicesAssetId = row2["DevicesAssetId"].ToString();
                                portUID = Convert.ToInt32(row2["PortUID"].ToString());
                            }


                        }





                        var node = DbClass.GetLinkDeviceInfo(devicesAssetId, portUID);

                        var info = $"{node.InfoA}\r{node.InfoB}\r{node.InfoC}\r{node.InfoD}\r{node.InfoE}";


                        DeviceLink.Text = info;

                    }


                }

            }



        }


        /// <summary>
        /// 根据端口类型加载对应标签
        /// </summary>
        private void LoadLabelTag()
        {
            if (portInfo.PortType == "D")
            {
                IdLabel.Content = "Raid:";
            }
            else
            {

                IdLabel.Content = "VlanID:";
            }
        }

        private int changedNumber = 0;
        private void PortInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changedNumber++;


        }

        /// <summary>
        /// 端口颜色标签被选择
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

        ObservableCollection<string> portModes = new ObservableCollection<string>();

        /// <summary>
        /// 加载端口模式
        /// </summary>
        private void LoadPortMode()
        {
            if (portInfo != null)
            {
                if (portInfo.PortType == "D")
                {
                    portModes.Add("SATA");
                    portModes.Add("SAS");
                    portModes.Add("PCIE NVMe");
                    portModes.Add("SCSI");
                    portModes.Add("M.2");
                    portModes.Add("U.2");
                }
                else
                {

                    portModes.Add("Hybrid");
                    portModes.Add("Trunk");
                    portModes.Add("Access");
                    portModes.Add("Route");
                }
            }








        }

        ObservableCollection<string> portStatusList = new ObservableCollection<string>();

        private void LoadPortStatus()
        {
            portStatusList.Clear();


            portStatusList.Add("未分配");
            portStatusList.Add("已分配，未启用");
            portStatusList.Add("已分配，已启用");
            portStatusList.Add("故障");




        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            PortTypeClass.PortDetailedInfo info = this.DataContext as PortTypeClass.PortDetailedInfo;

            if (changedNumber > 0)
            {
                foreach (var portDetailedInfo in portInfos)
                {
                    info.PortTag = portDetailedInfo.PortTag;
                    info.PortSlotNumber = portDetailedInfo.PortSlotNumber;
                    info.PortId = portDetailedInfo.PortId;
                    info.PortType = portDetailedInfo.PortType;
                   
                    info.UID = portDetailedInfo.UID;

                    //保存修改
                    await SavePortInfo(info);
                }




                this.DialogResult = true;
                this.Close();
            }
            else
            {
                Console.WriteLine("没发生修改");

            }


        }

        /// <summary>
        /// 保存端口信息
        /// </summary>
        /// <param name="info"></param>
        public async Task SavePortInfo(PortTypeClass.PortDetailedInfo info)
        {
            string tableName = $"De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}";

            int uid = info.UID;
            string portTag = info.PortTag;
            //int? portSlotNumber = info.PortSlotNumber;
            //string portId = info.PortId;
            //string portType=info.PortType;
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

            string sql = $"UPDATE  {tableName}  SET  PortStatus  = {status},  Mode  = '{mode}',  PortName  = '{portName}', PortTag  = '{portTag}',  VlanId  = '{vlanId}', OnTheLine  = '{onTheLine}',  PortColor  = '{portColor}',  TagA  = '{tagA}',  TagB  = '{tagB}',  TagC  = '{tagC}',  TagD  = '{tagD}',  TagE  = '{tagE}',  TagF  = '{tagF}' ,  AssetId  = '{assetId}' WHERE  ( UID  = '{uid}')";

            //Console.WriteLine(sql);
            GlobalVariables.DbService.ExecuteNonQuery(sql);
        }

        private void PortAllocationWindow_OnClosed(object? sender, EventArgs e)
        {

            // 触发事件，传递布尔值参数
            //PortAllocationWindowClosed?.Invoke(this, new BoolEventArgs(AllocationStatus));
        }



        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void FindAsset_OnClick(object sender, RoutedEventArgs e)
        //{
        //    string assetId = null;

        //    if (portInfo.AssetId.Length > 0)
        //    {
        //        assetId = portInfo.AssetId;
        //    }


        //    FindAssetWindow findAsset = new FindAssetWindow(assetId);
        //    findAsset.Owner = this;
        //    if (findAsset.ShowDialog() == true)
        //    {
        //        DeviceLink.Text = DataBridge.DataBridge.LinkSelectAssetId;
        //        // LoadTags();

        //    }
        //}


        /// <summary>
        /// 弹出关联链路查找编辑窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindLine_OnClick(object sender, RoutedEventArgs e)
        {

            FindLinkWindow findLink = new FindLinkWindow();
            findLink.Owner = this;
            if (findLink.ShowDialog() == true)
            {
                //DeviceLink.Text = DataBridge.DataBridge.LinkSelectAssetId;
                // LoadTags();

            }

        }

        private ObservableCollection<PortLinkClass> nodes;

        private void ViewLinkNodes_OnClick(object sender, RoutedEventArgs e)
        {
            int linkId = Convert.ToInt32(portInfo.OnTheLine);

            if (linkId > 0)
            {

                nodes = DbClass.GetLinkDetail(linkId);

                if (nodes.Count > 0)
                {
                    var newWindow = new ViewLinkWindow(nodes, linkId);

                    var window = Window.GetWindow(this);
                    if (window != null)
                    {
                        newWindow.Owner = window;
                    }

                    newWindow.ShowDialog();
                }

            }




        }
    }


}


