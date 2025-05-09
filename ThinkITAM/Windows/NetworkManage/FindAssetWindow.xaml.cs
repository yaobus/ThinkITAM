using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.AssetManage;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// FindAssetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindAssetWindow : Window
    {
        public string SelectAssetId = null;//传入的资产标签

        public FindAssetWindow(string assetId = null)
        {
            InitializeComponent();

            SelectAssetId = assetId;
            
            this.Owner = Application.Current.MainWindow;
        }

        private DbClass dbClass;

        private void FindAssetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            //加载全部资产信息
            LoadSelectAssetInfo();

            LoadAssetType();
            AssetDataGrid.ItemsSource = assetViewModels;

            if (SelectAssetId != null)
            {
                loadStatus = 1;
                LoadSelectAssetInfo(SelectAssetId);
                AssetDataGrid.SelectedIndex = 0;
                loadStatus = 0;
            }


        }

        //是否加载资产信息
        private int loadStatus = 0;//0：正常加载，1：不加载

        /// <summary>
        /// 通过传入的资产标签加载资产信息
        /// </summary>
        /// <param name="id"></param>
        private void LoadSelectAssetInfo(string id = null)
        {
            assetViewModels.Clear();

            string sql;

            if (id != null)
            {
                sql = $"SELECT * FROM Asset WHERE AssetId ='{id}'";
            }
            else
            {
                sql = $"SELECT * FROM Asset ";
            }


           


            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int i = 0;

            while (reader.Read())
            {
                var item = new AssetViewModel();
                i++;
                item.Index = i;

                item.AssetId = reader["AssetId"].ToString();
                item.AssetQrCode = reader["AssetQrCode"].ToString();
                item.AssetType = reader["AssetType"].ToString();
                item.DeviceType = reader["DeviceType"].ToString();
                item.AssetNumber = reader["AssetTag"].ToString() + reader["AssetNumber"].ToString();

                AssetType.Text = item.AssetType;
                DeviceType.Text = item.DeviceType;
                

                string purchaseDate = reader["PurchaseDate"].ToString();
                DateTime time1;
                if (purchaseDate.Length > 0)
                {
                    time1 = DateTime.Parse(reader["PurchaseDate"].ToString());
                    item.PurchaseDate = time1.ToString("d");
                }
                else
                {

                    item.PurchaseDate = null;
                }





                item.PurchasePrice = reader["PurchasePrice"].ToString();
                item.Manufacturer = reader["Manufacturer"].ToString();
                item.Model = reader["Model"].ToString();
                item.SerialNumber = reader["SerialNumber"].ToString();
                item.Configuration = reader["Configuration"].ToString();
                item.Location = reader["Location"].ToString();
                item.UserOrganization = reader["UserOrganization"].ToString();
                item.UserDepartment = reader["UserDepartment"].ToString();
                item.User = reader["User"].ToString();
                item.UserPhone = reader["UserPhone"].ToString();
                item.Consumer = reader["Consumer"].ToString();
                item.Status = reader["Status"].ToString();
                item.UsedYear = reader["UsedYear"].ToString();

                string timeStr = reader["ScrapDate"].ToString();

                if (timeStr.Length > 3)
                {
                    DateTime time2 = DateTime.Parse(timeStr);
                    item.ScrapDate = time2.ToString("d");

                }
                else
                {
                    item.ScrapDate = null;
                }



                item.Notes = reader["Notes"].ToString();
                item.TagA = reader["TagA"].ToString();
                item.TagB = reader["TagB"].ToString();
                item.TagC = reader["TagC"].ToString();
                item.TagD = reader["TagD"].ToString();
                item.TagE = reader["TagE"].ToString();
                item.TagF = reader["TagF"].ToString();

                //var asset = new AssetInfoUserControl();

                //asset.DataContext= item;

                //AssetListView.Items.Add(asset);

                assetViewModels.Add(item);

            }
        }


        /// <summary>
        /// 资产类型列表
        /// </summary>
        private ObservableCollection<string> assetTypeInfos = new ObservableCollection<string>();

        private void LoadAssetType()
        {

            assetTypeInfos.Clear();

            string query = "SELECT DISTINCT AssetType FROM AssetTag;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);

            SQLiteDataReader reader = command.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    assetTypeInfos.Add(reader["AssetType"].ToString());
                }
            }


            AssetType.ItemsSource = assetTypeInfos;

        }



        /// <summary>
        /// 设备类型列表
        /// </summary>
        private ObservableCollection<string> deviceTypeInfos = new ObservableCollection<string>();


        private void AssetType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (AssetType.SelectedIndex != -1)
            {
                deviceTypeInfos.Clear();
                assetViewModels.Clear();
                AssetTag.Text = "";



                string query = $"SELECT  DeviceType FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}';";

                //Console.WriteLine(query);

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    deviceTypeInfos.Add(reader["DeviceType"].ToString());
                }

                DeviceType.ItemsSource = deviceTypeInfos;
            }
            else
            {
                deviceTypeInfos.Clear();
            }
        }


        private void DeviceType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (DeviceType.SelectedIndex != -1 && loadStatus == 0)
            {
                string assetType = assetTypeInfos[AssetType.SelectedIndex];


                string deviceType = deviceTypeInfos[DeviceType.SelectedIndex];

                LoadAssetInfos(assetType, deviceType);

            }

        }

        ObservableCollection<AssetViewModel> assetViewModels = new ObservableCollection<AssetViewModel>();


        private void LoadAssetInfos(string assetType, string? deviceType)
        {
            assetViewModels.Clear();

            string sql;

            if (deviceType != null && deviceType.Replace(" ", "").Length > 0) //设备类型不为空
            {
                sql = $"SELECT * FROM Asset WHERE AssetType ='{assetType}' AND DeviceType='{deviceType}'";

            }
            else
            {
                sql = $"SELECT * FROM Asset WHERE AssetType ='{assetType}'";
            }

            

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int i = 0;

            while (reader.Read())
            {
                var item = new AssetViewModel();
                i++;
                item.Index = i;

                item.AssetId = reader["AssetId"].ToString();
                item.AssetQrCode = reader["AssetQrCode"].ToString();
                item.AssetType = reader["AssetType"].ToString();
                item.DeviceType = reader["DeviceType"].ToString();
                item.AssetNumber = reader["AssetTag"].ToString() + reader["AssetNumber"].ToString();


                string purchaseDate = reader["PurchaseDate"].ToString();
                DateTime time1;
                if (purchaseDate.Length > 0)
                {
                    time1 = DateTime.Parse(reader["PurchaseDate"].ToString());
                    item.PurchaseDate = time1.ToString("d");
                }
                else
                {

                    item.PurchaseDate = null;
                }



               

                item.PurchasePrice = reader["PurchasePrice"].ToString();
                item.Manufacturer = reader["Manufacturer"].ToString();
                item.Model = reader["Model"].ToString();
                item.SerialNumber = reader["SerialNumber"].ToString();
                item.Configuration = reader["Configuration"].ToString();
                item.Location = reader["Location"].ToString();
                item.UserOrganization = reader["UserOrganization"].ToString();
                item.UserDepartment = reader["UserDepartment"].ToString();
                item.User = reader["User"].ToString();
                item.UserPhone = reader["UserPhone"].ToString();
                item.Consumer = reader["Consumer"].ToString();
                item.Status = reader["Status"].ToString();
                item.UsedYear = reader["UsedYear"].ToString();

                string timeStr = reader["ScrapDate"].ToString();

                if (timeStr.Length > 3)
                {
                    DateTime time2 = DateTime.Parse(timeStr);
                    item.ScrapDate = time2.ToString("d");

                }
                else
                {
                    item.ScrapDate = null;
                }



                item.Notes = reader["Notes"].ToString();
                item.TagA = reader["TagA"].ToString();
                item.TagB = reader["TagB"].ToString();
                item.TagC = reader["TagC"].ToString();
                item.TagD = reader["TagD"].ToString();
                item.TagE = reader["TagE"].ToString();
                item.TagF = reader["TagF"].ToString();

                //var asset = new AssetInfoUserControl();

                //asset.DataContext= item;

                //AssetListView.Items.Add(asset);

                assetViewModels.Add(item);

            }
        }

        private string assetId;//关联的资产全局唯一ID

        private void AssetDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 检查选中项是否非空，以避免空引用异常
            if (AssetDataGrid.SelectedItem != null)
            {
                // 通过 SelectedItem 属性获取选中的行数据
                var selectedRowData = AssetDataGrid.SelectedItem as AssetViewModel;

                DataBridge.DataBridge.SelectAssetInfo = selectedRowData;

                AssetTag.Text = selectedRowData.AssetNumber;
                Model.Text=selectedRowData.Model;
                Description.Text = selectedRowData.Notes;
                assetId = selectedRowData.AssetId;
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
           this.Close();
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataBridge.DataBridge.LinkSelectAssetId = AssetTag.Text;
            DataBridge.DataBridge.LinkAssetId = assetId;

            this.DialogResult = true;
            this.Close();
        }
    }
}
