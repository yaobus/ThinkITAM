using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.Computer;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.Others;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.Windows.Computer;
using ThinkITAM.Windows.DevicePortManage;
using ThinkITAM.Windows.NetworkManage;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// ComputerPagexaml.xaml 的交互逻辑
    /// </summary>
    public partial class ComputerPage : UserControl
    {
        public ComputerPage()
        {
            InitializeComponent();
            ComputerListView.ItemsSource = portNumbers;
            DataBridge.DataBridge.modifyDeployDevices.CollectionChanged += ModifyDeployDevices_CollectionChanged;
        }

        private void ModifyDeployDevices_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PortManagePanel.Items.Clear();

            if (RoomListView.SelectedIndex != -1)
            {

                var rows = GlobalVariables.DbService.ExecuteQuery(lastSql);


                int index = 0;

                foreach (var row in rows)
                {
                    index++;

                    PortClass portClass = new PortClass();
                    portClass.Index = index;
                    portClass.UID = Convert.ToInt32(row["UID"]);
                    portClass.DeviceId = row["DeviceId"].ToString();
                    portClass.AssetId = row["AssetId"].ToString();
                    portClass.PortType = row["PortType"].ToString();
                    portClass.PortIndex = row["PortId"].ToString();
                    portClass.PortTag = row["PortTag"].ToString();
                    portClass.Room = row["Room"].ToString();
                    portClass.AssetNumber = $"{row["AssetTag"]}{row["AssetNumber"]}";
                    portClass.UserName = row["Name"].ToString();

                    if (row["PortColor"] == string.Empty)
                    {
                        portClass.PortColor = 0;
                    }
                    else
                    {
                        portClass.PortColor = Convert.ToInt32(row["PortColor"]);
                    }



                    if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                    {
                        portClass.OnTheLine = -1;
                    }
                    else
                    {
                        portClass.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                    }



                    DeviceNode port = new DeviceNode();

                    port.Margin = new Thickness(10);
                    port.DataContext = portClass;

                    PortManagePanel.Items.Add(port);
                }


            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var newWindow = new AddComputerWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                newWindow.Owner = window;
            }



            if (newWindow.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                LoadTreeList();

                //加载设备信息
                //LoadTags();
            }
        }

        private void ComputerPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadTreeList();
            LoadTags();
            RoomListView.ItemsSource = roomNumbers;

        }


        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {

            var tags = DbClass.LoadWindowTag("ComputerTag");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                TagA.Content = settings.TagA;
                TagB.Content = settings.TagB;
                TagC.Content = settings.TagC;
                TagD.Content = settings.TagD;
                TagE.Content = settings.TagE;
                TagF.Content = settings.TagF;
            }

        }



        private void LoadTreeList(string keyWord = null)
        {
            BuildingTreeView.Items.Clear();

            string filter = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                filter = $"WHERE ( Building LIKE '%{keyWord}%' OR Address LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%' )  AND (Del != 1 OR Del IS NULL) ";
            }
            else
            {
                filter = "WHERE (Del != 1 OR Del IS NULL)";
            }

            //第一步：读取所有建筑信息
            string query = $"SELECT *  FROM  Buildings {filter};";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;

                var info = new BuildingInfoClass();
                var building = new UserControls.Computer.DeviceBuilding();
                info.Index = index;

                string buildingId = row["BuildingId"].ToString();



                info.BuildingId = buildingId;

                DbClass.CreateDynamicsTableIfNotExists(buildingId, 1);

                //取出建筑名称
                info.Building = row["Building"].ToString();
                info.Address = row["Address"].ToString();
                info.User = row["User"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();
                info.Count = DbClass.StatisticsDeviceRoomFloor(buildingId);

                building.DataContext = info;

                //取出建筑楼层信息
                string sql = $"SELECT DISTINCT SlotId FROM Bu_{buildingId}";


                var rows1 = GlobalVariables.DbService.ExecuteQuery(sql);


                int index2 = 0;

                var buildingItem = new TreeViewItem();

                foreach (var row1 in rows1)
                {
                    index2++;

                    var floorClass = new FloorInfoClass();

                    var floor = new UserControls.Computer.DeviceFloor();

                    floorClass.Index = index2;
                    floorClass.Floor = row1["SlotId"].ToString();
                    floorClass.BuildingId = info.BuildingId;


                    floorClass.RoomCount = DbClass.GetRoomForFloorCount(floorClass.BuildingId, floorClass.Floor);
                    floorClass.PortCount = DbClass.GetDeviceForFloorCount(floorClass.BuildingId, floorClass.Floor);

                    floor.DataContext = floorClass;



                    buildingItem.Items.Add(floor);
                }




                buildingItem.Header = building;

                BuildingTreeView.Items.Add(buildingItem);
            }



        }


        private void BuildingTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            roomNumbers.Clear();

            if (e != null)
            {
                AddButton.IsEnabled = true;

                var selectedNode = e.NewValue;

                if (selectedNode is UserControls.Computer.DeviceFloor) //如果是楼层信息
                {
                    DeleteBuildingButton.IsEnabled = false;
                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    var childNode = selectedNode as UserControls.Computer.DeviceFloor;

                    //获取到的楼层信息
                    FloorInfoClass info = childNode.DataContext as FloorInfoClass;

                    DataBridge.DataBridge.SelectBuildingId = info.BuildingId;

                    DataBridge.DataBridge.SelectFloor = info.Floor;

                    //加载房间号

                    LoadRooms(info.BuildingId, info.Floor);



                }
                else if (selectedNode is TreeViewItem) //如果是建筑信息
                {
                    DeleteBuildingButton.IsEnabled = true;
                    TreeViewItem selectedItem = selectedNode as TreeViewItem;

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


                    if (selectedNode != null)
                    {
                        //获取建筑ID
                        var data = selectedItem.Header as UserControls.Computer.DeviceFloor;

                        if (data != null)
                        {

                            BuildingInfoClass info = data.DataContext as BuildingInfoClass;


                            DataBridge.DataBridge.SelectBuildingId = info.BuildingId;




                        }


                    }

                }

            }
        }

        private ObservableCollection<RoomClass> roomNumbers = new ObservableCollection<RoomClass>();

        private void LoadRooms(string buildingId, string floor)
        {
            string sql = $"SELECT DISTINCT Room FROM Computer WHERE BuildingId= '{buildingId}' AND Floor ='{floor}'";


            Console.WriteLine(sql);

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                index++;

                RoomClass room = new RoomClass();

                room.Index = index;
                room.RoomNumber = row["Room"].ToString();

                room.PortCount = DbClass.GetRoomDeviceCount(buildingId, floor, room.RoomNumber);


                string noteId = $"{buildingId}{floor}{room.RoomNumber}";
                //根据Id查询备注

                string note = DbClass.LoadNote(noteId);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    room.RoomNote = note;
                }
                else
                {
                    room.RoomNote = "";
                }

                roomNumbers.Add(room);
            }



        }
        /// <summary>
        /// 上一次执行的sql
        /// </summary>
        private string lastSql = string.Empty;
        private ObservableCollection<PortClass>portNumbers = new ObservableCollection<PortClass>();

        private void RoomListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NumberBlock.Text = string.Empty;
            PortManagePanel.Items.Clear();
            portNumbers.Clear();

            if (RoomListView.SelectedIndex != -1)
            {
                RoomClass room = RoomListView.SelectedItem as RoomClass;

                DataBridge.DataBridge.SelectRoom = room.RoomNumber;

                string sql = $"SELECT   c.*,   a.AssetTag,  a.AssetNumber,  u.Name FROM   Computer c JOIN   Asset a ON c.AssetId = a.AssetId  JOIN   UserInfo u ON c.AssetUser = u.UserId WHERE   c.BuildingId = '{DataBridge.DataBridge.SelectBuildingId}'   AND c.Floor = '{DataBridge.DataBridge.SelectFloor}'  AND c.Room = '{DataBridge.DataBridge.SelectRoom}';";

                lastSql = sql;

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);


                int index = 0;

                foreach (var row in rows)
                {
                    index++;

                    PortClass portClass = new PortClass();
                    portClass.Index = index;
                    portClass.UID = Convert.ToInt32(row["UID"]);
                    portClass.DeviceId = row["DeviceId"].ToString();
                    portClass.AssetId = row["AssetId"].ToString();
                    portClass.AssetUser= row["AssetUser"].ToString();
                    portClass.PortType = row["PortType"].ToString();
                    portClass.PortIndex = row["PortId"].ToString();
                    portClass.PortTag = row["PortTag"].ToString();
                    portClass.PortStatus= row["PortStatus"].ToString();
                    portClass.PortGroup= row["PortGroup"].ToString();
                    portClass.BuildingId= row["BuildingId"].ToString();
                    portClass.SlotIndex=row["Floor"].ToString();
                    portClass.Room = row["Room"].ToString();
                    portClass.AssetNumber = $"{row["AssetTag"]}{row["AssetNumber"]}";
                    portClass.UserName = row["Name"].ToString();
                    portClass.LinkIp = row["LinkIp"].ToString();
                    portClass.TagA = row["TagA"].ToString();
                    portClass.TagB = row["TagB"].ToString();
                    portClass.TagC = row["TagC"].ToString();
                    portClass.TagD = row["TagD"].ToString();
                    portClass.TagE = row["TagE"].ToString();
                    portClass.TagF = row["TagF"].ToString();

                    if (row["PortColor"] == string.Empty)
                    {
                        portClass.PortColor = 0;
                    }
                    else
                    {
                        portClass.PortColor = Convert.ToInt32(row["PortColor"]);
                    }



                    if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                    {
                        portClass.OnTheLine = -1;
                    }
                    else
                    {
                        portClass.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                    }



                    DeviceNode port = new DeviceNode();

                    port.Margin = new Thickness(10);
                    port.DataContext = portClass;
                    portNumbers.Add(portClass);
                    PortManagePanel.Items.Add(port);
                }


            }

        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {


            if (!string.IsNullOrWhiteSpace(SearchKeyWord.Text))
            {
                LoadTreeList(SearchKeyWord.Text);
            }
            else
            {
                LoadTreeList();
            }
        }

        private void SearchKeyWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_OnClick(null, null);
            }


        }

        private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
        {
            SearchKeyWord.Text = null;
            LoadTreeList();
        }

        private void MultipleButton_OnClick(object sender, RoutedEventArgs e)
        {
            var items = portNumbers.Where(item => item.IsSelected == true);

            var newItems = new ObservableCollection<PortClass>(items);


            if (portNumbers.Count > 0)
            {
                var newWindow = new ComputerEditWindow(newItems);

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    newWindow.Owner = window;
                }

                newWindow.ShowDialog();

            }





        }

        private void ComputerListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void SelectToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {

            var items = portNumbers.Where(item => item.IsSelected == true);
           

            if (items.Count() > 0)
            {
                NumberBlock.Text = items.Count().ToString();
                MultipleButton.IsEnabled=true;
                MultipleDeleteButton.IsEnabled=true;
            }
            else
            {
                NumberBlock.Text ="0";
                MultipleButton.IsEnabled = false;
                MultipleDeleteButton.IsEnabled = false;
            }


        }

        private void ShowModeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ShowModeButton.IsChecked == true)
            {
                ComputerScrollViewer.Visibility = Visibility.Collapsed;
                ComputerListView.Visibility = Visibility.Visible;

                //LoadMode = 1;
                //OperationPanel.IsEnabled = false;

                //if (IpAddressInfoLists.Count > 0)
                //{
                //    GraphicalPlan.Visibility = Visibility.Collapsed;
                //    AddressListView.Visibility = Visibility.Visible;
                //}

            }
            else
            {
                ComputerScrollViewer.Visibility = Visibility.Visible;
                ComputerListView.Visibility = Visibility.Collapsed;

                //LoadMode = 0;
                //OperationPanel.IsEnabled = true;
                //GraphicalPlan.Visibility = Visibility.Visible;
                //AddressListView.Visibility = Visibility.Collapsed;

            }
        }

        private void MultipleDeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要删除吗？\r该操作不可恢复！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                int successCount = 0;
                int failCount = 0;
                foreach (var port in portNumbers)
                {
                    if (port.IsSelected == true)
                    {
                        if (port.OnTheLine <= 0)
                        {

                            var sql = $"DELETE FROM Computer WHERE UID = {port.UID}";
                            GlobalVariables.DbService.ExecuteNonQuery(sql);

                            sql = $"UPDATE Asset SET Deploy = NULL WHERE AssetId = '{port.AssetId}'";
                            GlobalVariables.DbService.ExecuteNonQuery(sql);

                            successCount++;

                        }
                        else
                        {
                            failCount++;
                        }
                    }

                }

                string message = string.Empty;
                if (failCount > 0)
                {
                    message = $"\r失败{failCount}个。\r失败原因:端口已在链路上！";
                }

                MessageBox.Show($"删除成功{successCount}个{message}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                RoomListView_OnSelectionChanged(null, null);
            }
        }


        private ObservableCollection<PortClass>infos=new ObservableCollection<PortClass>();
        private void ComputerListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComputerListView.SelectedIndex != -1)
            {
                infos.Clear();

                var info = portNumbers[ComputerListView.SelectedIndex];

                if (info != null)
                {
                    infos.Add(info);

                    var newWindow = new ComputerEditWindow(infos);

                    var window = Window.GetWindow(this);
                    if (window != null)
                    {
                        newWindow.Owner = window;
                    }

                    newWindow.ShowDialog();

                }
            }
        }

        /// <summary>
        /// 打开自定义字段设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {

           var set = new ComputerTagSetWindow();

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
        /// 加载端口信息自定义标签
        /// </summary>
        private void LoadCustomTag()
        {
            
            //加载端口自定义标签
            var tagWindow = "ComputerTag" ;

            string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{tagWindow}'";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);


            if (num > 0) //存在本地自定义标签
            {
                var tags = DbClass.LoadWindowTag(tagWindow);

                if (tags != null)
                {
                    dynamic settings = JsonConvert.DeserializeObject<TagViewModel>(tags);

                    TagA.Content = settings.TagA;
                    TagB.Content = settings.TagB;
                    TagC.Content = settings.TagC;
                    TagD.Content = settings.TagD;
                    TagE.Content = settings.TagE;
                    TagF.Content = settings.TagF;

                }

            }
           

        }

        private void DeleteBuildingButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要删除该建筑吗？该操作不可恢复！\r此操作将同步导致面板管理页面无法访问该建筑信息！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                var query = $"SELECT COUNT(OnTheLine) FROM Bu_{DataBridge.DataBridge.SelectBuildingId} WHERE OnTheLine > 0";

                int count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));


                if (count > 0)
                {
                    MessageBox.Show("该建筑物内有端口位于链路上，无法进行删除", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var sql = $"UPDATE Buildings SET Del = 1 WHERE BuildingId ='{DataBridge.DataBridge.SelectBuildingId}'";

                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    LoadTreeList();
                }



            }

        }
    }
}
