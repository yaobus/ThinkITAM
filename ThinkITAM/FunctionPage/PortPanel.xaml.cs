using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.PortPanel;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.Others;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.Windows.Computer;
using ThinkITAM.Windows.PortPanel;
using ThinkITAM.Windows.PresetWindows;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// PortPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PortPanel : UserControl
    {
        public PortPanel()
        {
            InitializeComponent();
            PanelPortListView.ItemsSource = portNumbers;
            DataBridge.DataBridge.modifyPorts.CollectionChanged += ModifyPorts_CollectionChanged;
        }

        /// <summary>
        /// 删除端口后刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyPorts_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            PortManagePanel.Items.Clear();

            if (RoomListView.SelectedIndex != -1)
            {
                PortManagePanel.Items.Clear();

                var rows = GlobalVariables.DbService.ExecuteQuery(lastSql);


                int index = 0;

                foreach (var row in rows)
                {
                    index++;

                    PortClass portClass = new PortClass();
                    portClass.AssetId = DataBridge.DataBridge.SelectBuildingId;
                    portClass.UID = Convert.ToInt32(row["UID"]);
                    portClass.PortType = row["PortType"].ToString();
                    portClass.PortIndex = row["PortId"].ToString();
                    portClass.PortTag = row["PortTag"].ToString();
                    portClass.Room = row["RoomId"].ToString();
                    portClass.PortColor = Convert.ToInt32(row["PortColor"]);



                    if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                    {
                        portClass.OnTheLine = -1;
                    }
                    else
                    {
                        portClass.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                    }



                    PanelPort port = new PanelPort();

                    port.Margin = new Thickness(10);
                    port.DataContext = portClass;

                    PortManagePanel.Items.Add(port);
                }
            }
        }


        private void PortPanel_OnLoaded(object sender, RoutedEventArgs e)
        {


            LoadPanelPortTreeList();

            LoadCustomTag();//加载自定义标签

            RoomListView.ItemsSource = roomNumbers;

            // DataBridge.DataBridge.PortPanelLinkViewList.Clear();

            RouteViewPanel.ItemsSource = DataBridge.DataBridge.PortPanelLinkViewList;

            //端口色彩标签发生改变
            DataBridge.DataBridge.PortPanelModifyTagList.CollectionChanged += PortPanelModifyTagList_CollectionChanged;
        }

        private void PortPanelModifyTagList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (DataBridge.DataBridge.PortPanelModifyTagList.Count > 0)
            {

                //重新加载端口信息
                RoomListView_OnSelectionChanged(null, null);


            }


        }

        private void LoadPanelPortTreeList(string keyWord = null)
        {
            BuildingTreeView.Items.Clear();

            string filter = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                filter = $"WHERE ( Building LIKE '%{keyWord}%' OR Address LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%' ) AND (Del != 1 OR Del IS NULL) ";
            }
            else
            {
                filter="WHERE (Del != 1 OR Del IS NULL)";
            }

            //第一步：读取所有建筑信息
            string query = $"SELECT *  FROM  Buildings {filter};";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;

                var info = new BuildingInfoClass();
                var building = new BuildingUserControl();
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
                info.Count = DbClass.StatisticsPortRoomFloor(buildingId);

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
                    var floor = new FloorUserControl();

                    floorClass.Index = index2;
                    floorClass.Floor = row1["SlotId"].ToString();
                    floorClass.BuildingId = info.BuildingId;
                    floorClass.RoomCount = DbClass.GetRoomForFloorCount(buildingId, floorClass.Floor);
                    floorClass.PortCount = DbClass.GetPortForFloorCount(buildingId, floorClass.Floor);

                    floor.DataContext = floorClass;



                    buildingItem.Items.Add(floor);
                }




                buildingItem.Header = building;

                BuildingTreeView.Items.Add(buildingItem);
            }



        }



        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddPortPanelWindow add = new AddPortPanelWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }

            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                //如果是添加了楼层，则更新楼层信息树

                //如果是添加了房间，则更新房间信息列表

                //如果是添加了端口面板，则更新端口面板列表  

                LoadPanelPortTreeList();

            }
        }

        private void BuildingTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            NumberBlock.Text = "0";
            roomNumbers.Clear();
            DataBridge.DataBridge.PortPanelLinkViewList.Clear();
            if (e != null)
            {
                AddButton.IsEnabled = true;

                var selectedNode = e.NewValue;

                if (selectedNode is FloorUserControl) //如果是楼层信息
                {
                    DeleteBuildingButton.IsEnabled = false;

                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    FloorUserControl childNode = selectedNode as FloorUserControl;

                    //获取到的楼层信息
                    FloorInfoClass info = childNode.DataContext as FloorInfoClass;

                    DataBridge.DataBridge.SelectBuildingId = info.BuildingId;

                    DataBridge.DataBridge.SelectFloor = info.Floor;

                    //加载房间号

                    LoadRooms(info.BuildingId, info.Floor);



                }
                else if (selectedNode is TreeViewItem) //如果是建筑信息
                {

                    TreeViewItem selectedItem = selectedNode as TreeViewItem;


                    if (selectedItem != null)
                    {

                        DeleteBuildingButton.IsEnabled = true;

                        //获取建筑ID
                        var data = selectedItem.Header as BuildingUserControl;

                        BuildingInfoClass info = data.DataContext as BuildingInfoClass;


                        DataBridge.DataBridge.SelectBuildingId = info.BuildingId;



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

        private ObservableCollection<RoomClass> roomNumbers = new ObservableCollection<RoomClass>();

        private void LoadRooms(string buildingId, string floor)
        {
            string sql = $"SELECT DISTINCT RoomId FROM Bu_{buildingId} WHERE  SlotId ='{floor}'";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                index++;

                RoomClass room = new RoomClass();

                room.Index = index;
                room.RoomNumber = row["RoomId"].ToString();

                room.PortCount = DbClass.GetRoomPortCount(buildingId, floor, room.RoomNumber);


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

        private ObservableCollection<PortClass> portNumbers = new ObservableCollection<PortClass>();
        private async void RoomListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            portNumbers.Clear();
            NumberBlock.Text = "0";

            PortManagePanel.Items.Clear();
            DataBridge.DataBridge.PortPanelLinkViewList.Clear();

            if (RoomListView.SelectedIndex != -1)
            {
                RoomClass room = RoomListView.SelectedItem as RoomClass;


                DataBridge.DataBridge.SelectRoom = room.RoomNumber;

                string sql =
                    $"SELECT * FROM Bu_{DataBridge.DataBridge.SelectBuildingId} WHERE SlotId ='{DataBridge.DataBridge.SelectFloor}' AND RoomId='{room.RoomNumber}'";

                lastSql = sql;

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);





                int index = 0;

                foreach (var row in rows)
                {
                    index++;

                    PortClass portClass = new PortClass();
                    portClass.Index = index;
                    portClass.AssetId = DataBridge.DataBridge.SelectBuildingId;
                    portClass.UID = Convert.ToInt32(row["UID"]);
                    portClass.PortType = row["PortType"].ToString();
                    portClass.PortGroup= row["PortGroup"].ToString();
                    portClass.PortStatus= row["PortStatus"].ToString();
                    portClass.PortIndex = row["PortId"].ToString();
                    portClass.PortTag = row["PortTag"].ToString();
                    portClass.SlotIndex=row["SlotId"].ToString();
                    portClass.PortIndex=row["PortId"].ToString();
                    portClass.Room = row["RoomId"].ToString();
                    portClass.PortTag=row["PortTag"].ToString();
                    portClass.PortColor = Convert.ToInt32(row["PortColor"]);
                    portClass.TagA = row["TagA"].ToString();
                    portClass.TagB = row["TagB"].ToString();
                    portClass.TagC = row["TagC"].ToString();
                    portClass.TagD = row["TagD"].ToString();
                    portClass.TagE = row["TagE"].ToString();
                    portClass.TagF = row["TagF"].ToString();


                    if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                    {
                        portClass.OnTheLine = -1 ;
                    }
                    else
                    {
                        portClass.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                    }



                    PanelPort port = new PanelPort();

                    port.Margin = new Thickness(10);
                    port.DataContext = portClass;
                    
                    portNumbers.Add(portClass);



                    PortManagePanel.Items.Add(port);

                    await Task.Delay(1);
                }


            }

        }


        private void AddBuildingButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddBuildingWindow add = new AddBuildingWindow();


            //窗口放中间
            var window = Window.GetWindow(this);

            if (window != null)
            {
                add.Owner = window;
            }

            if (add.ShowDialog() == true)
            {

                LoadPanelPortTreeList();

            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchKeyWord.Text))
            {
                LoadPanelPortTreeList(SearchKeyWord.Text);
            }
            else
            {
                LoadPanelPortTreeList();
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
            LoadPanelPortTreeList();
        }


        private void ShowModeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ShowModeButton.IsChecked == true)
            {
                PortScrollViewer.Visibility = Visibility.Collapsed;
                PanelPortListView.Visibility = Visibility.Visible;

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
                PortScrollViewer.Visibility = Visibility.Visible;
                PanelPortListView.Visibility = Visibility.Collapsed;

                //LoadMode = 0;
                //OperationPanel.IsEnabled = true;
                //GraphicalPlan.Visibility = Visibility.Visible;
                //AddressListView.Visibility = Visibility.Collapsed;

            }
        }


        private void SelectToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            StatisticsSelectedPort();
        }

        private void SelectToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            StatisticsSelectedPort();
        }


        private void StatisticsSelectedPort()
        {
            var items = portNumbers.Where(item => item.IsSelected == true);
        

            if (items.Count() > 0)
            {
                NumberBlock.Text = items.Count().ToString();
                MultipleButton.IsEnabled = true;
                MultipleDeleteButton.IsEnabled = true;
            }
            else
            {
                NumberBlock.Text = "0";
                MultipleButton.IsEnabled = false;
                MultipleDeleteButton.IsEnabled = false;
            }



        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result =MessageBox.Show("确定要删除吗？\r该操作不可恢复！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                            
                            var sql = $"DELETE FROM Bu_{DataBridge.DataBridge.SelectBuildingId} WHERE UID = {port.UID}";
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
                if (failCount>0)
                {
                    message = $"\r失败{failCount}个。\r失败原因:端口已在链路上！";
                }

                MessageBox.Show($"删除成功{successCount}个{message}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                RoomListView_OnSelectionChanged(null,null);
            }
        }

        private void PanelPortListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PanelPortListView.SelectedIndex != -1)
            {
                DataBridge.DataBridge.PortPanelLinkViewList.Clear();

                var port = portNumbers[PanelPortListView.SelectedIndex];

                if (port.OnTheLine != null && port.OnTheLine > 0)
                {


                    foreach (var node in DbClass.GetLinkDetail(port.OnTheLine))
                    {
                        if (port.RackId == node.PortClass.RackId)
                        {
                            port.NodeIndex = node.PortClass.NodeIndex;

                            port.IsSelected = true;
                        }


                        DataBridge.DataBridge.PortPanelLinkViewList.Add(node);
                    }



                }
                else
                {
                    DataBridge.DataBridge.PortPanelLinkViewList.Clear();
                }


            }
        }

        private void MultipleButton_OnClick(object sender, RoutedEventArgs e)
        {


            var items = portNumbers.Where(item => item.IsSelected == true);

            var newItems = new ObservableCollection<PortClass>(items);


            if (newItems.Count > 0)
            {
                var newWindow = new PortPanelEditWindow(newItems);

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    newWindow.Owner = window;
                }

                newWindow.ShowDialog();

            }




        }

        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {

            var set = new PortPanelTagSetWindow();

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
        /// 加载自定义标签
        /// </summary>
        private void LoadCustomTag()
        {

            //加载端口自定义标签
            var tagWindow = "PortPanelTag";

            string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{tagWindow}'";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);


            if (num > 0) //存在本地自定义标签
            {
                var tags = DbClass.LoadWindowTag(tagWindow);

                if (tags != null)
                {
                    dynamic settings = JsonConvert.DeserializeObject<TagViewModel>(tags);

                    LabelA.Content = settings.TagA;
                    LabelB.Content = settings.TagB;
                    LabelC.Content = settings.TagC;
                    LabelD.Content = settings.TagD;
                    LabelE.Content = settings.TagE;
                    LabelF.Content = settings.TagF;

                }

            }


        }


        private ObservableCollection<PortClass> infos = new ObservableCollection<PortClass>();

        private void PanelPortListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            infos.Clear();
            var info = portNumbers[PanelPortListView.SelectedIndex];
            if (info != null)
            {
                infos.Add(info);

                var newWindow = new PortPanelEditWindow(infos);

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    newWindow.Owner = window;
                }

                newWindow.ShowDialog();
            }



        }

        private void DeleteBuildingButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要删除该建筑吗？该操作不可恢复！\r此操作将同步导致终端管理页面无法访问该建筑信息！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

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

                    LoadPanelPortTreeList();
                }



            }


        }
    }

}