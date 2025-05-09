using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
using ThinkITAM.ChildrenWindows.LinkWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModes.AssetManage;
using ThinkITAM.ViewModes.LinkManage;

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

        private DbClass dbClass;
        private void CabinetUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            RoomListView.ItemsSource = deviceRoomInfos;
            CabinetListView.ItemsSource= deviceCabinetInfos;

            LoadDeviceRoomInfo();
        }


        /// <summary>
        /// 机房信息列表
        /// </summary>
        private ObservableCollection<ViewModes.LinkManage.DeviceRoomClass> deviceRoomInfos = new ObservableCollection<ViewModes.LinkManage.DeviceRoomClass>();


        /// <summary>
        /// 加载机房信息
        /// </summary>
        private void LoadDeviceRoomInfo()
        {
            deviceRoomInfos.Clear();

            string query = "SELECT * FROM DeviceRoom;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                ViewModes.LinkManage.DeviceRoomClass info = new ViewModes.LinkManage.DeviceRoomClass();
                info.Index = index;
                info.DeviceRoomQrId = reader["DeviceRoomQrId"].ToString();
                info.Name = reader["RoomName"].ToString();
                info.Location = reader["Location"].ToString();
                info.User = reader["User"].ToString();
                info.UserPhone = reader["UserPhone"].ToString();
                info.Note = reader["Note"].ToString();



                deviceRoomInfos.Add(info);
            }




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
                var roomInfo = RoomListView.SelectedItem as DeviceRoomClass;

                roomId = roomInfo.DeviceRoomQrId;


                LoadCabinetInfo(roomId);
            }
        }

        private void LoadCabinetInfo(string deviceRoomQrId)
        {
            deviceCabinetInfos.Clear();

            string query = $"SELECT * FROM DeviceCabinet WHERE DeviceRoomQrId='{deviceRoomQrId}';";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;

                CabinetClass info = new CabinetClass();
                info.Index = index;
                info.CabinetId = reader["CabinetId"].ToString();
                info.Name = reader["CabinetName"].ToString();
                info.Position = reader["Position"].ToString();
                info.Note = reader["Note"].ToString();


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

                // 当子窗口关闭后执行这里的代码

                LoadDeviceRoomInfo();
                //加载设备信息
                //LoadTags();
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
    }
}
