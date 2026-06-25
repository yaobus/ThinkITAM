using System.Windows;
using System.Windows.Controls;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.Windows.DevicePortManage;
using ThinkITAM.Windows.NetworkManage;
using static ThinkITAM.Windows.NetworkManage.AddressAllocationWindow;

namespace ThinkITAM.UserControls.DevicePortManage;
/// <summary>
/// EthernetPortInfo.xaml 的交互逻辑
/// </summary>
public partial class DevicePort : UserControl
{
    public DevicePort()
    {
        InitializeComponent();
    }
    public event EventHandler<BoolEventArgs> PortAllocationWindowClosed;

    private void EthernetButton_OnClick(object sender, RoutedEventArgs e)
    {


        if (sender is Button button)
        {

            var info = button.DataContext as ViewModels.DevicePortManage.PortTypeClass.PortDetailedInfo;

            int sum = DataBridge.DataBridge.PortDetailedInfos.Sum(item => Convert.ToInt32(item.IsSelected));




            if (info.FullPortId != null)
            {


                switch (DataBridge.DataBridge.PortOperationType)
                {
                    case 0: //单选模式

                        info.IsSelected = true;

                        PortAllocationWindow portAllocationWindow = new PortAllocationWindow();

                        string portName = $"{info.PortType}{info.PortSlotNumber}{info.PortId}";

                        DataBridge.DataBridge.PortSelectCount.Add(portName);


                        //窗口放中间
                        var window = Window.GetWindow(this);
                        if (window != null)
                        {
                            portAllocationWindow.Owner = window;
                        }

                        //window.Owner = Application.Current.MainWindow;
                        //portAllocationWindow.PortAllocationWindowClosed += Window_PortAllocationWindowClosed;

                        if (portAllocationWindow.ShowDialog() == true)
                        {
                            //清空选择
                            foreach (var item in DataBridge.DataBridge.PortDetailedInfos.ToList()) // ToList()创建了一个快照，避免在遍历时修改集合引发的问题
                            {
                                item.IsSelected = false;

                            }


                        }
                        else
                        {
                            //清空选择
                            foreach (var item in DataBridge.DataBridge.PortDetailedInfos.ToList()) // ToList()创建了一个快照，避免在遍历时修改集合引发的问题
                            {
                                item.IsSelected = false;

                            }
                        }

                        DataBridge.DataBridge.PortSelectCount.Clear();
                        break;


                    case 1://多选模式
                           //判断是否是选中的第一个端口

                        if (sum == 0)
                        {
                            DataBridge.DataBridge.SelectPortMode = info.Status;
                            DataBridge.DataBridge.SelectPortType = info.PortType;

                            info.IsSelected = true;

                            string port = $"{info.PortType}{info.PortSlotNumber}{info.PortId}";

                            DataBridge.DataBridge.PortSelectCount.Add(port);


                        }
                        else//不是第一个端口
                        {
                            //判断是否是同一个模式及同一个类型
                            if (DataBridge.DataBridge.SelectPortMode == info.Status && DataBridge.DataBridge.SelectPortType == info.PortType)//同一个模式
                            {

                                string port = $"{info.PortType}{info.PortSlotNumber}{info.PortId}";

                                if (info.IsSelected == true)
                                {
                                    info.IsSelected = false;
                                    DataBridge.DataBridge.PortSelectCount.Remove(port);
                                }
                                else
                                {
                                    info.IsSelected = true;

                                    DataBridge.DataBridge.PortSelectCount.Add(port);

                                }


                            }
                            else//不是同一个模式
                            {
                                info.IsSelected = false;
                                Console.WriteLine($"端口模式或类型不一样，已有端口模式为:{DataBridge.DataBridge.SelectPortMode},当前选择的端口模式为：{info.Status} ，已有类型为{DataBridge.DataBridge.SelectPortType}，当前选择的端口类型为：{info.PortType}");
                            }

                        }



                        break;
                }


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
}
