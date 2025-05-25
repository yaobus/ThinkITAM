using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.Windows.NetworkManage;

namespace ThinkITAM.Windows.Computer
{
    /// <summary>
    /// AddComputerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddComputerWindow : Window
    {
        public AddComputerWindow()
        {
            InitializeComponent();
        }

        private void FindAsset_OnClick(object sender, RoutedEventArgs e)
        {
            DataBridge.DataBridge.SelectAssetInfo = null;
            FindAssetWindow findAsset = new FindAssetWindow();

            if (findAsset.ShowDialog() == true)
            {
                if (DataBridge.DataBridge.SelectAssetInfo != null)
                {
                    var info = DataBridge.DataBridge.SelectAssetInfo;

                    AssetType.DataContext = info;
                   
                    AssetNumber.DataContext = info;

                    DeviceType.DataContext=info;


                }


                //加载资产信息

            }
        }


        private ObservableCollection<BuildingInfoClass> buildingInfos = new ObservableCollection<BuildingInfoClass>();
        private ObservableCollection<FloorInfoClass> floorInfos = new ObservableCollection<FloorInfoClass>();

        private void AddComputerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            FloorCombobox.ItemsSource = floorInfos;
            BuildingCombobox.ItemsSource = buildingInfos;
            RoomCombobox.ItemsSource = roomIds;


            PortGroupCombobox.ItemsSource = groups;

            //加载建筑信息
            LoadBuildingInfos();
            LoadGroups();
        }

        /// <summary>
        /// 加载建筑名称
        /// </summary>
        private void LoadBuildingInfos()
        {
            buildingInfos.Clear();

            string sql = "SELECT * FROM Buildings";



            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                BuildingInfoClass info = new BuildingInfoClass();

                info.Index = index;
                info.BuildingId = row["BuildingId"].ToString();
                info.Building = row["Building"].ToString();
                info.Address = row["Address"].ToString();
                info.User = row["User"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                buildingInfos.Add(info);
            }



            if (DataBridge.DataBridge.SelectBuildingId != null)
            {
                // 使用LINQ查询与targetBuildingId相匹配的BuildingInfoClass实例
                BuildingInfoClass matchedBuildingInfo =
                    buildingInfos.FirstOrDefault(b => b.BuildingId == DataBridge.DataBridge.SelectBuildingId);

                if (matchedBuildingInfo != null)
                {
                    BuildingCombobox.Text = matchedBuildingInfo.Building;

                    // 加载楼层信息
                    LoadFloorInfos(DataBridge.DataBridge.SelectBuildingId);

                    if (DataBridge.DataBridge.SelectFloor != "")
                    {
                        FloorCombobox.Text = DataBridge.DataBridge.SelectFloor;
                    }

                }


            }


        }


        /// <summary>
        /// 加载楼层信息,楼层信息存放在 SlotId 中 SlotId就是Floor
        /// </summary>
        /// <param name="buildingId"></param>
        private void LoadFloorInfos(string buildingId)
        {
            floorInfos.Clear();

            string sql = $"SELECT DISTINCT SlotId FROM Bu_{buildingId}";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;


            foreach (var row in rows)
            {
                index++;
                FloorInfoClass floor = new FloorInfoClass();

                floor.Floor = row["SlotId"].ToString();

                floorInfos.Add(floor);
            }


        }

        private ObservableCollection<string> roomIds = new ObservableCollection<string>();
        private void BuildingCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            floorInfos.Clear();
            roomIds.Clear();
            
            if (BuildingCombobox.SelectedIndex != -1)
            {

                string buildingId = buildingInfos[BuildingCombobox.SelectedIndex].BuildingId;

                DataBridge.DataBridge.SelectBuildingId = buildingId;

                LoadFloorInfos(buildingId);


            }
        }

        private void FloorCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FloorCombobox.SelectedIndex != -1)
            {



                DataBridge.DataBridge.SelectFloor = floorInfos[FloorCombobox.SelectedIndex].Floor;

                LoadRoomId();

                RoomCombobox.ItemsSource = roomIds;

            }
        }

        private void LoadRoomId()
        {
            roomIds.Clear();
            string sql =
                $"SELECT DISTINCT RoomId FROM Bu_{DataBridge.DataBridge.SelectBuildingId}  WHERE SlotId ='{DataBridge.DataBridge.SelectFloor}'";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                string room = row["RoomId"].ToString();

                roomIds.Add(room);
            }


        }



        private ObservableCollection<string> groups = new ObservableCollection<string>();

        /// <summary>
        /// 加载自定义分组
        /// </summary>
        private void LoadGroups()
        {
            string sql = $"SELECT DISTINCT PortGroup FROM Computer ";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                string group = row["PortGroup"].ToString();
                groups.Add(group);
            }


        }

        private void FindUser_OnClick(object sender, RoutedEventArgs e)
        {
            //DataBridge.DataBridge.SelectAssetInfo = null;
            FindUserWindow find = new FindUserWindow();
            find.Owner = this;
            if (find.ShowDialog() == true)
            {

                AssetUser.Text = DataBridge.DataBridge.SelectPeopleViewModel.Name;
                //if (DataBridge.DataBridge.SelectAssetInfo != null)
                //{
                //    //var info = DataBridge.DataBridge.SelectAssetInfo;

                //    //AssetType.DataContext = info;

                //    //AssetNumber.DataContext = info;

                //    //DeviceType.DataContext = info;


                //}


                //加载资产信息

            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            string portType = null;

            if (PortTypeCombobox.SelectedIndex == 0)
            {

                portType = "E";
            }
            else
            {
                portType = "F";
            }

           


            //创建资产ID字符串，0为机房，1为机柜，2为设备,3为机架，4为通用终端（计算机、IP电话）
            string deviceId = $"4{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";


            var query = $"SELECT COUNT(*) FROM Computer WHERE AssetId='{DataBridge.DataBridge.SelectAssetInfo.AssetId}'";

            var count  =Convert.ToInt32( GlobalVariables.DbService.ExecuteScalar(query));

            if (count > 0)
            {
                MessageBox.Show("该资产已有部署信息，无法重复部署");
            }
            else
            {
                if (BuildingCombobox.SelectedIndex != -1 && FloorCombobox.SelectedIndex != -1 && RoomCombobox.SelectedIndex != -1)
                {
                    var info = new
                    {   
                        UID= DbClass.GetNextAvailableNumber("Computer", "UID"),
                        DeviceId = deviceId,
                        AssetId = DataBridge.DataBridge.SelectAssetInfo.AssetId,
                        AssetUser = DataBridge.DataBridge.SelectPeopleViewModel.UserId,
                        PortId = 1,
                        PortTag = PortTag.Text,
                        PortType = portType,
                        PortGroup = PortGroupCombobox.Text,
                        BuildingId = buildingInfos[BuildingCombobox.SelectedIndex].BuildingId,
                        Floor = floorInfos[FloorCombobox.SelectedIndex].Floor,
                        Room = roomIds[RoomCombobox.SelectedIndex]

                    };

                    GlobalVariables.DbService.InsertEntity("Computer", info);

                    string sql = $"UPDATE Asset SET Deploy = 4 WHERE AssetId = '{DataBridge.DataBridge.SelectAssetInfo.AssetId}'";

                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("请选择设备所部署的建筑物、楼层、房间等信息");
                }

            }






        }
    }
}
