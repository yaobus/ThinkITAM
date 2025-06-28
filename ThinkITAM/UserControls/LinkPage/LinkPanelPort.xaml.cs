using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.Windows.PortPanel;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// EthernetPort.xaml 的交互逻辑
    /// </summary>
    public partial class LinkPanelPort : UserControl
    {
        public static readonly DependencyProperty RackInfoProperty =
            DependencyProperty.Register("PortPanelInfo", typeof(PortPanelClass), typeof(LinkPanelPort), new PropertyMetadata(null));



        public PortPanelClass PortPanelInfo
        {
            get
            {
                return (PortPanelClass)GetValue(RackInfoProperty);
            }
            set
            {
                SetValue(RackInfoProperty, value);
            }
        }



        public LinkPanelPort()
        {
            InitializeComponent();
            this.DataContext = PortPanelInfo;


        }

        private void Port_OnLoaded(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 端口被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortButton_OnClick(object sender, RoutedEventArgs e)
        {
            //发生修改的端口信息，含配线架及槽位号
            PortLinkClass portInfo = new PortLinkClass();

            // portInfo.PortClass

            MdfRackClass mdfRack = new MdfRackClass();
            mdfRack = DbClass.GetRackInfo(DataBridge.DataBridge.SelectedBuildingId);



            portInfo.MdfRackClass = mdfRack;


            // 获取 Port的 DataContext
            var portDataContext = this.DataContext;

            var p = portDataContext as PortLinkClass;

            var info = p.PortClass;

            //Console.WriteLine("RackId:"+portInfo.MdfRackClass.RackId);

            portInfo.PortClass = info;

            portInfo.SlotClass = DbClass.GetBuildingRoomInfo(DataBridge.DataBridge.SelectedBuildingId, info.UID);

            portInfo.MdfRackClass.CabinetName = portInfo.SlotClass.SlotName;
            portInfo.MdfRackClass.RackName = portInfo.SlotClass.SlotTag;

            int count = DataBridge.DataBridge.LinkManageList.Count;

            switch (DataBridge.DataBridge.LinkManageMode)
            {


                case 0://添加顺藤摸瓜起点

                    DataBridge.DataBridge.LinkViewList.Clear();
                    if (info.OnTheLine != null && info.OnTheLine > 0)
                    {
                        foreach (var node in DbClass.GetLinkDetail(info.OnTheLine))
                        {
                            if (info.RackId == node.PortClass.RackId)
                            {
                                info.NodeIndex = node.PortClass.NodeIndex;
                                DataBridge.DataBridge.RackSelectPortInfo = info;
                                info.IsSelected = true;
                            }



                            DataBridge.DataBridge.LinkViewList.Add(node);
                        }
                    }
                    else
                    {
                        DataBridge.DataBridge.LinkViewList.Clear();

                    }


                    break;


                case 1://链路管理模式

                    if (count > 0)
                    {
                        //判断列表中是否已有同一个配线架
                        var mdfs = DataBridge.DataBridge.LinkManageList.Where(item => item.MdfRackClass.RackId == mdfRack.RackId).ToList();


                        if (mdfs.Count > 0)//已有一个同配线架（房间）的端口
                        {
                            //如果是同一个配线架同一个端口，则移除该端口
                            var items = DataBridge.DataBridge.LinkManageList.Where(item => item.PortClass == info).ToList();

                            if (items.Count > 0)
                            {
                                info.IsSelected = false;
                                info.NodeIndex = 0;
                                Console.WriteLine(info.UID);
                                DataBridge.DataBridge.LinkManageList.Remove(items[0]);

                            }
                            else//不是是同一个配线架同一个端口，判断是否是同一个房间的端口和终端
                            {

                                var DeviceNodeCount = 0;

                                foreach (var item in mdfs)
                                {
                                    if (item.PortClass.DeviceId != null)
                                    {
                                        DeviceNodeCount++;
                                        break;
                                    }

                                }


                                //存在与新增节点同一个父节点的Node，判断链路中已有节点是设备节点还是墙面节点
                                var deviceNodeCount = 0;
                                var panelNodeCount = 0;
                                foreach (var mdf in mdfs)
                                {
                                    if (mdf.PortClass.DeviceId != null)
                                    {
                                        deviceNodeCount++;
                                    }
                                    else
                                    {
                                        panelNodeCount++;
                                    }

                                }


                                if (p.PortClass.DeviceId != null)//新增节点是设备节点
                                {
                                    if (deviceNodeCount == 0)
                                    {
                                        DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                        portInfo.PortClass.IsSelected = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("链路中只能有一个终端设备节点");
                                    }

                                }
                                else//墙面节点
                                {
                                    if (panelNodeCount == 0)
                                    {
                                        DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                        portInfo.PortClass.IsSelected = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("链路中只能有一个墙面端口节点");
                                    }
                                }

                            }




                        }
                        else//不是同一个
                        {
                            //判断两个端口是否是同一大类
                            string nowType = info.PortType;


                            string oldType = string.Empty; ;

                            foreach (var node in DataBridge.DataBridge.LinkManageList)
                            {

                                if (node.MdfRackClass.RackId.Substring(0, 1) != "2")//不是设备类型
                                {
                                    oldType = node.PortClass.PortType;
                                    break;
                                }
                            }


                            if (CheckStrings(nowType, oldType) == false)
                            {
                                MessageBox.Show("链路介质类型应该保持一致");

                            }
                            else
                            {
                                //创建链路
                                //先获取节点是否已经在链路上

                                if (portInfo?.PortClass?.OnTheLine == -1)
                                {
                                    if (!DataBridge.DataBridge.LinkManageList.Contains(portInfo))
                                    {
                                        DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                        portInfo.PortClass.IsSelected = true;
                                        Console.WriteLine("Count:" + DataBridge.DataBridge.LinkManageList.Count);
                                    }
                                    else
                                    {
                                        Console.WriteLine("该端口信息已添加到操作列表，请勿重复添加");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("该端口信息已存在关联信息，如需修改请先删除关联信息");
                                }

                            }






                        }

                    }
                    else
                    {

                        if (portInfo?.PortClass?.OnTheLine == -1)
                        {
                            if (!DataBridge.DataBridge.LinkManageList.Contains(portInfo))
                            {
                                DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                portInfo.PortClass.IsSelected = true;
                                Console.WriteLine("Count:" + DataBridge.DataBridge.LinkManageList.Count);
                            }
                            else
                            {
                                Console.WriteLine("该端口信息已添加到操作列表，请勿重复添加");
                            }
                        }
                        else
                        {
                            MessageBox.Show("该端口信息已存在关联信息，如需修改请先删除关联信息");
                        }


                    }



                    break;


                case 2:
                    MessageBox.Show("LINK");
                    break;

                case 3://链路清除模式
                    //第一步，获取链路ID
                    var linkId = portInfo.PortClass.OnTheLine;

                    if (linkId > 0)
                    {
                        var result = MessageBox.Show("是否确认删除该链路\r该操作不可逆!", "注意", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {

                            //清空链路在节点上的信息
                            foreach (var node in DbClass.GetLinkDetail(linkId))
                            {
                                var devicesAssetId = node.MdfRackClass.RackId;
                                var uid = node.PortClass.UID;

                                var tableName = TableNameClass.GetTableName(node);

                                var sql = $"UPDATE {tableName} SET OnTheLine = NULL,PortTag = NULL WHERE UID='{uid}'";

                                GlobalVariables.DbService.ExecuteNonQuery(sql);

                            }

                            //清空链路详表信息

                            var sql2 = $"DELETE FROM LinkDetail WHERE LinkId={linkId}";
                            GlobalVariables.DbService.ExecuteNonQuery(sql2);

                            var sql3 = $"DELETE FROM Link WHERE Link_ID={linkId}";
                            GlobalVariables.DbService.ExecuteNonQuery(sql3);

                            DataBridge.DataBridge.ChangedLink.Add(linkId);

                        }


                    }


                    break;
            }


        }

        /// <summary>
        /// 判断端口类型是否一致
        /// </summary>
        /// <param name="stringA"></param>
        /// <param name="stringB"></param>
        /// <returns></returns>
        private bool CheckStrings(string stringA, string stringB)
        {
            bool containsEInA = stringA.Contains('E') || string.IsNullOrWhiteSpace(stringA);
            bool containsEInB = stringB.Contains('E') || string.IsNullOrWhiteSpace(stringB);

            // 如果两个字符串都包含E（无论大小写），返回true
            // 如果只有一个字符串包含E，返回false
            // 如果两个都不包含E，返回true
            if (containsEInA && containsEInB)
            {
                return true;
            }
            else if (containsEInA != containsEInB) // 一个为true，另一个为false
            {
                return false;
            }
            else // 两个都不包含E的情况
            {
                return true;
            }
        }

        /// <summary>
        /// 端口标签被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;



            PanelPortTagModify add = new PanelPortTagModify(port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.PortPanelModifyTagList.Add($"{port.PortIndex}");
                // 当子窗口关闭后执行这里的代码

            }
        }

        private void ColorTagSet_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;



            PortPanelColorSetWindow add = new PortPanelColorSetWindow(port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }


            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.PortPanelModifyTagList.Add($"{port.PortIndex}");
                // 当子窗口关闭后执行这里的代码

            }


        }



    }
}
