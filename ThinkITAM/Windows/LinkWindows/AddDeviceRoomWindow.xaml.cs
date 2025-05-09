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
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Preset;

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
            string query = "SELECT Location FROM  Address";


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
                string assetId =AssetIdCreate.CreateAssetId(DeviceRoom.Text);

                //创建资产二维码,0为机房，1为机柜，2为设备
                string qrCode = "0" +AssetCodeClass.GenerateChecksum(assetId).ToUpper();


                string sql = $"INSERT INTO DeviceRoom (DeviceRoomQrId,RoomName,Location,User,UserPhone,Note) VALUES ('{qrCode}','{DeviceRoom.Text}','{AddressCombobox.Text}','{PeopleName.Text}','{Phone.Text}','{Note.Text}')";


                Console.WriteLine(sql);

             
                GlobalVariables.DbService.ExecuteNonQuery(sql);
                this.DialogResult = true;


            }
            else
            {
              MessageBox.Show("请输入设备房间名称", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }




    }
}
