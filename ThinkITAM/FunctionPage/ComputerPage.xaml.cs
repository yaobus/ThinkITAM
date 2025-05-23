using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.Computer;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.Windows.Computer;
using ThinkITAM.Windows.DevicePortManage;

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

            RoomListView.ItemsSource = roomNumbers;

        }

        private void LoadTreeList()
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
                var building = new  BuildingUserControl();
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

                    var floor = new UserControls.Computer.FloorUserControl();

                    floorClass.Index = index2;
                    floorClass.Floor = row1["SlotId"].ToString();
                    floorClass.BuildingId = info.BuildingId;
                    floorClass.RoomCount = DbClass.GetRoomForFloorCount(buildingId, floorClass.Floor);
                    floorClass.PortCount = DbClass.GetDeviceForFloorCount(buildingId, floorClass.Floor);

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

                if (selectedNode is UserControls.Computer.FloorUserControl) //如果是建筑信息
                {

                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    var childNode = selectedNode as UserControls.Computer.FloorUserControl;

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
                        var data = selectedItem.Header as UserControls.Computer.FloorUserControl;

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

    }
}
