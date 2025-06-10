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
using ThinkITAM.Windows.LinkWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.DataBridge;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// CabinetUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class CabinetUserControl : UserControl
    {
        public CabinetUserControl()
        {
            InitializeComponent();
        }


        private void CabinetUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {

            RoomListView.ItemsSource = deviceRoomInfos;
            CabinetListView.ItemsSource= deviceCabinetInfos;

            LoadDeviceRoomInfo();
        }


        /// <summary>
        /// 机房信息列表
        /// </summary>
        private ObservableCollection<ViewModels.LinkManage.DeviceRoomClass> deviceRoomInfos = new ObservableCollection<ViewModels.LinkManage.DeviceRoomClass>();


        /// <summary>
        /// 加载机房信息
        /// </summary>
        private void LoadDeviceRoomInfo()
        {
            deviceRoomInfos.Clear();

            string query = "SELECT * FROM DeviceRoom WHERE (Del != 1 OR Del IS NULL);";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                                index++;
                ViewModels.LinkManage.DeviceRoomClass info = new ViewModels.LinkManage.DeviceRoomClass();
                info.Index = index;
                info.DeviceRoomQrId = row["DeviceRoomQrId"].ToString();
                info.Name = row["RoomName"].ToString();
                info.Location = row["Location"].ToString();
                info.User = row["User"].ToString();
                info.UserPhone = row["UserPhone"].ToString();
                info.Note = row["Note"].ToString();



                deviceRoomInfos.Add(info);
            }




            deviceCabinetInfos.Clear();
        }

        /// <summary>
        /// 机房信息列表
        /// </summary>
        private ObservableCollection<CabinetClass> deviceCabinetInfos = new ObservableCollection<CabinetClass>();

        /// <summary>
        /// 配线间ID
        /// </summary>
        private string roomId;

        /// <summary>
        /// 选中机房，加载机房的机柜信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = RoomListView.SelectedIndex;

            if (num != -1)
            {
                var roomInfo = deviceRoomInfos[num];

                roomId = roomInfo.DeviceRoomQrId;


                EditRoomButton.IsEnabled=true;
                DeleteRoomButton.IsEnabled=true;

                LoadCabinetInfo(roomId);
            }
            else
            {
                EditRoomButton.IsEnabled = false;
                DeleteRoomButton.IsEnabled = false;
            }
        }

        private void LoadCabinetInfo(string deviceRoomQrId)
        {
            deviceCabinetInfos.Clear();

            string query = $"SELECT * FROM DeviceCabinet WHERE DeviceRoomQrId='{deviceRoomQrId}' AND (Del != 1 OR Del IS NULL);";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;

                CabinetClass info = new CabinetClass();
                info.Index = index;
                info.DeviceRoomQrId = deviceRoomQrId;
                info.CabinetId = row["CabinetId"].ToString();
                info.Name = row["CabinetName"].ToString();
                info.Position = row["Position"].ToString();
                info.Note = row["Note"].ToString();


                deviceCabinetInfos.Add(info);
            }



        }


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

                LoadDeviceRoomInfo();
               
            }

        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
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
                if (roomId != null)
                {
                    LoadCabinetInfo(roomId);
                }


            }
        }

        private void RoomListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int num = RoomListView.SelectedIndex;

            if (num != -1)
            {
                var roomInfo = deviceRoomInfos[num];


                var addDeviceRoomWindow = new AddDeviceRoomWindow(roomInfo);

                //窗口放中间
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    addDeviceRoomWindow.Owner = window;
                }

                if (addDeviceRoomWindow.ShowDialog() == true)
                {

                    LoadDeviceRoomInfo();

                }

            }

        }

        private void DeleteRoomButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = RoomListView.SelectedIndex;

            if (index != -1)
            {

                var info = deviceRoomInfos[index];


                var message = $"确定要删除吗？\r房间名称:{info.Name}\r地址:{info.Location}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE DeviceRoom SET Del = 1 WHERE DeviceRoomQrId ='{info.DeviceRoomQrId}';";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadDeviceRoomInfo();

                }


            }
        }

        private void CabinetListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = CabinetListView.SelectedIndex;

            if (num != -1)
            {

                EditCabinetButton.IsEnabled = true;
                DeleteCabinetButton.IsEnabled = true;

                
            }
            else
            {
                EditCabinetButton.IsEnabled = false;
                DeleteCabinetButton.IsEnabled = false;
            }
        }

        private void CabinetListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int num = CabinetListView.SelectedIndex;

            if (num != -1)
            {
                var cabinetInfo = deviceCabinetInfos[num];


                AddGroupWindow add = new AddGroupWindow(cabinetInfo);

                //窗口放中间
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    add.Owner = window;
                }

                if (add.ShowDialog() == true)
                {

                    // 当子窗口关闭后执行这里的代码
                    if (roomId != null)
                    {
                        LoadCabinetInfo(roomId);
                    }
                }


            }
        }

        private void DeleteCabinetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = CabinetListView.SelectedIndex;

            if (index != -1)
            {

                var info = deviceCabinetInfos[index];


                var message = $"确定要删除吗？\r房间名称:{info.Name}\r位置:{info.Position}\r备注:{info.Note}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE DeviceCabinet SET Del = 1 WHERE DeviceRoomQrId ='{info.DeviceRoomQrId}' AND  CabinetId ='{info.CabinetId}';";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadCabinetInfo(info.DeviceRoomQrId);
                }


            }
        }
    }
}
