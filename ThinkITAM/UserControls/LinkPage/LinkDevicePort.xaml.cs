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
using Nmap.NET.Container;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;

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


        
        

        portInfo.SlotClass = DbClass.GetDeviceSlotInfo(assetId, portLinkClass.PortClass.UID);




        #region MyRegion

        switch (DataBridge.DataBridge.LinkManageMode)
        {


            case 0://链路查看模式
                DataBridge.DataBridge.LinkViewList.Clear();
                if (portLinkClass.PortClass.OnTheLine != null && portLinkClass.PortClass.OnTheLine > 0)
                {

                    foreach (var node in DbClass.GetLinkDetail(portLinkClass.PortClass.OnTheLine))
                    {
                        if (portLinkClass.PortClass.RackId == node.PortClass.RackId)
                        {
                            portLinkClass.PortClass.NodeIndex = node.PortClass.NodeIndex;
                            DataBridge.DataBridge.RackSelectPortInfo = portLinkClass.PortClass;
                            portLinkClass.PortClass.IsSelected = true;
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


        #endregion












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
