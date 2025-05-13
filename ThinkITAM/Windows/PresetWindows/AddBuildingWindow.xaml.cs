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



        private void AddBuildingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


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


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
                info.Location = row["Location"].ToString();
                info.Note = row["Note"].ToString();

                addressInfos.Add(info);
            }





        }


        private void LoadPeopleInfo()
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

                var countNum = DbClass.ExecuteScalarTableNum(sql);

                if (countNum == 0)
                {

                    //创建资产ID
                    string assetId = AssetIdCreate.CreateAssetId(building + address);

                    //创建资产ID,0为机房，1为机柜，2为设备,3为机架，8为建筑，9为人员
                    string buildingId = "8" + AssetCodeClass.GenerateChecksum(assetId).ToUpper();
                    string people = PeopleCombobox.Text;
                    string phone = Phone.Text;
                    string note = Note.Text;

                    var buildingInfo = new
                    {
                        BuildingId=buildingId,
                        Building=building,
                        Address=address,
                        User = people,
                        Phone=phone,
                        Note=note   
                    };

                   // sql = $"INSERT INTO \"Buildings\" (\"BuildingId\", \"Building\", \"Address\", \"User\", \"Phone\", \"Note\") VALUES ('{buildingId}', '{building}', '{address}', '{people}', '{phone}', '{note}')";


                    GlobalVariables.DbService.InsertEntity("Buildings", buildingInfo);

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
