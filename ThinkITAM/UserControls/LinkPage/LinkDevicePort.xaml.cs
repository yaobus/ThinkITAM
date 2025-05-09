using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ThinkITAM.Windows.DevicePortManage;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;
using static ThinkITAM.Windows.NetworkManage.AddressAllocationWindow;

namespace ThinkITAM.UserControls.LinkPage;
/// <summary>
/// EthernetPortInfo.xaml 的交互逻辑
/// </summary>
public partial class LinkDevicePort : UserControl
{
    public LinkDevicePort()
    {
        InitializeComponent();
    }
    public event EventHandler<BoolEventArgs> PortAllocationWindowClosed;

    private void EthernetButton_OnClick(object sender, RoutedEventArgs e)
    {


        //发生修改的端口信息，含配线架及槽位号
        PortLinkClass portInfo = new PortLinkClass();

        // portInfo.PortClass

       
        // 获取 Port的 DataContext
       
        var portLinkClass = this.DataContext as PortLinkClass;

        string assetId = portLinkClass.PortClass.RackId;


        //机架信息
        portInfo.MdfRackClass = DbClass.GetRackInfo(assetId);

        //端口信息
        portInfo.PortClass = portLinkClass.PortClass;

        portInfo.PortClass.PortType = portLinkClass.PortClass.PortType;

        Console.WriteLine(portInfo.PortClass.PortType);

        //switch (portType)
        //{
        //    case "E":
        //        portInfo.PortClass.PortType = "Eth";
        //        break;
        //    case "M":
        //        portInfo.PortClass.PortType = "Eth";
        //        break;
        //    default:
        //        portInfo.PortClass.PortType = "LC";
        //        break;
        //}

        
        

        portInfo.SlotClass = DbClass.GetDeviceSlotInfo(assetId, portLinkClass.PortClass.UID);

        //槽位信息
        //portInfo.SlotClass.SlotIndex = portLinkClass.PortClass.SlotIndex;

        //portInfo.MdfRackClass.RackName = portInfo.SlotClass.SlotTag;

        int count = DataBridge.DataBridge.LinkManageList.Count;


        if (count > 0)
        {
            //判断列表中是否已有同一个配线架
            var mdfs = DataBridge.DataBridge.LinkManageList.Where(item => item.MdfRackClass.RackId == assetId).ToList();


            if (mdfs.Count > 0)//已有一个同配线架的端口
            {
                //如果是同一个配线架同一个端口，则移除该端口
                var items = DataBridge.DataBridge.LinkManageList.Where(item => item.PortClass == portLinkClass.PortClass).ToList();

                if (items.Count > 0)
                {
                    portLinkClass.PortClass.IsSelected = false;
                    portLinkClass.PortClass.NodeIndex = 0;
                    Console.WriteLine(portLinkClass.PortClass.UID);
                    DataBridge.DataBridge.LinkManageList.Remove(items[0]);

                }
                else
                {
                    MessageBox.Show("链路不能重复通过同一个父节点");
                }




            }
            else//不是同一个
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





    }


    private void Window_PortAllocationWindowClosed(object? sender, AddressAllocationWindow.BoolEventArgs e)
    {
        if (e.Result == true)
        {

            // 传递布尔值参数
            PortAllocationWindowClosed?.Invoke(this, e);
        }
    }

    private void ColorTagSet_OnClick(object sender, RoutedEventArgs e)
    {
        var info = (PortTypeClass.PortDetailedInfo)this.DataContext;

        PortColorSetWindow add = new PortColorSetWindow(info);

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            add.Owner = window;
        }

        if (add.ShowDialog() == true)
        {

            // DataBridge.DataBridge.ModifyIpColorList.Add(AddressBlock.Text);
            // 当子窗口关闭后执行这里的代码

        }
    }

    private void LinkDevicePort_OnLoaded(object sender, RoutedEventArgs e)
    {

    }
}
