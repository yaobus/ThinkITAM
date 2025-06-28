using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// AddDeviceRoomWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeviceRoomWindow : Window
    {
        public AddDeviceRoomWindow(DeviceRoomClass deviceRoomInfo = null)
        {
            InitializeComponent();
            if (deviceRoomInfo != null)
            {
                deviceRoom = deviceRoomInfo;
                this.DataContext = deviceRoom;
            }
        }


        private DeviceRoomClass deviceRoom = null;
        private void AddDeviceRoomWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


            //加载地址信息
            LoadAddressInfo();
            AddressCombobox.ItemsSource = addressList;

            //加载人员信息
            LoadPeopleInfos();
            PeopleCombobox.ItemsSource = peopleList;

        }


        private ObservableCollection<string> addressList = new ObservableCollection<string>();


        /// <summary>
        /// 加载地址信息
        /// </summary>
        private void LoadAddressInfo()
        {
            string query = "SELECT * FROM Address WHERE (Del != 1 OR Del IS NULL)";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                addressList.Add(row["Location"].ToString());
            }


        }

        private ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();
        private ObservableCollection<string> peopleList = new ObservableCollection<string>();

        /// <summary>
        /// 加载人员信息
        /// </summary>
        private void LoadPeopleInfos()
        {
            peopleInfos.Clear();

            string query = "SELECT * FROM UserInfo;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            int index = 0;

            foreach (var row in rows)
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                peopleList.Add($"{info.Name}-{info.Organization}-{info.Department}-{info.Phone}");

                peopleInfos.Add(info);
            }






        }


        private void PeopleCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PeopleCombobox.SelectedIndex;


            PeopleName.Text = peopleInfos[index].Name;
            Phone.Text = peopleInfos[index].Phone;
        }


        /// <summary>
        /// 保存  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DeviceRoom.Text.Replace(" ", "").Length > 1)
            {

                if (deviceRoom != null)//UPDATE
                {

                    var roomInfo = new
                    {
                        DeviceRoomQrId = deviceRoom.DeviceRoomQrId,
                        RoomName = DeviceRoom.Text,
                        Location = AddressCombobox.Text,
                        User = PeopleName.Text,
                        UserPhone = Phone.Text,
                        Note = Note.Text
                    };

                    var conditions = new
                    {
                        DeviceRoomQrId = deviceRoom.DeviceRoomQrId
                    };

                    GlobalVariables.DbService.UpdateEntity("DeviceRoom", roomInfo, conditions);
                    this.DialogResult = true;


                }
                else
                {
                    //创建资产ID
                    string assetId = AssetIdCreate.CreateAssetId(DeviceRoom.Text);

                    //创建资产二维码,0为机房，1为机柜，2为设备
                    string qrCode = "0" + AssetCodeClass.GenerateChecksum(assetId).ToUpper();

                    var roomInfo = new
                    {
                        DeviceRoomQrId = qrCode,
                        RoomName = DeviceRoom.Text,
                        Location = AddressCombobox.Text,
                        User = PeopleName.Text,
                        UserPhone = Phone.Text,
                        Note = Note.Text
                    };

                    GlobalVariables.DbService.InsertEntity("DeviceRoom", roomInfo);
                    this.DialogResult = true;


                }


            }
            else
            {
                MessageBox.Show("请输入设备房间名称", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }




    }
}
