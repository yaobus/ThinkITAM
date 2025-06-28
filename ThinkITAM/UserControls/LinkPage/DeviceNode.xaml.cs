using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// DerviceNode.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceNode : UserControl
    {
        public DeviceNode()
        {
            InitializeComponent();
        }

        private void DeviceNodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            //发生修改的端口信息，含配线架及槽位号
            PortLinkClass portInfo = new PortLinkClass();

            // 获取 Port的 DataContext
            var portDataContext = this.DataContext;
            var p = portDataContext as PortLinkClass;

            // portInfo.PortClass

            MdfRackClass mdfRack = new MdfRackClass();

            mdfRack = DbClass.GetRackInfo(p.PortClass.RackId);



            portInfo.MdfRackClass = mdfRack;





            var info = p.PortClass;

            //Console.WriteLine("RackId:"+portInfo.MdfRackClass.RackId);

            portInfo.PortClass = info;

            portInfo.SlotClass = DbClass.GetBuildingRoomInfo(DataBridge.DataBridge.SelectedBuildingId, info.UID);

            portInfo.MdfRackClass.CabinetName = p.PortClass.SlotIndex;

            portInfo.MdfRackClass.RackName = p.PortClass.Room;

            int count = DataBridge.DataBridge.LinkManageList.Count;

            switch (DataBridge.DataBridge.LinkManageMode)
            {


                case 0://链路查看模式

                    DataBridge.DataBridge.LinkViewList.Clear();

                    if (info.OnTheLine != null && info.OnTheLine > 0)
                    {
                        foreach (var node in DbClass.GetLinkDetail(info.OnTheLine))
                        {
                            if (node.PortClass.DeviceId != null)
                            {
                                info.NodeIndex = node.PortClass.NodeIndex;
                                DataBridge.DataBridge.RackSelectPortInfo = info;
                                info.IsSelected = true;
                            }
                            else
                            {
                                if (info.RackId == node.PortClass.RackId)
                                {
                                    info.NodeIndex = node.PortClass.NodeIndex;
                                    DataBridge.DataBridge.RackSelectPortInfo = info;
                                    info.IsSelected = true;
                                }
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


                        if (mdfs.Count > 0)//已有一个同配线架的端口
                        {
                            //如果是同一个配线架同一个端口，则移除该端口
                            var items = DataBridge.DataBridge.LinkManageList.Where(item => item.PortClass == info).ToList();

                            if (items.Count > 0)
                            {
                                info.IsSelected = false;
                                info.NodeIndex = 0;
                                DataBridge.DataBridge.LinkManageList.Remove(items[0]);

                            }
                            else
                            {

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


                            // Console.WriteLine(nowType+":"+oldType);


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
    }
}
