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
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.ViewModes.Preset;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// AddDeviceRoomWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeviceRoomWindow : Window
    {
        public AddDeviceRoomWindow()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        private void AddDeviceRoomWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

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
            string query = "SELECT Location FROM  Address";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                addressList.Add(reader["Location"].ToString());

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

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();



            int index = 0;

            while (reader.Read())
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.Name = reader["Name"].ToString();
                info.Organization = reader["Organization"].ToString();
                info.Department = reader["Department"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();

                peopleList.Add($"{info.Name}-{info.Organization}-{info.Department}-{info.Phone}");

                peopleInfos.Add(info);
            }

            


        }


        private void PeopleCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PeopleCombobox.SelectedIndex;


            PeopleName.Text = peopleInfos[index].Name;
            Phone.Text= peopleInfos[index].Phone;
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
                


                //MessageBox.Show("TODO/检查机房是否存在");

                //创建资产ID
                string assetId = AssetIdCreate.CreateAssetId(DeviceRoom.Text);

                //创建资产二维码,0为机房，1为机柜，2为设备
                string qrCode = "0" + FunctionClass.AssetCodeClass.GenerateChecksum(assetId).ToUpper();


                string sql = $"INSERT INTO DeviceRoom (DeviceRoomQrId,RoomName,Location,User,UserPhone,Note) VALUES ('{qrCode}','{DeviceRoom.Text}','{AddressCombobox.Text}','{PeopleName.Text}','{Phone.Text}','{Note.Text}')";


                Console.WriteLine(sql);

                dbClass.ExecuteQuery(sql);
                this.DialogResult = true;


            }
            else
            {
              MessageBox.Show("请输入设备房间名称", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }




    }
}
