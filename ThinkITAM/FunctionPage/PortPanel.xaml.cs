using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Controls;

using ThinkITAM.Windows.PortPanel;
using ThinkITAM.Windows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.UserControls.PortPanel;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.DataBridge;

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



        private void PortPanel_OnLoaded(object sender, RoutedEventArgs e)
        {


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



        private void RoomListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PortManagePanel.Items.Clear();

            if (RoomListView.SelectedIndex != -1)
            {
                RoomClass room = RoomListView.SelectedItem as RoomClass;


                DataBridge.DataBridge.SelectRoom = room.RoomNumber;

                string sql =
                    $"SELECT * FROM Bu_{DataBridge.DataBridge.SelectBuildingId} WHERE SlotId ='{DataBridge.DataBridge.SelectFloor}' AND RoomId='{room.RoomNumber}'";

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);





                int index = 0;

                foreach (var row in rows)
                {
                    index++;

                    PortClass portClass = new PortClass();

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