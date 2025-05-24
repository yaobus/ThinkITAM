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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
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


                case 0://添加顺藤摸瓜起点
                       //DataBridge.DataBridge.LinkManageSelectPorts.Clear();

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
                               
                                var DeviceNodecount = 0;

                                foreach (var item in items)
                                {
                                    if (item.PortClass.DeviceId != null)
                                    {
                                        DeviceNodecount++;
                                        break;
                                    }
                                }

                                if (DeviceNodecount == 0)
                                {

                                    DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                    portInfo.PortClass.IsSelected = true;
                                    Console.WriteLine("Count:" + DataBridge.DataBridge.LinkManageList.Count);


                                }
                                else
                                {
                                    MessageBox.Show("链路不能重复通过同一个父节点,且链路中只能有一个终端设备节点");
                                    //判断是否全都是设备，还是一个是设备一个是墙面端口

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


                    break;
            }

        }
    }
}
