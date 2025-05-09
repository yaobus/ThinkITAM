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

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddBuildingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddBuildingWindow : Window
    {
        public AddBuildingWindow()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        private void AddBuildingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

            AddressCombobox.ItemsSource = addressInfos;

            PeopleCombobox.ItemsSource = peopleInfos;

            LoadAddressInfo();
            LoadPeopleInfo();
        }

        ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();
        ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();


        private void LoadAddressInfo()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
                info.Location = reader["Location"].ToString();
                info.Note = reader["Note"].ToString();

                addressInfos.Add(info);
            }




        }


        private void LoadPeopleInfo()
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

                peopleInfos.Add(info);
            }

        }

        private void PeopleCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Phone.Text = peopleInfos[PeopleCombobox.SelectedIndex].Phone;
            Note.Text = peopleInfos[PeopleCombobox.SelectedIndex].Note;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string building = BuildingName.Text;
            string address = AddressCombobox.Text;

            if (building.Replace(" ", "").Length > 0 && address.Replace(" ", "").Length > 0)
            {
                string sql = $"SELECT COUNT(*) FROM Buildings WHERE Building='{building}' AND Address='{address}'";

                var countNum = dbClass.ExecuteScalarTableNum(sql, dbClass.connection);

                if (countNum == 0)
                {

                    //创建资产ID
                    string assetId = AssetIdCreate.CreateAssetId(building + address);

                    //创建资产ID,0为机房，1为机柜，2为设备,3为机架，8为建筑，9为人员
                    string buildingId = "8" + FunctionClass.AssetCodeClass.GenerateChecksum(assetId).ToUpper();
                    string people = PeopleCombobox.Text;
                    string phone = Phone.Text;
                    string note = Note.Text;

                    sql = $"INSERT INTO \"Buildings\" (\"BuildingId\", \"Building\", \"Address\", \"User\", \"Phone\", \"Note\") VALUES ('{buildingId}', '{building}', '{address}', '{people}', '{phone}', '{note}')";

                    dbClass.ExecuteQuery(sql);

                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("该建筑已存在");
                }
            }
            else
            {
                MessageBox.Show("请输入完整的建筑名称和地址信息");
            }






        }
    }
}
