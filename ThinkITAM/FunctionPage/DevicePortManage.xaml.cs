using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.Export;
using ThinkITAM.UserControls.Asset;
using ThinkITAM.UserControls.DevicePortManage;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.Others;
using ThinkITAM.Windows.DevicePortManage;
using ThinkITAM.Windows.NetworkManage;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;
using static ThinkITAM.Windows.NetworkManage.AddressAllocationWindow;


namespace ThinkITAM.FunctionPage;
/// <summary>
/// DevicePortManage.xaml 的交互逻辑
/// </summary>
public partial class DevicePortManage : UserControl
{
    public DevicePortManage()
    {
        InitializeComponent();
        PortListView.ItemsSource = DataBridge.DataBridge.PortDetailedInfos;
    }


    private void DevicePortManage_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadTags();

        DataBridge.DataBridge.PortSelectCount.CollectionChanged += PortSelectCount_CollectionChanged;

        LoadAssetTreeViewInfos();
    }

    private void PortSelectCount_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {

        // 使用Dispatcher来更新UI
        NumberBlock.Dispatcher.Invoke(() =>
        {
            NumberBlock.Text = GetSelectedCount().ToString();

        });
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        DeviceCreateGuideWindow addDevice = new DeviceCreateGuideWindow();


        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            addDevice.Owner = window;
        }



        if (addDevice.ShowDialog() == true)
        {

            // 当子窗口关闭后执行这里的代码
            LoadAssetTreeViewInfos();

            //加载设备信息
            //LoadTags();
        }
    }



    private ObservableCollection<AssetTypeViewModel> assetTypes = new ObservableCollection<AssetTypeViewModel>();

    private async void LoadAssetTreeViewInfos(string keyWord = null)
    {
        assetTypes.Clear();
        AssetTreeView.Items.Clear();

        string filter = string.Empty;

        if (!string.IsNullOrWhiteSpace(keyWord))
        {
            filter = $"AND ( AssetNumber LIKE '%{keyWord}%' OR Model LIKE '%{keyWord}%' OR Description LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%')";
        }

        string sqlTemp = $"SELECT COUNT(*) FROM Devices  WHERE Del != 1 OR Del IS NULL {filter}";

        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num > 0)
        {
            string query = $"SELECT DISTINCT AssetType FROM Devices WHERE Del != 1 OR Del IS NULL {filter};";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;

                var info = new AssetTypeViewModel();

                info.Index = index;

                string assetTypeInfo = row["AssetType"].ToString();

                info.AssetType = assetTypeInfo;//资产类型

                sqlTemp = $"SELECT * FROM Devices  WHERE AssetType = '{assetTypeInfo}' AND (Del != 1 OR Del IS NULL) {filter}";

                var rows2 = GlobalVariables.DbService.ExecuteQuery(sqlTemp);

                int index2 = 0;

                var deviceTypeItems = new TreeViewItem();

                foreach (var row2 in rows2)
                {
                    index2++;
                    var device = new DeviceInfo();
                    var deviceInfo = new DeviceTypeViewModel();

                    deviceInfo.Index = index2;
                    deviceInfo.DeviceType = row2["DeviceType"].ToString();
                    deviceInfo.AssetId = row2["AssetId"].ToString();
                    deviceInfo.AssetType = assetTypeInfo;
                    deviceInfo.Description = row2["Description"].ToString();
                    deviceInfo.Model = row2["Model"].ToString();
                    deviceInfo.AssetNumber = row2["AssetNumber"].ToString();
                    deviceInfo.ToolTip = $"[{deviceInfo.Description}]-[{deviceInfo.Model}]-[{deviceInfo.AssetNumber}]";


                    device.DataContext = deviceInfo;


                    deviceTypeItems.Items.Add(device);
                }


                info.DeviceTypeCount = "设备总数:" + index2;//设备类型总数

                var assetTypeControl = new AssetTypeUserControl();

                assetTypeControl.DataContext = info;

                deviceTypeItems.Header = assetTypeControl;


                AssetTreeView.Items.Add(deviceTypeItems);

                await Task.Delay(50);

                assetTypes.Add(info);
            }



            //Organization.ItemsSource = organizationInfo;







        }
    }



    private async void AssetTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        DataExport.IsEnabled = false;//导出按钮可用

        if (e != null)
        {
            //清空已选端口列表
            ClearSelectedPort();

            PortManagePanel.Children.Clear();



            if (this.IsLoaded == true)
            {

                // 获取用户选择的项
                var selectedNode = e.NewValue;

                if (selectedNode is DeviceInfo) //如果是子项
                {
                    DataExport.IsEnabled = true;//导出按钮可用
                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    DeviceInfo childNode = selectedNode as DeviceInfo;

                    DeviceTypeViewModel info = childNode.DataContext as DeviceTypeViewModel;

                    DataBridge.DataBridge.SelectDeviceTableInfo = info;

                    //加载网段标签
                    LoadCustomTag();

                    await LoadPortInfos(info);


                    await AnalysisPortInfos();



                    //加载设备信息
                    LoadDeviceInfo(info);

                    EditButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;

                }
                else if (selectedNode is TreeViewItem) //如果是带有子节点的表项
                {
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    TreeViewItem selectedItem = selectedNode as TreeViewItem;

                    if (selectedItem != null)
                    {
                        // 判断节点是否展开
                        if (selectedItem.IsExpanded)
                        {
                            // 如果已经展开，就折叠节点
                            selectedItem.IsExpanded = false;
                        }
                        else
                        {
                            // 如果未展开，就展开节点
                            selectedItem.IsExpanded = true;
                        }
                    }

                }

            }


        }

    }

    /// <summary>
    /// 加载设备信息
    /// </summary>
    /// <param name="tableInfo"></param>
    private void LoadDeviceInfo(DeviceTypeViewModel tableInfo)
    {
        string query = $"SELECT * FROM Devices WHERE AssetId='{tableInfo.AssetId}'  AND Del != 1 OR Del IS NULL;";

        Console.WriteLine(query);

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        var info = new DeviceTypeViewModel();

        foreach (var row in rows)
        {

            info.AssetNumber = row["AssetNumber"].ToString();
            info.AssetType = row["AssetType"].ToString();
            info.DeviceType = row["DeviceType"].ToString();
            info.Model = row["Model"].ToString();
            info.Description = row["Description"].ToString();
            info.User = row["User"].ToString();
            info.TagA = row["TagA"].ToString();
            info.TagB = row["TagB"].ToString();
            info.TagC = row["TagC"].ToString();
            info.TagD = row["TagD"].ToString();
            info.TagE = row["TagE"].ToString();
            info.TagF = row["TagF"].ToString();
        }

        DeviceDetailInfo.DataContext = info;
    }





    /// <summary>
    /// 加载端口信息
    /// </summary>
    /// <param name="tableName"></param>
    private async Task LoadPortInfos(DeviceTypeViewModel tableInfo)
    {
        string tableName = $"De_{tableInfo.AssetId}";

        if (tableName != "" && tableName != null)
        {
            PortManagePanel.Children.Clear();

            DataBridge.DataBridge.PortDetailedInfos.Clear();

            string query = $"SELECT * FROM {tableName};";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            foreach (var row in rows)
            {
                //读取端口信息，并写入列表
                var info = new PortDetailedInfo();
                info.UID = Convert.ToInt32(row["UID"]);
                info.PortType = row["PortType"].ToString();
                info.PortSpeed = row["PortSpeed"].ToString();
                info.PortTag = row["PortTag"].ToString();
                info.PortSlotNumber = Convert.ToInt32(row["PortSlotNumber"]);
                info.PortId = row["PortId"].ToString();
                info.Status = Convert.ToInt32(row["PortStatus"].ToString());
                info.Mode = row["Mode"].ToString();
                info.PortName = row["PortName"].ToString();
                info.VlanId = row["VlanId"].ToString();




                //如果颜色索引数据库返回值为空数据，则使用默认颜色

                if (row["PortColor"] == DBNull.Value || row["PortColor"] == string.Empty)
                {
                    info.PortColor = 0;
                }
                else
                {
                    info.PortColor = Convert.ToInt32(row["PortColor"]);
                }


                if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                {
                    info.OnTheLine = -1;
                }
                else
                {
                    info.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                }




                info.TagA = row["TagA"].ToString();
                info.TagB = row["TagB"].ToString();
                info.TagC = row["TagC"].ToString();
                info.TagD = row["TagD"].ToString();
                info.TagE = row["TagE"].ToString();
                info.TagF = row["TagF"].ToString();
                info.AssetId = row["AssetId"].ToString();

                info.ToolTip = JoinTip(info);

                DataBridge.DataBridge.PortDetailedInfos.Add(info);

            }



        }

    }

    /// <summary>
    /// 自动拼接端口提示信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private string JoinTip(PortDetailedInfo info)
    {
        string tip = null;

        tip += $"端口编号: {info.PortSpeed}{info.PortSlotNumber}{info.PortId}\r";

        //0为未分配，1为已分配未启用，2为已分配，已启用，3为故障

        switch (info.Status)
        {
            case 0:
                tip += $"端口状态: 未分配\r";
                break;
            case 1:
                tip += $"端口状态: 已分配未启用\r";
                break;
            case 2:
                tip += $"端口状态: 已分配\r";
                break;
            case 3:
                tip += $"端口状态: 故障\r";
                break;


        }

        //端口类型，E为以太网口，F为光纤口，D为硬盘，M为管理口
        switch (info.PortType)
        {
            case "E":
                tip += $"端口类型: 以太网口\r";
                break;
            case "F":
                tip += $"端口类型: 光纤网口\r";
                break;
            case "D":
                tip += $"端口类型: 硬盘插槽\r";
                break;
            case "M":
                tip += $"端口类型: 管理网口\r";
                break;
        }

        //端口模式
        if (!string.IsNullOrWhiteSpace(info.Mode))
        {
            tip += $"端口模式: {info.Mode}\r";
        }



        //VLAN或RAID
        if (!string.IsNullOrWhiteSpace(info.VlanId))
        {
            if (info.PortType == "D")//如果是磁盘插槽
            {
                tip += $"RAID类型: {info.VlanId}\r";
            }
            else
            {
                tip += $"VLAN ID: {info.VlanId}\r";
            }

        }




        if (settingTags != null)
        {
            var settings = JsonConvert.DeserializeObject<TagViewModel>(settingTags);

            if (!string.IsNullOrWhiteSpace(info.TagA))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagA))
                {
                    tip += $"{settings.TagA}: {info.TagA}\r";
                }
                else
                {
                    tip += $"自定义标签A: {info.TagA}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagB))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagB))
                {
                    tip += $"{settings.TagB}: {info.TagB}\r";
                }
                else
                {
                    tip += $"自定义标签B: {info.TagB}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagC))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagC))
                {
                    tip += $"{settings.TagC}: {info.TagC}\r";
                }
                else
                {
                    tip += $"自定义标签C: {info.TagC}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagD))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagD))
                {
                    tip += $"{settings.TagD}: {info.TagD}\r";
                }
                else
                {
                    tip += $"自定义标签D: {info.TagD}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagE))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagE))
                {
                    tip += $"{settings.TagE}: {info.TagE}\r";
                }
                else
                {
                    tip += $"自定义标签A: {info.TagE}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagF))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagF))
                {
                    tip += $"{settings.TagF}: {info.TagF}\r";
                }
                else
                {
                    tip += $"自定义标签F: {info.TagF}\r";
                }
            }

        }
        else
        {
            if (!string.IsNullOrWhiteSpace(info.TagA))
            {

                tip += $"自定义标签A: {info.TagA}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagB))
            {


                tip += $"自定义标签B: {info.TagB}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagC))
            {

                tip += $"自定义标签C: {info.TagC}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagD))
            {

                tip += $"自定义标签D: {info.TagD}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagE))
            {

                tip += $"自定义标签A: {info.TagE}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagF))
            {

                tip += $"自定义标签F: {info.TagF}\r";

            }

        }


        return tip.TrimEnd('\r');

    }

    /// <summary>
    /// 解析端口配置
    /// </summary>
    private async Task AnalysisPortInfos()
    {
        //图形化解析

        //把具有相同PortSlotNumber和PortType的项分组

        var groupedItems = DataBridge.DataBridge.PortDetailedInfos
            .GroupBy(item => new { item.PortSlotNumber, item.PortType });
        foreach (var group in groupedItems)
        {
            foreach (var item in group) // 直接遍历group中的元素
            {
                var port = new DevicePort();

                port.PortAllocationWindowClosed += PortAllocationWindowClosed;
                port.Margin = new Thickness(5);
                item.FullPortId = $"{item.PortSlotNumber}{item.PortId}";
                port.DataContext = item;


                PortManagePanel.Dispatcher.Invoke(() =>
                {
                    PortManagePanel.Children.Add(port);
                });


                await Task.Delay(1);
            }

            // 添加分割线
            Separator separator = new Separator();
            separator.Width = 10000; // 设置横线的宽度，根据需要调整
            separator.Opacity = 0.3;

            PortManagePanel.Dispatcher.Invoke(() =>
            {
                PortManagePanel.Children.Add(separator);
            });

        }


    }


    /// <summary>
    /// 订阅窗口关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PortAllocationWindowClosed(object sender, BoolEventArgs e)
    {
        if (e.Result == true)
        {
            //Console.WriteLine("信息传递成功！");

            //更新显示
            //LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);
            await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);

            // 解析端口信息
            await AnalysisPortInfos();

            //MessageBox.Show("信息传递成功！");

            //端口模式设置
            if (SingleSelectMode.IsChecked == true)
            {
                DataBridge.DataBridge.PortOperationType = 0;
            }
            else
            {
                DataBridge.DataBridge.PortOperationType = 1;
            }

        }

    }



    /// <summary>
    /// 单选模式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SingleSelectMode_OnClick(object sender, RoutedEventArgs e)
    {
        if (SingleSelectMode.IsChecked == true)
        {
            MultipleAllocationPanel.Visibility = Visibility.Collapsed;

            DataBridge.DataBridge.SelectPortMode = 0;//默认选择未分配地址

            DataBridge.DataBridge.PortOperationType = 0;//模式为单选模式


            ClearSelectedPort();

            //if (DataBridge.DataBridge.PortSelectLists.Count > 0)
            //{
            //    //清空已选择的端口
            //    DataBridge.DataBridge.PortSelectLists.Clear();

            //    //重新加载端口列表
            //    await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);

            //    //重新解析端口信息
            //    AnalysisPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);
            //}
        }


    }

    /// <summary>
    /// 多选模式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MultipleSelectMode_OnClick(object sender, RoutedEventArgs e)
    {
        if (MultipleSelectMode.IsChecked == true)
        {
            MultipleAllocationPanel.Visibility = Visibility.Visible;

            DataBridge.DataBridge.PortOperationType = 1;//模式为多选模式
            //DataBridge.DataBridge.SelectPortMode = 0;
        }




    }




    /// <summary>
    /// 多选模式一键分配
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MultipleAllocationButton_Click(object sender, RoutedEventArgs e)
    {

        if (GetSelectedCount() < 1)
        {
            return;
        }


        PortAllocationWindow portAllocationWindow = new PortAllocationWindow();


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

            //重新加载
            await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);

            // 解析端口信息
            await AnalysisPortInfos();

            ////清空已选端口列表
            ClearSelectedPort();


            if (SingleSelectMode.IsChecked == true)
            {
                DataBridge.DataBridge.PortOperationType = 0;
            }
            else
            {
                DataBridge.DataBridge.PortOperationType = 1;
            }


        }
    }


    /// <summary>
    /// 清除已选端口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearSelected_OnClick(object sender, RoutedEventArgs e)
    {
        ClearSelectedPort();

    }

    /// <summary>
    /// 清空已选端口
    /// </summary>
    private void ClearSelectedPort()
    {
        foreach (var item in DataBridge.DataBridge.PortDetailedInfos.ToList()) // ToList()创建了一个快照，避免在遍历时修改集合引发的问题
        {

            item.IsSelected = false;

        }
        DataBridge.DataBridge.PortSelectCount.Clear();
    }


    private void Window_PortAllocationWindowClosed(object? sender, AddressAllocationWindow.BoolEventArgs e)
    {
        if (e.Result == true)
        {
            // 传递布尔值参数
            //PortAllocationWindowClosed?.Invoke(this, e);
        }
    }


    private int LoadMode = 0;//0为图形化加载，1为列表化加载




    /// <summary>
    /// 进行自定义标签设置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SetButton_OnClick(object sender, RoutedEventArgs e)
    {
        PortTagSetWindow set = new PortTagSetWindow();

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            set.Owner = window;
        }


        if (set.ShowDialog() == true)
        {

            LoadCustomTag();

        }
    }


    /// <summary>
    /// 自定义标签
    /// </summary>
    private string settingTags;


    /// <summary>
    /// 加载端口信息自定义标签
    /// </summary>
    private void LoadCustomTag()
    {

        //加载端口自定义标签
        var tagWindow = "DevicePortTag" + DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;




        string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE Window ='{tagWindow}'";

        var num = DbClass.ExecuteScalarTableNum(sqlTemp);



        if (num > 0) //存在本地自定义标签
        {
            var tags = DbClass.LoadWindowTag(tagWindow);

            if (tags != null)
            {
                settingTags = tags;

                var settings = JsonConvert.DeserializeObject<TagViewModel>(tags);

                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;

            }

        }
        else //全局标签
        {
            var tags = DbClass.LoadWindowTag("DevicePortTag");

            if (tags != null)
            {
                settingTags = tags;
                var settings = JsonConvert.DeserializeObject<TagViewModel>(tags);
                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;

            }
            else
            {
                TagA.Text = "自定义标签A";
                TagB.Text = "自定义标签B";
                TagC.Text = "自定义标签C";
                TagD.Text = "自定义标签D";
                TagE.Text = "自定义标签E";
                TagF.Text = "自定义标签F";
            }

        }



    }



    /// <summary>
    /// 重置排序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReSort_OnClick(object sender, RoutedEventArgs e)
    {
        PortListView.ItemsSource = null;
        foreach (var column in PortListView.Columns)
        {
            if (column.SortDirection != null)
            {
                column.SortDirection = null;
            }
        }
        PortListView.ItemsSource = DataBridge.DataBridge.PortDetailedInfos;
    }




    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {

        var toggleButton = sender as FrameworkElement;

        if (toggleButton != null)
        {
            var rowData = toggleButton.DataContext as PortDetailedInfo;

            if (rowData != null)
            {

                // 在这里运行你的逻辑代码
                string portType = rowData.PortType;

                int portMode = (int)rowData.Status;

                //1、判断选择的第一个地址是已分配还是未分配
                //1.1 判断是否是第一个地址
                if (GetSelectedCount() == 1) //当前选择的是第一个端口
                {
                    //记录当前选择的端口是已分配还是未分配


                    DataBridge.DataBridge.SelectPortMode = portMode; // 接下来要选择的端口状态只能为当前选中的端口的状态
                    DataBridge.DataBridge.SelectPortType = portType; // 接下来要选择的端口类型只能为当前选中的端口的类型

                    //Console.WriteLine($"端口模式:{portMode}");

                    string portName = $"{rowData.PortType}{rowData.PortSlotNumber}{rowData.PortId}";

                    DataBridge.DataBridge.PortSelectCount.Add(portName);
                    UpdateNumberBlock();
                }
                else//当前选择的不是第一个地址
                {

                    //判断当前选择的地址是已分配还是未分配
                    if (portMode == DataBridge.DataBridge.SelectPortMode && portType == DataBridge.DataBridge.SelectPortType)//同种类型的地址则添加到列表中，否则不添加
                    {

                        rowData.IsSelected = true;
                        string portName = $"{rowData.PortType}{rowData.PortSlotNumber}{rowData.PortId}";

                        DataBridge.DataBridge.PortSelectCount.Add(portName);


                    }
                    else
                    {
                        rowData.IsSelected = false;
                        //DataBridge.DataBridge.PortSelectCount.RemoveAt(0);
                    }

                    //AddressNumber.Text = SelectAddress.Count.ToString();
                    UpdateNumberBlock();
                }

            }
        }
    }

    private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        var toggleButton = sender as FrameworkElement;
        if (toggleButton != null)
        {
            var rowData = toggleButton.DataContext as PortDetailedInfo;

            if (rowData != null)
            {
                // 在这里运行你的逻辑代码

                rowData.IsSelected = false;


                string portName = $"{rowData.PortType}{rowData.PortSlotNumber}{rowData.PortId}";

                DataBridge.DataBridge.PortSelectCount.Remove(portName);


                //DataBridge.DataBridge.PortSelectCount.RemoveAt(0);
                UpdateNumberBlock();

            }
        }
    }



    /// <summary>
    /// 获取选中的个数
    /// </summary>
    /// <returns></returns>
    private int GetSelectedCount()
    {
        return DataBridge.DataBridge.PortDetailedInfos.Sum(item => Convert.ToInt32(item.IsSelected));


    }

    /// <summary>
    /// 更新已选数据总数
    /// </summary>
    private void UpdateNumberBlock()
    {

        // 确保在UI线程上执行更新
        Application.Current.Dispatcher.Invoke(() =>
        {
            //NumberBlock.Text = GetSelectedCount().ToString();
            if (GetSelectedCount() >= 1)
            {
                MultipleAllocationPanel.Visibility = Visibility.Visible;
            }
        });
    }


    private void PortListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

        DependencyObject dep = (DependencyObject)e.OriginalSource;

        // 迭代视觉树以找到 DataGridRow
        while ((dep != null) && !(dep is DataGridRow))
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep == null)
            return;

        // 获取 DataGridRow
        DataGridRow row = dep as DataGridRow;
        if (row == null)
            return;

        // 获取行数据对象
        var rowData = row.Item as PortDetailedInfo;
        if (rowData != null)
        {
            // 逻辑代码
            RunOnDoubleClick(rowData);
        }
    }

    /// <summary>
    /// 双击数据行
    /// </summary>
    /// <param name="rowData"></param>
    private async void RunOnDoubleClick(PortDetailedInfo rowData)
    {
        //rowData.IsSelected = true;

        DataBridge.DataBridge.SelectPortMode = rowData.Status;
        DataBridge.DataBridge.SelectPortType = rowData.PortType;


        PortAllocationWindow allocationWindow = new PortAllocationWindow();

        // addressAllocationWindow.AddressAllocationWindowClosed += AddressAllocationWindow_AddressAllocationWindowClosed;

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            allocationWindow.Owner = window;
        }



        if (allocationWindow.ShowDialog() == true)
        {

            //清空已选端口列表
            ClearSelectedPort();

            //更新显示
            //LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);
            await LoadPortInfos(DataBridge.DataBridge.SelectDeviceTableInfo);

            // 解析端口信息
            await AnalysisPortInfos();

            //MessageBox.Show("信息传递成功！");


            if (SingleSelectMode.IsChecked == true)
            {
                DataBridge.DataBridge.PortOperationType = 0;
            }
            else
            {
                DataBridge.DataBridge.PortOperationType = 1;
            }

        }
    }


    /// <summary>
    /// 删除搜索关键词
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
    {
        SearchKeyWord.Text = null;

        LoadAssetTreeViewInfos();
    }

    /// <summary>
    /// 删除设备
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {


        var result = MessageBox.Show("确定要删除该设备吗？\r该操作不可逆！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            //查询是否有端口已经在链路上
            var sql = $"SELECT COUNT(OnTheLine)  FROM De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId} WHERE OnTheLine IS NOT NULL ";

            
            int count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

            if (count > 0)
            {
                MessageBox.Show($"设备上有{count}个端口已经在链路上，无法删除！", "无法删除", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var query = $"DELETE FROM  Devices  WHERE AssetId = '{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}'";
                GlobalVariables.DbService.ExecuteNonQuery(query);


                query = $"DROP TABLE De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId};";
                GlobalVariables.DbService.ExecuteNonQuery(query);


                query = $"UPDATE Asset SET Deploy = NULL WHERE AssetId = '{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}'";


                GlobalVariables.DbService.ExecuteNonQuery(query);
                LoadAssetTreeViewInfos();
            }


        }

    }


    private async void ShowModeButton_OnClick(object sender, RoutedEventArgs e)
    {
        //已选端口数量大于1，就切换为多选模式
        int sum = DataBridge.DataBridge.PortDetailedInfos.Sum(item => Convert.ToInt32(item.IsSelected));

        if (sum > 1)
        {
            MultipleSelectMode.IsChecked = true;
            MultipleAllocationPanel.Visibility = Visibility.Visible;
            DataBridge.DataBridge.PortOperationType = 1;
        }
        else
        {
            SingleSelectMode.IsChecked = true;
            MultipleAllocationPanel.Visibility = Visibility.Collapsed;
            DataBridge.DataBridge.PortOperationType = 0;
        }



    }

    /// <summary>
    /// 编辑设备信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        DeviceEditWindow newWindow = new DeviceEditWindow();


        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            newWindow.Owner = window;
        }



        if (newWindow.ShowDialog() == true)
        {

            // 当子窗口关闭后执行这里的代码
            LoadAssetTreeViewInfos();

            //加载设备信息
            //LoadTags();
        }
    }


    /// <summary>
    /// 搜索设备
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(SearchKeyWord.Text))
        {
            LoadAssetTreeViewInfos(SearchKeyWord.Text);
        }
        else
        {
            LoadAssetTreeViewInfos();
        }
    }

    private void SearchKeyWord_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchButton_OnClick(null, null);
        }
    }

    private void DataExport_OnClick(object sender, RoutedEventArgs e)
    {


        if (DataBridge.DataBridge.PortDetailedInfos == null || DataBridge.DataBridge.PortDetailedInfos.Count == 0)
        {
            MessageBox.Show("没有可导出的数据。");
            return;
        }



        var fileName = $"{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}-{DateTime.Now.ToString("yyyyMMddHHmmss")}";

        // 创建保存文件对话框
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel 文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true,
            FileName = fileName  // 默认文件名
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string selectedFilePath = saveFileDialog.FileName;


            // 调用导出方法
            ExcelExporter.ExportToExcel(DataBridge.DataBridge.PortDetailedInfos, selectedFilePath);
        }




    }



    /// <summary>
    /// 加载网段信息备注标签
    /// </summary>
    private void LoadTags()
    {

        settingTags = DbClass.LoadWindowTag("AddDevice");


        if (settingTags != null)
        {
            var settings = JsonConvert.DeserializeObject<TagViewModel>(settingTags);



            if (!string.IsNullOrWhiteSpace(settings.TagA))
            {
                HintAssist.SetHint(TagATextBox, settings.TagA);
            }
            else
            {
                HintAssist.SetHint(TagATextBox, "TagA");
            }

            if (!string.IsNullOrWhiteSpace(settings.TagB))
            {
                HintAssist.SetHint(TagBTextBox, settings.TagB);
            }
            else
            {
                HintAssist.SetHint(TagBTextBox, "TagB");
            }

            if (!string.IsNullOrWhiteSpace(settings.TagC))
            {
                HintAssist.SetHint(TagCTextBox, settings.TagC);
            }
            else
            {
                HintAssist.SetHint(TagCTextBox, "TagC");
            }

            if (!string.IsNullOrWhiteSpace(settings.TagD))
            {
                HintAssist.SetHint(TagDTextBox, settings.TagD);
            }
            else
            {
                HintAssist.SetHint(TagDTextBox, "TagD");
            }

            if (!string.IsNullOrWhiteSpace(settings.TagE))
            {
                HintAssist.SetHint(TagETextBox, settings.TagE);
            }
            else
            {
                HintAssist.SetHint(TagETextBox, "TagE");
            }

            if (!string.IsNullOrWhiteSpace(settings.TagF))
            {
                HintAssist.SetHint(TagFTextBox, settings.TagF);
            }
            else
            {
                HintAssist.SetHint(TagFTextBox, "TagF");
            }

        }

    }


}
