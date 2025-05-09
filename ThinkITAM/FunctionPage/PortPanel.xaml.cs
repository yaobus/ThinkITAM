using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.ChildrenWindows.PortPanel;
using ThinkITAM.ChildrenWindows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.UserControls.PortPanel;
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModes.PortPanel;

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
        }

        private DbClass dbClass;

        private void PortPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

            LoadPanelPortTreeList();


            RoomListView.ItemsSource = roomNumbers;

            


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

        private void LoadPanelPortTreeList()
        {
            BuildingTreeView.Items.Clear();

            //第一步：读取所有建筑信息
            string query = "SELECT *  FROM  Buildings;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;

                var info = new BuildingInfoClass();
                var building = new BuildingUserControl();
                info.Index = index;
                
                string buildingId = reader["BuildingId"].ToString();

                info.BuildingId = buildingId;

                dbClass.CreateDynamicsTableIfNotExists(buildingId, 1);

                //取出建筑名称
                info.Building = reader["Building"].ToString();

                info.Address = reader["Address"].ToString();
                info.User = reader["User"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();
                info.Count = dbClass.StatisticsPortRoomFloor(buildingId);

                building.DataContext = info;

                //取出建筑楼层信息
                string sql = $"SELECT DISTINCT SlotId FROM Bu_{buildingId}";
               

                SQLiteCommand command1 = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                int index2 = 0;

                var buildingItem = new TreeViewItem();

                while (reader1.Read())
                {
                    index2++;
                    var floorClass = new FloorInfoClass();
                    var floor = new FloorUserControl();

                    floorClass.Index = index2;
                    floorClass.Floor = reader1["SlotId"].ToString();
                    floorClass.BuildingId = info.BuildingId;
                    floorClass.RoomCount = dbClass.GetRoomForFloorCount(buildingId, floorClass.Floor);
                    floorClass.PortCount = dbClass.GetPortForFloorCount(buildingId, floorClass.Floor);

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
            roomNumbers.Clear();

            if (e != null)
            {
                AddButton.IsEnabled = true;

                var selectedNode = e.NewValue;

                if (selectedNode is FloorUserControl) //如果是建筑信息
                {

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
            string sql = $"SELECT DISTINCT RoomId FROM bu_{buildingId} WHERE  SlotId ='{floor}'";

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;

                RoomClass room = new RoomClass();

                room.Index = index;
                room.RoomNumber = reader["RoomId"].ToString();
                
                room.PortCount = dbClass.GetRoomPortCount(buildingId,floor, room.RoomNumber);


                string noteId = $"{buildingId}{floor}{room.RoomNumber}";
                //根据Id查询备注

                string note = dbClass.LoadNote(noteId);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    room.RoomNote =note;
                }
                else
                {
                    room.RoomNote = "";
                }

                roomNumbers.Add(room);


            }

        }

       

        private void RoomListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PortManagePanel.Items.Clear();

            if (RoomListView.SelectedIndex != -1)
            {
                RoomClass room = RoomListView.SelectedItem as RoomClass;


                DataBridge.DataBridge.SelectRoom = room.RoomNumber;

                string sql =
                    $"SELECT * FROM Bu_{DataBridge.DataBridge.SelectBuildingId} WHERE SlotId ='{DataBridge.DataBridge.SelectFloor}' AND RoomId='{room.RoomNumber}'";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int index = 0;

                while (reader.Read())
                {
                    index++;

                    PortClass portClass = new PortClass();

                    portClass.UID= Convert.ToInt32(reader["UID"]);
                    portClass.PortType = reader["PortType"].ToString();
                    portClass.PortIndex = reader["PortId"].ToString();
                    portClass.PortTag= reader["PortTag"].ToString();
                    portClass.Room = reader["RoomId"].ToString();
                    portClass.PortColor = Convert.ToInt32(reader["PortColor"]);



                    if (reader["OnTheLine"] == DBNull.Value)
                    {
                        portClass.OnTheLine = -1;
                    }
                    else
                    {
                        portClass.OnTheLine = Convert.ToInt32(reader["OnTheLine"]);
                    }



                    PanelPort port = new PanelPort();

                    port.Margin = new Thickness(10);
                    port.DataContext = portClass;

                    PortManagePanel.Items.Add(port);
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
    }
    
}