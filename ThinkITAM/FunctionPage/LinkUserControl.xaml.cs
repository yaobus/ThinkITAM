using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using IPAM_NOTE.ChildrenWindows.LinkWindows;
using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.FunctionClass;
using IPAM_NOTE.UserControls.LinkPage;
using IPAM_NOTE.UserControls.PortPanel;
using IPAM_NOTE.ViewModes.AssetManage;
using IPAM_NOTE.ViewModes.LinkManage;
using IPAM_NOTE.ViewModes.PortPanel;
using IPAM_NOTE.ViewModes.Preset;
using Nodify;
using static IPAM_NOTE.ViewModes.DevicePortManage.PortTypeClass;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;


namespace IPAM_NOTE.FunctionPage
{
    /// <summary>
    /// LinkUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class LinkUserControl : UserControl
    {
        public LinkUserControl()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        /// <summary>
        /// 链路是否可以保存
        /// </summary>
        private bool CanSave = false;

        private void LinkUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();


            CabinetListView.ItemsSource = rackInfos;

            RackPanel.ItemsSource = rackPanelItems;

            //建筑信息选择框
            BuildingsComboBox.ItemsSource = buildings;
            FloorComboBox.ItemsSource = floors;
            RoomComboBox.ItemsSource = roomNumbers;

            //设备分组信息框
            DeviceGroups.ItemsSource = groupsTypes;
            AssetsComboBox.ItemsSource = assets;

            //TODO 加载机房及配线间信息

            LoadDevicesTreeView();

            //加载建筑信息
            LoadBuildings();

            //加载设备分组
            LoadDevicesGroups();

            //订阅标签修改事件
            DataBridge.DataBridge.ModifyTagList.CollectionChanged += ModifyTagList_CollectionChanged;
            DataBridge.DataBridge.SelectRackId.CollectionChanged += SelectRackId_CollectionChanged;
            DataBridge.DataBridge.LinkManageList.CollectionChanged += PermanentManageList_CollectionChanged;
            rackPanelItems.CollectionChanged += RackPanelItems_CollectionChanged;

            //订阅更新机架信息，当机架上的端口信息发生改变，则更新
            DataBridge.DataBridge.SelectUpdateRackId.CollectionChanged += SelectUpdateRackId_CollectionChanged;

            //订阅链路管理选择端口事件
            DataBridge.DataBridge.LinkManageSelectPorts.CollectionChanged += LinkManageSelectPorts_CollectionChanged;


            //DataBridge.DataBridge.FindPorts.CollectionChanged += FindPorts_CollectionChanged;

            RoutePanel.ItemsSource = DataBridge.DataBridge.LinkManageList;



        }


        private ObservableCollection<AssetTypeViewModel> groupsTypes = new ObservableCollection<AssetTypeViewModel>();

        /// <summary>
        /// 加载设备分组
        /// </summary>
        private void LoadDevicesGroups()
        {
            groupsTypes.Clear();

            string sqlTemp = $"SELECT COUNT(*) FROM Devices";

            var num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

            if (num > 0)
            {
                string query = "SELECT DISTINCT AssetType FROM Devices;";

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    AssetTypeViewModel asset = new AssetTypeViewModel();

                    asset.AssetType = reader["Assettype"].ToString();

                    groupsTypes.Add(asset);
                }
            }
        }

        /// <summary>
        /// 链路管理选择端口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkManageSelectPorts_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ////清空画布
            //RoutePanel.Children.Clear();


            //foreach (var item in DataBridge.DataBridge.LinkManageSelectPorts)
            //{
            //    item.PortClass.IsSelected = true;
            //}


            Console.WriteLine("节点总数:" + DataBridge.DataBridge.LinkManageSelectPorts.Count);

            //if (DataBridge.DataBridge.LinkManageSelectPorts.Count > 0)
            //{

            //    var info = DataBridge.DataBridge.LinkManageSelectPorts[0];

            //    info.PortClass.IsSelected = true;


            //}
        }



        /// <summary>
        /// 发生更新的机架ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectUpdateRackId_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (DataBridge.DataBridge.SelectUpdateRackId.Count > 0)
            {
                foreach (var rackId in DataBridge.DataBridge.SelectUpdateRackId)
                {

                    var itemToList = rackPanelItems.FirstOrDefault(r => r.RackInfo.RackId == rackId);

                    if (itemToList != null)
                    {
                        UpdateRackInfo(rackId);

                    }

                }

                DataBridge.DataBridge.SelectUpdateRackId.Clear();
            }



        }

        /// <summary>
        /// 画布中的机架总数发生改变的时候执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RackPanelItems_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ItemCount.Text = $"操作区设备总数:" + rackPanelItems.Count.ToString();
            CabinetListView.SelectedIndex = -1;
        }

        /// <summary>
        /// 链路管理选择端口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PermanentManageList_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int count = DataBridge.DataBridge.LinkManageList.Count;

            if (count > 0)
            {
                int index = 0;

                foreach (var item in DataBridge.DataBridge.LinkManageList)
                {
                    index++;
                    item.PortClass.NodeIndex = index;

                    //string type = item.MdfRackClass.RackId.Substring(0, 1);

                    //if (type == "2")
                    //{
                    //    item.PortClass.NodeIndex = 1;
                    //}
                    //else
                    //{
                    //    index++;
                    //    item.PortClass.NodeIndex = index;
                    //}




                }

                //if (AutoSaveLinkConfig.IsChecked == true)
                //{
                //    SaveRackLink();

                //}

            }
            else
            {
                //RoutePanel.Children.Clear();
            }



        }

        /// <summary>
        /// 链路路径解析
        /// </summary>
        private void RouteResolver(ObservableCollection<PortLinkClass> list)
        {








            //int count = list.Count;

            ////Console.WriteLine(count);

            //if (count > 0)
            //{
            //    if (NumberCheckClass.OddOrEven(count) == 1)
            //    {
            //        //奇数

            //        //创建起点端口
            //        PortOutIn port = new PortOutIn();

            //        var info = list[count - 1];


            //        //获取端口信息
            //        port.DataContext = GetLinkPagePortInfo(info);


            //        //port.DataContext = list[count - 1];

            //        RoutePanel.Children.Add(port);

            //        //创建连接线
            //        CableLine cable = new CableLine();
            //        RoutePanel.Children.Add(cable);

            //    }
            //    else
            //    {
            //        //偶数

            //        //创建终点端口
            //        PortInOut port2 = new PortInOut();


            //        var info = list[count - 1];


            //        //获取端口信息
            //        port2.DataContext = GetLinkPagePortInfo(info);



            //       // port2.DataContext = list[count - 1];

            //        RoutePanel.Children.Add(port2);

            //        //可以保存
            //        CanSave = true;
            //    }

            //}
            //else
            //{
            //    RoutePanel.Children.Clear();
            //    CanSave = false;
            //}


        }

        /// <summary>
        /// 通过端口信息获取并生成链路数据端口信息
        /// </summary>
        /// <param name="portLinkClass"></param>
        /// <returns></returns>
        private LinkPagePortInfoClass GetLinkPagePortInfo(PortLinkClass portLinkClass)
        {
            var info = new LinkPagePortInfoClass();

            string rackId = portLinkClass.PortClass.RackId;
            info.RackId = rackId;

            //获取配线架或者建筑的名称
            if (rackId.Substring(0, 1) == "8") //如果是建筑
            {
                string sql = $"SELECT Building FROM Buildings WHERE BuildingId='{rackId}'";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    info.RoomOrBuilding = reader["Building"].ToString();
                }

            }
            else
            {
                string sql = $"SELECT dr.Name AS RoomName, dc.Name AS CabinetName,r.RackName AS RackName\r\nFROM Racks r\r\nJOIN DeviceCabinet dc ON r.CabinetId = dc.CabinetId\r\nJOIN DeviceRoom dr ON dc.DeviceRoomQrId = dr.DeviceRoomQrId\r\nWHERE r.RackId = '{rackId}';";
                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    info.RoomOrBuilding = reader["RoomName"].ToString();
                    info.Cabinet = reader["CabinetName"].ToString();
                    info.RackName = reader["RackName"].ToString();
                }

                //从数据库获取端口信息



            }


            string slotOrFloor = portLinkClass.PortClass.SlotIndex;
            string room = portLinkClass.PortClass.Room;
            string port = portLinkClass.PortClass.PortIndex;

            info.SlotOrFloor = slotOrFloor;
            info.RoomNumber = room;
            info.PortId = port;

            return info;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectRackId_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (DataBridge.DataBridge.SelectRackId.Count > 0)
            {
                //Console.WriteLine($"loadRack{DataBridge.DataBridge.SelectRackId[0]}");
                dbClass.LoadSelectRackInfo(DataBridge.DataBridge.SelectRackId[0]);
            }

        }


        /// <summary>
        /// 标签修改后更新显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyTagList_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {


            UpdateRackInfo(DataBridge.DataBridge.SelectRackId[0]);



        }

        /// <summary>
        /// 更新画布上的机架信息
        /// </summary>
        /// <param name="rackId"></param>
        private void UpdateRackInfo(string rackId)
        {

            string sql = $"SELECT * FROM Racks WHERE RackId = '{rackId}'";

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            RackInfo rack = new RackInfo();

            while (reader.Read())
            {


                rack.rackId = reader["RackId"].ToString();
                rack.rackGroup = reader["RackGroup"].ToString();
                rack.rackName = reader["RackName"].ToString();
                rack.rackNote = reader["RackNote"].ToString();
                string infos = reader["SlotInfos"].ToString();

                var slotInfos = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SlotClass>>(infos);

                rack.slotInfos = slotInfos;

                rack.slotCount = Convert.ToInt32(reader["SlotCount"]);


            }



            LoadRackInfo(rack);
        }



        private async void LoadDevicesTreeView()
        {
            string query = "SELECT DISTINCT DeviceRoomQrId FROM DeviceCabinet;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                //取出机房ID
                string deviceRoomQrId = reader["DeviceRoomQrId"].ToString();

                //取出机房信息
                string sql = $"SELECT * FROM DeviceRoom  WHERE DeviceRoomQrId = '{deviceRoomQrId}'";

                SQLiteCommand command1 = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader1 = command1.ExecuteReader();

                if (reader1.Read())
                {
                    var info = new DeviceRoomClass();
                    info.Index = index;
                    info.Name = reader1["RoomName"].ToString();
                    info.Location = reader1["Location"].ToString();
                    info.User = reader1["User"].ToString();
                    info.UserPhone = reader1["UserPhone"].ToString();
                    info.Note = reader1["Note"].ToString();
                    info.DeviceRoomQrId = reader1["DeviceRoomQrId"].ToString();

                    var room = new UserControls.LinkPage.DeviceRoomInfo();

                    room.DataContext = info;


                    string sqlTemp = $"SELECT * FROM DeviceCabinet  WHERE DeviceRoomQrId = '{deviceRoomQrId}'";

                    SQLiteCommand command2 = new SQLiteCommand(sqlTemp, dbClass.connection);
                    SQLiteDataReader reader2 = command2.ExecuteReader();

                    int index2 = 0;


                    var deviceRoomItems = new TreeViewItem();

                    while (reader2.Read())
                    {
                        index2++;
                        var cabinet = new UserControls.LinkPage.CabinetUserControl();

                        var cabinetInfo = new CabinetClass();
                        cabinetInfo.Index = index2;
                        cabinetInfo.Name = reader2["CabinetName"].ToString();
                        cabinetInfo.CabinetId = reader2["CabinetId"].ToString();
                        cabinetInfo.Position = reader2["Position"].ToString();
                        cabinetInfo.Note = reader2["Note"].ToString();
                        cabinetInfo.DeviceRoomQrId = reader2["DeviceRoomQrId"].ToString();
                        cabinet.DataContext = cabinetInfo;

                        deviceRoomItems.Items.Add(cabinet);
                    }

                    deviceRoomItems.Header = room;
                    await Task.Delay(50);
                    LinkTreeView.Items.Add(deviceRoomItems);

                }






            }
        }








        /// <summary>
        /// 添加配线间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRoomButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddDeviceRoomWindow addDeviceRoomWindow = new AddDeviceRoomWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addDeviceRoomWindow.Owner = window;
            }



            if (addDeviceRoomWindow.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码


                //加载设备信息
                //LoadTags();
            }


        }






        /// <summary>
        /// 添加机柜/分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCabinetButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddGroupWindow add = new AddGroupWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码

            }
        }

        /// <summary>
        /// 加载机柜设备列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null)
            {
                var selectedNode = e.NewValue;

                if (selectedNode is CabinetUserControl) //如果是机柜信息
                {
                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    CabinetUserControl childNode = selectedNode as CabinetUserControl;

                    CabinetClass info = childNode.DataContext as CabinetClass;


                    string cabinetId = info.CabinetId;

                    LoadRacksInfos(cabinetId);

                }
                else if (selectedNode is TreeViewItem) //如果是机房信息
                {

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

        ObservableCollection<RackInfo> rackInfos = new ObservableCollection<RackInfo>();

        /// <summary>
        /// 加载机柜内的配线架信息
        /// </summary>
        /// <param name="cabinetId"></param>
        private void LoadRacksInfos(string cabinetId)
        {

            rackInfos.Clear();

            string query = $"SELECT * FROM Racks WHERE CabinetId='{cabinetId}'";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                RackInfo info = new RackInfo();
                info.Index = index;
                info.rackId = reader["RackId"].ToString();
                info.rackGroup = reader["RackGroup"].ToString();
                info.rackName = reader["RackName"].ToString();
                info.rackNote = reader["RackNote"].ToString();
                string infos = reader["SlotInfos"].ToString();




                var slotInfos = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SlotClass>>(infos);
                info.slotInfos = slotInfos;

                info.slotCount = Convert.ToInt32(reader["SlotCount"]);

                rackInfos.Add(info);


            }
        }

        /// <summary>
        /// 通过RackId加载配线架信息
        /// </summary>
        /// <param name="rackId"></param>
        /// <returns></returns>
        private MdfRackClass LoadRackForRackId(string rackId)
        {
            MdfRackClass mdf = new MdfRackClass();

            string query = $"SELECT * FROM Racks WHERE RackId='{rackId}'";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {

                mdf.RackId = reader["RackId"].ToString();
                mdf.RackGroup = reader["RackGroup"].ToString();
                mdf.RackName = reader["RackName"].ToString();
                mdf.RackNote = reader["RackNote"].ToString();
                string infos = reader["SlotInfos"].ToString();


                var slotInfos = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SlotClass>>(infos);
                mdf.Slots = slotInfos;

                mdf.SlotCount = Convert.ToInt32(reader["SlotCount"]);

            }

            return mdf;
        }


        /// <summary>
        /// 添加配线架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRackButton_OnClick(object sender, RoutedEventArgs e)
        {
            RackCreateGuideWindow add = new RackCreateGuideWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码

            }

        }


        /// <summary>
        /// 加载配线架信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CabinetListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CabinetListView.SelectedItem != null)
            {
                var info = CabinetListView.SelectedItem as RackInfo;

                DataBridge.DataBridge.SelectRackInfo = info;


                //RackPanel.Items.Clear();

                LoadRackInfo(info);

            }

        }


        private void LoadRackInfo(RackInfo rackInfo)
        {
            string rackId = rackInfo.rackId;

            //第一步，获取机架信息
            MdfRackClass mdfRack = new MdfRackClass();
            mdfRack.RackId = rackId;
            mdfRack.RackGroup = rackInfo.rackGroup;
            mdfRack.RackName = rackInfo.rackName;
            mdfRack.RackNote = rackInfo.rackNote;
            mdfRack.SlotCount = rackInfo.slotCount;

            //第二步，获取槽位信息
            ObservableCollection<SlotClass> slots = rackInfo.slotInfos;

            int index = 0;

            foreach (var slot in slots)
            {
                string slotIndex = slot.SlotIndex;

                ObservableCollection<PortClass> ports = new ObservableCollection<PortClass>();

                //2.1，读取该槽位全部号信息
                string query = $"SELECT * FROM ra_{rackId} WHERE SlotId = {slotIndex};";

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PortClass port = new PortClass();

                    //端口基础信息
                    port.UID = Convert.ToInt32(reader["UID"]);
                    port.PortIndex = reader["PortId"].ToString();

                    //如果获取到的颜色为空，则设置为默认颜色
                    if (reader["PortColor"] == DBNull.Value)
                    {
                        port.PortColor = 0;
                    }
                    else
                    {
                        port.PortColor = Convert.ToInt32(reader["PortColor"]);
                    }


                    port.PortTag = reader["PortTag"].ToString();
                    port.PortStatus = reader["PortStatus"].ToString();
                    port.PortType = slots[index].SlotType;


                    if (reader["OnTheLine"] == DBNull.Value)
                    {
                        port.OnTheLine = -1;
                    }
                    else
                    {
                        port.OnTheLine = Convert.ToInt32(reader["OnTheLine"]);
                    }



                    ports.Add(port);
                }

                //类型索引
                index++;

                slot.Ports = ports;

            }


            mdfRack.Slots = slots;


            Rack rack = new Rack()
            {
                RackInfo = mdfRack,
            };



            AddNodeToRackPanel(rack);


        }



        ObservableCollection<Rack> rackPanelItems = new ObservableCollection<Rack>();


        private void AddNodeToRackPanel(Rack node)
        {
            if (rackPanelItems.Count > 0)
            {
                int status = 0;

                for (int i = 0; i < rackPanelItems.Count; i++)
                {
                    var item = rackPanelItems[i];

                    MdfRackClass rackInfo = item.RackInfo;

                    if (rackInfo.RackId == node.RackInfo.RackId)
                    {
                        node.Location = rackPanelItems[i].Location;

                        // 修改当前项
                        rackPanelItems[i] = node;
                        status = 1;
                        break;
                    }
                }

                if (status == 0)
                {

                    var rack = rackPanelItems[rackPanelItems.Count - 1];

                    double height = rack.ActualHeight;

                    Point point = new Point(rack.Location.X, rack.Location.Y + height + 5);

                    node.Location = point;

                    rackPanelItems.Add(node);
                }


            }
            else
            {
                rackPanelItems.Add(node);
            }


        }



        /// <summary>
        /// 重置画布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearRackPanel_OnClick(object sender, RoutedEventArgs e)
        {
            rackPanelItems.Clear();
            DataBridge.DataBridge.SelectRackId.Clear();
            CabinetListView.SelectedIndex = -1;
            DataBridge.DataBridge.LinkManageList.Clear();
            //RoutePanel.Children.Clear();
        }



        /// <summary>
        /// 链路预览模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TempManage_OnClick(object sender, RoutedEventArgs e)
        {

            DataBridge.DataBridge.LinkManageMode = 0;
            //DataBridge.DataBridge.PermanentManageList.Clear();
            //RoutePanel.Children.Clear();
        }



        /// <summary>
        /// 链路管理模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkManage_OnClick(object sender, RoutedEventArgs e)
        {
            DataBridge.DataBridge.LinkManageMode = 1;
            //DataBridge.DataBridge.PermanentManageList.Clear();
            //RoutePanel.Children.Clear();
        }

        /// <summary>
        /// 链路清除模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LinkClear_OnClick(object sender, RoutedEventArgs e)
        {
            DataBridge.DataBridge.LinkManageMode = 3;
            DataBridge.DataBridge.LinkPerClear = 0;
            DataBridge.DataBridge.LinkTempClear = 0;
            DataBridge.DataBridge.LinkManageList.Clear();
            //RoutePanel.Children.Clear();
        }

        /// <summary>
        /// 清空已选端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ClearSelect_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataBridge.DataBridge.LinkManageList)
            {
                item.PortClass.IsSelected = false;
                item.PortClass.NodeIndex = 0;
            }


            DataBridge.DataBridge.LinkManageList.Clear();
            //RoutePanel.Children.Clear();
        }



        /// <summary>
        /// 保存链路配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveLinkConfig_OnClick(object sender, RoutedEventArgs e)
        {
            if (CanSave = true)
            {
                SaveRackLink();

            }
        }


        /// <summary>
        /// 保存机架链路信息
        /// </summary>
        private void SaveRackLink()
        {
            int count = DataBridge.DataBridge.LinkManageList.Count;
            var list = DataBridge.DataBridge.LinkManageList;

            for (int i = 0; i < count; i++)
            {
                //List 索引从0开始
                if (i < count - 1)
                {
                    if (NumberCheckClass.OddOrEven(i) == 0)
                    {
                        //A端
                        var linkA = list[i];
                        string rackIdA = linkA.MdfRackClass.RackId;
                        string slotIdA = linkA.SlotClass.SlotIndex;
                        string portIdA = linkA.PortClass.PortIndex.ToString();
                        string roomA = linkA.PortClass.Room;
                        int typeA = Convert.ToInt32(rackIdA.Substring(0, 1));

                        Console.WriteLine("ROOMA:" + roomA);

                        int statusA = 0;//如果是0，则表示是RACK，否则是Building

                        string tableHeaderA = "ra";

                        if (rackIdA.Substring(0, 1) == "8")
                        {
                            tableHeaderA = "bu";
                            statusA = 1;
                        }



                        //B端
                        var linkB = list[i + 1];
                        string rackIdB = linkB.MdfRackClass.RackId;
                        string slotIdB = linkB.SlotClass.SlotIndex;
                        string portIdB = linkB.PortClass.PortIndex.ToString();
                        string roomB = linkB.PortClass.Room;
                        int typeB = Convert.ToInt32(rackIdB.Substring(0, 1));
                        int statusB = 0;//如果是0，则表示是RACK，否则是Building
                        string tableHeaderB = "ra";

                        Console.WriteLine("ROOMB:" + roomB);

                        if (rackIdB.Substring(0, 1) == "8")
                        {
                            tableHeaderB = "bu";
                            statusB = 1;
                        }


                        if (statusA == 1)
                        {
                            string roomSql = "";

                            if (roomB.Length > 0)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomB}'";
                            }

                            //往A端的表存入B端的信息(建筑端口类)
                            string sqlA =
                                $"UPDATE \"{tableHeaderA}_{rackIdA}\" SET \"PermanentType\" = {typeB}, \"PermanentRackId\" = '{rackIdB}', \"PermanentSlot\" = '{slotIdB}', \"PermanentPort\" = '{portIdB}' {roomSql} WHERE SlotId='{slotIdA}' AND PortId ='{portIdA}' AND RoomId='{roomA}'";
                            dbClass.ExecuteQuery(sqlA);

                            ReLoadRack(rackIdA);

                        }
                        else
                        {
                            string roomSql = "";

                            if (roomB.Length > 0)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomB}'";
                            }


                            //往A端的表存入B端的信息(配线架端口类)
                            string sqlA =
                                $"UPDATE \"{tableHeaderA}_{rackIdA}\" SET \"PermanentType\" = {typeB}, \"PermanentRackId\" = '{rackIdB}', \"PermanentSlot\" = '{slotIdB}', \"PermanentPort\" = '{portIdB}'  {roomSql}  WHERE SlotId='{slotIdA}' AND PortId ='{portIdA}'";
                            dbClass.ExecuteQuery(sqlA);

                            ReLoadRack(rackIdA);

                        }


                        if (statusB == 1)
                        {

                            string roomSql = "";

                            if (roomA != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomA}'";
                            }


                            //往B端的表存入A端的信息(建筑端口类)
                            string sqlB =
                                $"UPDATE \"{tableHeaderB}_{rackIdB}\" SET \"PermanentType\" = {typeA}, \"PermanentRackId\" = '{rackIdA}', \"PermanentSlot\" = '{slotIdA}', \"PermanentPort\" = '{portIdA}' {roomSql} WHERE SlotId='{slotIdB}' AND PortId ='{portIdB}' AND RoomId='{roomB}'";

                            dbClass.ExecuteQuery(sqlB);
                            ReLoadRack(rackIdB);

                        }
                        else
                        {

                            string roomSql = "";

                            if (roomA != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomA}'";
                            }

                            //往B端的表存入A端的信息(配线架端口类)
                            string sqlB =
                                $"UPDATE \"{tableHeaderB}_{rackIdB}\" SET \"PermanentType\" = {typeA}, \"PermanentRackId\" = '{rackIdA}', \"PermanentSlot\" = '{slotIdA}', \"PermanentPort\" = '{portIdA}'  {roomSql}  WHERE SlotId='{slotIdB}' AND PortId ='{portIdB}'";

                            dbClass.ExecuteQuery(sqlB);
                            ReLoadRack(rackIdB);
                        }



                    }
                    else
                    {

                        //A端
                        var linkA = list[i];
                        string rackIdA = linkA.MdfRackClass.RackId;
                        string slotIdA = linkA.SlotClass.SlotIndex;
                        string portIdA = linkA.PortClass.PortIndex.ToString();
                        string roomA = linkA.PortClass.Room;
                        int typeA = Convert.ToInt32(rackIdA.Substring(0, 1));

                        Console.WriteLine("ROOMA:" + roomA);

                        string tableHeaderA = "ra";
                        int statusA = 0;//如果是0，则表示是RACK，否则是Building

                        if (rackIdA.Substring(0, 1) == "8")
                        {
                            tableHeaderA = "bu";
                            statusA = 1;
                        }

                        //B端
                        var linkB = list[i + 1];
                        string rackIdB = linkB.MdfRackClass.RackId;
                        string slotIdB = linkB.SlotClass.SlotIndex;
                        string portIdB = linkB.PortClass.PortIndex.ToString();
                        string roomB = linkB.PortClass.Room;
                        int typeB = Convert.ToInt32(rackIdB.Substring(0, 1));

                        Console.WriteLine("ROOMB:" + roomB);

                        string tableHeaderB = "ra";
                        int statusB = 0;//如果是0，则表示是RACK，否则是Building

                        if (rackIdB.Substring(0, 1) == "8")
                        {
                            tableHeaderB = "bu";
                            statusB = 1;
                        }


                        if (statusA == 1)
                        {

                            string roomSql = "";

                            if (roomB != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomB}'";
                            }

                            //往A端的表存入B端的信息
                            string sqlA =
                                $"UPDATE \"{tableHeaderA}_{rackIdA}\" SET \"TempType\" = {typeB}, \"TempRackId\" = '{rackIdB}', \"TempSlot\" = '{slotIdB}', \"TempPort\" = '{portIdB}' {roomSql} WHERE SlotId='{slotIdA}' AND PortId ='{portIdA}' AND RoomId='{roomA}'";
                            dbClass.ExecuteQuery(sqlA);
                            ReLoadRack(rackIdA);
                        }
                        else
                        {

                            string roomSql = "";

                            if (roomB != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomB}'";
                            }

                            //往A端的表存入B端的信息
                            string sqlA =
                                $"UPDATE \"{tableHeaderA}_{rackIdA}\" SET \"TempType\" = {typeB}, \"TempRackId\" = '{rackIdB}', \"TempSlot\" = '{slotIdB}', \"TempPort\" = '{portIdB}' {roomSql} WHERE SlotId='{slotIdA}' AND PortId ='{portIdA}' ";
                            dbClass.ExecuteQuery(sqlA);
                            ReLoadRack(rackIdA);
                        }

                        if (statusB == 1)
                        {

                            string roomSql = "";

                            if (roomA != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomA}'";
                            }


                            //往B端的表存入A端的信息
                            string sqlB =
                                $"UPDATE \"{tableHeaderB}_{rackIdB}\" SET \"TempType\" = {typeA}, \"TempRackId\" = '{rackIdA}', \"TempSlot\" = '{slotIdA}', \"TempPort\" = '{portIdA}' {roomSql} WHERE SlotId='{slotIdB}' AND PortId ='{portIdB}' AND RoomId='{roomB}'";
                            dbClass.ExecuteQuery(sqlB);
                            ReLoadRack(rackIdB);
                        }
                        else
                        {

                            string roomSql = "";

                            if (roomA != null)
                            {
                                roomSql = $" ,\"PermanentRoom\" = '{roomA}'";
                            }


                            //往B端的表存入A端的信息
                            string sqlB =
                                $"UPDATE \"{tableHeaderB}_{rackIdB}\" SET \"TempType\" = {typeA}, \"TempRackId\" = '{rackIdA}', \"TempSlot\" = '{slotIdA}', \"TempPort\" = '{portIdA}' {roomSql} WHERE SlotId='{slotIdB}' AND PortId ='{portIdB}'";
                            dbClass.ExecuteQuery(sqlB);
                            ReLoadRack(rackIdB);
                        }



                    }




                }
                else
                {
                    break;
                }

            }


        }

        /// <summary>
        /// 重新加载机架
        /// </summary>
        private void ReLoadRack(string rackId)
        {
            if (rackId.Substring(0, 1) != "8")
            {
                string sql = $"SELECT * FROM Racks WHERE RackId = '{rackId}'";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                RackInfo rack = new RackInfo();

                while (reader.Read())
                {


                    rack.rackId = reader["RackId"].ToString();
                    rack.rackGroup = reader["RackGroup"].ToString();
                    rack.rackName = reader["RackName"].ToString();
                    rack.rackNote = reader["RackNote"].ToString();
                    string infos = reader["SlotInfos"].ToString();

                    var slotInfos = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SlotClass>>(infos);

                    rack.slotInfos = slotInfos;

                    rack.slotCount = Convert.ToInt32(reader["SlotCount"]);


                }

                //RackPanel.Items.Clear();

                LoadRackInfo(rack);
            }
            else
            {
                RoomComboBox_OnSelectionChanged(null, null);
            }



        }

        /// <summary>
        /// 删除选中的机架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeleteSelectedRack_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataBridge.DataBridge.SelectRackId.Count > 0)
            {
                var itemToRemove =
                    rackPanelItems.FirstOrDefault(r => r.RackInfo.RackId == DataBridge.DataBridge.SelectRackId[0]);

                if (itemToRemove != null)
                {
                    rackPanelItems.Remove(itemToRemove);
                    DataBridge.DataBridge.SelectRackId.Clear();
                    //DataBridge.DataBridge.SelectRackInfo = null;
                }

            }



        }

        /// <summary>
        /// 永久链路清除被勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerLink_OnClick(object sender, RoutedEventArgs e)
        {
            if (PerLink.IsChecked == true)
            {
                DataBridge.DataBridge.LinkPerClear = 1;
            }
            else
            {
                DataBridge.DataBridge.LinkPerClear = 0;
            }


        }

        /// <summary>
        /// 临时链路清除被勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TempLink_OnClick(object sender, RoutedEventArgs e)
        {
            if (TempLink.IsChecked == true)
            {
                DataBridge.DataBridge.LinkTempClear = 1;
            }
            else
            {
                DataBridge.DataBridge.LinkTempClear = 0;
            }
        }


        /// <summary>
        /// 建筑信息列表
        /// </summary>
        private ObservableCollection<BuildingInfoClass> buildings = new ObservableCollection<BuildingInfoClass>();
        /// <summary>
        /// 加载建筑信息
        /// </summary>
        private void LoadBuildings()
        {
            buildings.Clear();
            floors.Clear();
            roomNumbers.Clear();

            //第一步：读取所有建筑信息
            string query = "SELECT *  FROM  Buildings;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;

                var info = new BuildingInfoClass();

                info.Index = index;

                string buildingId = reader["BuildingId"].ToString();

                info.BuildingId = buildingId;



                //取出建筑名称
                info.Building = reader["Building"].ToString();

                info.Address = reader["Address"].ToString();
                info.User = reader["User"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();


                buildings.Add(info);

            }
        }


        /// <summary>
        /// 楼层信息列表
        /// </summary>
        private ObservableCollection<FloorInfoClass> floors = new ObservableCollection<FloorInfoClass>();
        /// <summary>
        /// 加载楼层信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildingsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (BuildingsComboBox.SelectedIndex != -1)
            {
                floors.Clear();
                roomNumbers.Clear();


                string buildingId = buildings[BuildingsComboBox.SelectedIndex].BuildingId;
                DataBridge.DataBridge.SelectedBuildingId = buildingId;


                dbClass.CreateDynamicsTableIfNotExists($"{buildingId}", 1);

                //取出建筑楼层信息
                string sql = $"SELECT DISTINCT SlotId FROM bu_{buildingId}";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                int index2 = 0;
                while (reader.Read())
                {
                    index2++;
                    var floorClass = new FloorInfoClass();

                    floorClass.Index = index2;
                    floorClass.Floor = reader["SlotId"].ToString();
                    floorClass.BuildingId = buildingId;

                    floors.Add(floorClass);

                }


            }




        }


        private ObservableCollection<RoomClass> roomNumbers = new ObservableCollection<RoomClass>();


        /// <summary>
        /// 加载房间信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FloorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FloorComboBox.SelectedIndex != -1)
            {
                roomNumbers.Clear();

                string buildingId = buildings[BuildingsComboBox.SelectedIndex].BuildingId;
                string floor = floors[FloorComboBox.SelectedIndex].Floor;
                DataBridge.DataBridge.SelectedSlotId = floor;

                LoadRooms(buildingId, floor);
            }


        }



        private void LoadRooms(string buildingId, string floor)
        {
            string sql = $"SELECT DISTINCT RoomId FROM Bu_{buildingId} WHERE  SlotId ='{floor}'";

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;

                RoomClass room = new RoomClass();

                room.Index = index;
                room.RoomNumber = reader["RoomId"].ToString();

                //string roomNote = reader["RoomNote"].ToString();

                //room.RoomNote = !string.IsNullOrEmpty(roomNote)
                //    ? roomNote
                //    : "";

                roomNumbers.Add(room);


            }

        }

        /// <summary>
        /// 加载对应的房间端口信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            PortManagePanel.Children.Clear();

            if (RoomComboBox.SelectedIndex != -1)
            {
                string buildingId = buildings[BuildingsComboBox.SelectedIndex].BuildingId;

                string floor = floors[FloorComboBox.SelectedIndex].Floor;

                string room = roomNumbers[RoomComboBox.SelectedIndex].RoomNumber;


                string sql =
                    $"SELECT * FROM Bu_{buildingId} WHERE  SlotId ='{floor}' AND RoomId='{room}'";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int index = 0;

                while (reader.Read())
                {
                    index++;

                    PortLinkClass p = new PortLinkClass();

                    PortClass info = new PortClass();



                    if (reader["OnTheLine"] == DBNull.Value)
                    {
                        info.OnTheLine = -1;
                    }
                    else
                    {
                        info.OnTheLine = Convert.ToInt32(reader["OnTheLine"]);
                    }



                    info.UID = Convert.ToInt32(reader["UID"].ToString());
                    info.RackId = buildingId;
                    info.PortType = reader["PortType"].ToString();
                    info.PortIndex = reader["PortId"].ToString();
                    info.PortTag = reader["PortTag"].ToString();
                    info.SlotIndex = reader["SlotId"].ToString();
                    info.Room = room;


                    SlotClass slot = new SlotClass();
                    slot.SlotIndex = reader["SlotId"].ToString();

                    info.PortColor = Convert.ToInt32(reader["PortColor"].ToString());

                    p.PortClass = info;
                    p.SlotClass = slot;
                    LinkPanelPort port = new LinkPanelPort();

                    port.Margin = new Thickness(5);
                    port.DataContext = p;

                    PortManagePanel.Children.Add(port);
                }
            }

        }

        private ObservableCollection<DeviceTypeViewModel> assets = new ObservableCollection<DeviceTypeViewModel>();
        private void DeviceGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = DeviceGroups.SelectedIndex;

            if (index != -1)
            {
                assets.Clear();
                string asset = groupsTypes[index].AssetType;

                string sqlTemp = $"SELECT * FROM Devices  WHERE AssetType = '{asset}'";

                SQLiteCommand command = new SQLiteCommand(sqlTemp, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var info = new DeviceTypeViewModel();
                    info.AssetId = reader["AssetId"].ToString();
                    info.AssetType = reader["AssetType"].ToString();
                    info.DeviceType = reader["DeviceType"].ToString();
                    info.Description = reader["Description"].ToString();
                    info.Model = reader["Model"].ToString();
                    info.AssetNumber = reader["AssetNumber"].ToString();


                    assets.Add(info);
                }

            }
        }

        /// <summary>
        /// 选择设备后，加载设备端口信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssetsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = AssetsComboBox.SelectedIndex;

            if (index != -1)
            {
                string assetId = assets[index].AssetId;

                if (!string.IsNullOrWhiteSpace(assetId))
                {
                    LoadDevicePortInfos(assetId);
                }

                //DevicePortPanel.ItemsSource = devicePorts;
            }



        }


        //private ObservableCollection<PortDetailedInfo> devicePorts=  new ObservableCollection<PortDetailedInfo>();

        /// <summary>
        /// 加载端口信息
        /// </summary>
        /// <param name="tableName"></param>
        private void LoadDevicePortInfos(string assetId)
        {
            //devicePorts.Clear();
            DevicePortPanel.Children.Clear();

            string tableName = $"De_{assetId}";

            if (tableName != "" && tableName != null)
            {
                string query = $"SELECT * FROM {tableName};";

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                #region 

                //while (reader.Read())
                //{
                //    //读取端口信息，并写入列表
                //    var info = new PortDetailedInfo();
                //    info.UID = Convert.ToInt32(reader["UID"]);
                //    info.PortType = reader["PortType"].ToString();
                //    info.PortTag = reader["PortTag"].ToString();
                //    info.PortSlotNumber = Convert.ToInt32(reader["PortSlotNumber"]);
                //    info.PortId = reader["PortId"].ToString();
                //    info.Status = Convert.ToInt32(reader["Status"].ToString());
                //    info.Mode = reader["Mode"].ToString();
                //    info.PortName = reader["PortName"].ToString();
                //    info.VlanId = reader["VlanId"].ToString();

                //    info.FullPortId= $"{info.PortSlotNumber}{info.PortId}";


                //    //如果颜色索引数据库返回值为空数据，则使用默认颜色

                //    if (reader["PortColor"] == DBNull.Value)
                //    {
                //        info.PortColor = 0;
                //    }
                //    else
                //    {
                //        info.PortColor = Convert.ToInt32(reader["PortColor"]);
                //    }


                //    if (reader["OnTheLine"] == DBNull.Value)
                //    {
                //        info.OnTheLine = -1;
                //    }
                //    else
                //    {
                //        info.OnTheLine = Convert.ToInt32(reader["OnTheLine"]);
                //    }


                //    info.TagA = reader["TagA"].ToString();
                //    info.TagB = reader["TagB"].ToString();
                //    info.TagC = reader["TagC"].ToString();
                //    info.TagD = reader["TagD"].ToString();
                //    info.TagE = reader["TagE"].ToString();
                //    info.TagF = reader["TagF"].ToString();
                //    info.AssetId = reader["AssetId"].ToString();



                //    devicePorts.Add(info);

                //}
                #endregion

                int index = 0;

                while (reader.Read())
                {
                    string portType = reader["PortType"].ToString();



                    if (portType == "D")
                    {
                        return;
                    }

                    index++;

                    PortLinkClass p = new PortLinkClass();

                    PortClass info = new PortClass();


                    if (reader["OnTheLine"] == DBNull.Value)
                    {
                        info.OnTheLine = -1;
                    }
                    else
                    {
                        info.OnTheLine = Convert.ToInt32(reader["OnTheLine"]);
                    }



                    info.UID = Convert.ToInt32(reader["UID"].ToString());
                    info.RackId = assetId;


                    int slotNumber = Convert.ToInt32(reader["PortSlotNumber"]);




                    string portId = reader["PortId"].ToString();

                    #region MyRegion

                    //switch (portType)
                    //{
                    //    case "E":
                    //        info.PortType = "Eth";
                    //        break;
                    //    case "M":
                    //        info.PortType = "Eth";
                    //        break;
                    //    default:
                    //        info.PortType = "FC";  
                    //        break;
                    //}
                    #endregion

                    info.PortType = reader["PortType"].ToString();
                    info.PortIndex = $"{slotNumber}{portId}";
                    info.PortTag = reader["PortTag"].ToString();
                    info.SlotIndex = slotNumber.ToString();



                    SlotClass slot = new SlotClass();

                    slot.SlotIndex = reader["PortSlotNumber"].ToString();


                    if (reader["PortColor"] == DBNull.Value)
                    {
                        info.PortColor = 0;
                    }
                    else
                    {
                        info.PortColor = Convert.ToInt32(reader["PortColor"].ToString());
                    }



                    p.PortClass = info;
                    p.SlotClass = slot;

                    LinkDevicePort port = new LinkDevicePort();

                    port.Margin = new Thickness(5);

                    port.DataContext = p;

                    DevicePortPanel.Children.Add(port);
                }

            }

        }

    }
}
