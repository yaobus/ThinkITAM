using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.ViewModels.NetworkManage;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.FunctionPage;
using ThinkITAM.Functions.FunctionClass;
using static ThinkITAM.Windows.NetworkManage.AddressAllocationWindow;
using ThinkITAM.Windows.ToolWindows;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.UserControls.NetworkManage;

/// <summary>
/// IpAddressInfo.xaml 的交互逻辑
/// </summary>
public partial class IpAddressInfo : UserControl
{
    public IpAddressInfo()
    {
        InitializeComponent();
    }

    public event EventHandler<BoolEventArgs> AddressAllocationWindowClosed;

    private void AddressButton_Click(object sender, RoutedEventArgs e)
    {

        if (sender is Button button)
        {
           
            var info = button.DataContext as IpAddressInfoListViewMode;

            int index = info.Index;

            int selectAddress = Convert.ToInt32(AddressBlock.Text);

            
            if (selectAddress != null)
            {
                string url = DataBridge.DataBridge.SelectNetwork + selectAddress;

                switch (DataBridge.DataBridge.OperationType)
                {
                    case 0: //单个分配模式

                       

                        int addressStatus = Convert.ToInt32(button.Tag);


                        DataBridge.DataBridge.AddressStatus = addressStatus;//记录所选地址类型

                        //当前要修改的IP地址

                        DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected = true;

                        DataBridge.DataBridge.SelectAddress.Add(selectAddress);


                        AddressAllocationWindow addressAllocationWindow = new AddressAllocationWindow(DataBridge.DataBridge.IpAddressInfoLists);

                        //窗口放中间
                        var window = Window.GetWindow(this);
                        if (window != null)
                        {
                            addressAllocationWindow.Owner = window;
                        }

                        //addressAllocationWindow.Owner = Application.Current.MainWindow;
                        addressAllocationWindow.AddressAllocationWindowClosed += AddressAllocationWindow_AddressAllocationWindowClosed;

                        if (addressAllocationWindow.ShowDialog() == true)
                        {
                            
                        }
                        DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected = false;
                        DataBridge.DataBridge.SelectAddress.Add(1);
                        break;

                    case 1: //多个分配模式

                        //1、判断选择的第一个地址是已分配还是未分配
                        //1.1 判断是否是第一个地址
                        
                        
                        if (GetSelectedAddressCount() == 0) //当前选择的是第一个地址
                        {
                            //记录当前选择的地址是已分配还是未分配
                            DataBridge.DataBridge.AddressStatus = Convert.ToInt32(button.Tag);
                            

                            DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected = true;

                            DataBridge.DataBridge.SelectAddress.Add(selectAddress);

                        }
                        else//当前选择的不是第一个地址
                        {
                            //判断当前选择的地址是已分配还是未分配
                            if (Convert.ToInt32(button.Tag) == DataBridge.DataBridge.AddressStatus)//同种类型的地址则添加到列表中，否则不添加
                            {
                                
                                //地址不存在则添加
                                if (DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected==false)
                                {

                                    DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected = true;
                                    DataBridge.DataBridge.SelectAddress.Add(selectAddress);
                                }
                                else//地址已存在，则删除
                                {

                                    DataBridge.DataBridge.IpAddressInfoLists[index].IsSelected = false;

                                    DataBridge.DataBridge.SelectAddress.Add(selectAddress);

                                }

                                

                            }

                            
                        }

                        ButtonCommand?.Execute(null);
                        break;

                    case 2: //浏览器访问模式

                        var port =string.Empty;

                        if (!string.IsNullOrWhiteSpace(DataBridge.DataBridge.SelectPort))
                        {
                            port = $":{DataBridge.DataBridge.SelectPort}";
                        }

                        string url2 = $"{DataBridge.DataBridge.Protocol}{url}{port}";


                        

                       OpenUrlClass.OpenUrlInSpecificBrowser(url2, DataBridge.DataBridge.SelectBrowser);

                        

                        break;
                    case 3: //PING模式


                        string arguments = $"-t {url}";

                        // 创建一个新的ProcessStartInfo对象
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe", // 要执行的程序是cmd.exe
                            Arguments = $"/k ping {arguments}", // /k参数告诉cmd执行完命令后不会自动退出
                            UseShellExecute = false, // 必须设置为false，否则无法重定向标准输入输出
                            CreateNoWindow = false // 创建一个新窗口来显示CMD窗口
                        };

                        // 创建一个新的Process对象
                        Process process = new Process
                        {
                            StartInfo = processStartInfo
                        };

                        // 启动进程
                        process.Start();

                        break;
                    case 4: //端口检测模式


                        break;
                }



            }

        }

    }
    /// <summary>
    /// 查询选中的地址数量
    /// </summary>
    /// <returns></returns>
    public int GetSelectedAddressCount()
    {
        int sum = DataBridge.DataBridge.IpAddressInfoLists.Sum(item => Convert.ToInt32(item.IsSelected));

        return sum;
    }

    private void AddressAllocationWindow_AddressAllocationWindowClosed(object sender, BoolEventArgs e)
    {

        if (e.Result == true)
        {
            // 传递布尔值参数
            AddressAllocationWindowClosed?.Invoke(this, e);
        }

       

    }


    public static readonly DependencyProperty ButtonCommandProperty =
        DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(IpAddressInfo));

    public ICommand ButtonCommand
    {
        get => (ICommand)GetValue(ButtonCommandProperty);
        set => SetValue(ButtonCommandProperty, value);
    }

    /// <summary>
    /// 修改颜色标签
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorTagSet_OnClick(object sender, RoutedEventArgs e)
    {
       var info= (IpAddressInfoListViewMode)this.DataContext;

        IpColorSetWindow add = new IpColorSetWindow(info);

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            add.Owner = window;
        }

        if (add.ShowDialog() == true)
        {
           
            DataBridge.DataBridge.ModifyIpColorList.Add(AddressBlock.Text);
            // 当子窗口关闭后执行这里的代码

        }
    }




    /// <summary>
    /// IP地址上的右键菜单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        int selectAddress = Convert.ToInt32(AddressBlock.Text);
        string url = DataBridge.DataBridge.SelectNetwork + selectAddress;

       

        // 获取触发事件的MenuItem
        var menuItem = sender as MenuItem;

        if (menuItem != null)
        {
            // 根据菜单项的不同进行相应的处理
            switch (menuItem.Tag)
            {
                case "PortScan":
                    // 执行选项1的操作

                    PortScanWindow portScanWindow = new PortScanWindow(url);
                    //窗口放中间
                    var window2 = Window.GetWindow(this);
                    if (window2 != null)
                    {
                        portScanWindow.Owner = window2;
                    }

                    portScanWindow.ShowDialog();



                    break;

                case "Ping":

                    string arguments = $"-t {url}";

                    // 创建一个新的ProcessStartInfo对象
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe", // 要执行的程序是cmd.exe
                        Arguments = $"/k ping {arguments}", // /k参数告诉cmd执行完命令后不会自动退出
                        UseShellExecute = false, // 必须设置为false，否则无法重定向标准输入输出
                        CreateNoWindow = false // 创建一个新窗口来显示CMD窗口
                    };

                    // 创建一个新的Process对象
                    Process process = new Process
                    {
                        StartInfo = processStartInfo
                    };

                    // 启动进程
                    process.Start();



                    break;



                case "Collect":

                    AddressCollectWindow addressCollectWindow = new AddressCollectWindow(url);
                    //窗口放中间
                    var window = Window.GetWindow(this);
                    if (window != null)
                    {
                        addressCollectWindow.Owner = window;
                    }

                    addressCollectWindow.ShowDialog();





                    // 执行选项2的操作
                    break;
                case "Wol":

                    var portInfo = this.DataContext as IpAddressInfoListViewMode;

                    var info = new WakeOnLanHostViewModel();
                    info.IpAddress = url;
                    info.Netmask = DataBridge.DataBridge.SelectNetworkInfo.Netmask;
                    info.Mac = portInfo.MacAddress;
                    info.Port = 9;

                    AddWakeOnLan wake = new AddWakeOnLan(info);




                    var window3 = Window.GetWindow(this);
                    if (window3 != null)
                    {
                        wake.Owner = window3;
                    }

                    wake.ShowDialog();
                    break;
            }
        }
    }
}
