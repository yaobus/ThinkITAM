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
using ThinkITAM.DataBridge;
using System.Collections;

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



        private void FindAssetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


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


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);


            int i = 0;

            foreach (var row in rows)
            {
                var item = new AssetViewModel();
                i++;
                item.Index = i;

                item.AssetId = row["AssetId"].ToString();
                item.AssetQrCode = row["AssetQrCode"].ToString();
                item.AssetType = row["AssetType"].ToString();
                item.DeviceType = row["DeviceType"].ToString();
                item.AssetNumber = row["AssetTag"].ToString() + row["AssetNumber"].ToString();

                AssetType.Text = item.AssetType;
                DeviceType.Text = item.DeviceType;


                string purchaseDate = row["PurchaseDate"].ToString();
                DateTime time1;
                if (purchaseDate.Length > 0)
                {
                    time1 = DateTime.Parse(row["PurchaseDate"].ToString());
                    item.PurchaseDate = time1.ToString("d");
                }
                else
                {

                    item.PurchaseDate = null;
                }





                item.PurchasePrice = row["PurchasePrice"].ToString();
                item.Manufacturer = row["Manufacturer"].ToString();
                item.Model = row["Model"].ToString();
                item.SerialNumber = row["SerialNumber"].ToString();
                item.Configuration = row["Configuration"].ToString();
                item.Location = row["Location"].ToString();
                item.UserOrganization = row["UserOrganization"].ToString();
                item.UserDepartment = row["UserDepartment"].ToString();
                item.User = row["User"].ToString();
                item.UserPhone = row["UserPhone"].ToString();
                item.Consumer = row["Consumer"].ToString();
                item.Status = row["Status"].ToString();
                item.UsedYear = row["UsedYear"].ToString();

                string timeStr = row["ScrapDate"].ToString();

                if (timeStr.Length > 3)
                {
                    DateTime time2 = DateTime.Parse(timeStr);
                    item.ScrapDate = time2.ToString("d");

                }
                else
                {
                    item.ScrapDate = null;
                }



                item.Notes = row["Notes"].ToString();
                item.TagA = row["TagA"].ToString();
                item.TagB = row["TagB"].ToString();
                item.TagC = row["TagC"].ToString();
                item.TagD = row["TagD"].ToString();
                item.TagE = row["TagE"].ToString();
                item.TagF = row["TagF"].ToString();

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



            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                 assetTypeInfos.Add(row["AssetType"].ToString());
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


                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    deviceTypeInfos.Add(row["DeviceType"].ToString());
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

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

           

            int i = 0;

            foreach (var row in rows)
            {
                                var item = new AssetViewModel();
                i++;
                item.Index = i;

                item.AssetId = row["AssetId"].ToString();
                item.AssetQrCode = row["AssetQrCode"].ToString();
                item.AssetType = row["AssetType"].ToString();
                item.DeviceType = row["DeviceType"].ToString();
                item.AssetNumber = row["AssetTag"].ToString() + row["AssetNumber"].ToString();


                string purchaseDate = row["PurchaseDate"].ToString();
                DateTime time1;
                if (purchaseDate.Length > 0)
                {
                    time1 = DateTime.Parse(row["PurchaseDate"].ToString());
                    item.PurchaseDate = time1.ToString("d");
                }
                else
                {

                    item.PurchaseDate = null;
                }



               

                item.PurchasePrice = row["PurchasePrice"].ToString();
                item.Manufacturer = row["Manufacturer"].ToString();
                item.Model = row["Model"].ToString();
                item.SerialNumber = row["SerialNumber"].ToString();
                item.Configuration = row["Configuration"].ToString();
                item.Location = row["Location"].ToString();
                item.UserOrganization = row["UserOrganization"].ToString();
                item.UserDepartment = row["UserDepartment"].ToString();
                item.User = row["User"].ToString();
                item.UserPhone = row["UserPhone"].ToString();
                item.Consumer = row["Consumer"].ToString();
                item.Status = row["Status"].ToString();
                item.UsedYear = row["UsedYear"].ToString();

                string timeStr = row["ScrapDate"].ToString();

                if (timeStr.Length > 3)
                {
                    DateTime time2 = DateTime.Parse(timeStr);
                    item.ScrapDate = time2.ToString("d");

                }
                else
                {
                    item.ScrapDate = null;
                }



                item.Notes = row["Notes"].ToString();
                item.TagA = row["TagA"].ToString();
                item.TagB = row["TagB"].ToString();
                item.TagC = row["TagC"].ToString();
                item.TagD = row["TagD"].ToString();
                item.TagE = row["TagE"].ToString();
                item.TagF = row["TagF"].ToString();

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
