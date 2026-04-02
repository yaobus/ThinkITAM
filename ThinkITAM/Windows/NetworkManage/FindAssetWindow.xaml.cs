using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.AssetManage;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// FindAssetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindAssetWindow : Window
    {
        public string SelectAssetId = null;//传入的资产标签

        /// <summary>
        /// 资产查找
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="mode"></param>
        public FindAssetWindow(string assetId = null, int mode = 1)
        {
            InitializeComponent();

            if (assetId != null)
            {
                SelectAssetId = assetId;
            }




            FindAssetDataGrid.ItemsSource = assetViewModels;

            if (mode != 1)
            {
                loadMode = mode;
            }

        }

        private int loadMode = 1;

        private string filter = $"AND( Deploy IS NULL OR Deploy='') AND (Del != 1 OR Del IS NULL) ";

        private void FindAssetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            if (loadMode == 0)
            {
                FilterButton.IsChecked = false;
            }


            //加载全部资产信息
            LoadSelectAssetInfo();

            LoadAssetType();


            if (!string.IsNullOrWhiteSpace(SelectAssetId))
            {
                loadStatus = 1;
                LoadSelectAssetInfo(SelectAssetId);
                FindAssetDataGrid.SelectedIndex = 0;
                loadStatus = 0;
            }

            //加载表单列排序
            Functions.DataGridColumn.DataGridColumnOrderClass.LoadColumnOrder(FindAssetDataGrid);
        }

        //是否加载资产信息
        private int loadStatus = 0; //0：正常加载，1：不加载

        /// <summary>
        /// 通过传入的资产标签加载资产信息
        /// </summary>
        /// <param name="id"></param>
        private void LoadSelectAssetInfo(string? id = null)
        {
            //Console.WriteLine("01、开始加载资产信息...");
            

            assetViewModels.Clear();

            string sql;


            if (FilterButton.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $"SELECT * FROM Asset WHERE AssetId ='{id}' AND (Del != 1 OR Del IS NULL) ";
                }
                else
                {
                    sql = $"SELECT * FROM Asset WHERE Deploy IS NULL OR Deploy='' AND (Del != 1 OR Del IS NULL) ";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $"SELECT * FROM Asset WHERE AssetId ='{id}' AND (Del != 1 OR Del IS NULL)  ";
                }
                else
                {
                    sql = $"SELECT * FROM Asset WHERE (Del != 1 OR Del IS NULL) ";
                }
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

                //AssetType.Text = item.AssetType;
                //DeviceType.Text = item.DeviceType;


                var purchaseDate = row["PurchaseDate"].ToString();
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
                item.UserGroup = row["UserGroup"].ToString();
                item.User = row["User"].ToString();
                item.UserPhone = row["UserPhone"].ToString();
                item.Consumer = row["Consumer"].ToString();
                item.AssetStatus = row["AssetStatus"].ToString();
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


                //string assetType = assetTypeInfos[AssetType.SelectedIndex];

                //if (!string.IsNullOrWhiteSpace(assetType))
                //{
                //    LoadAssetInfos(assetType, null);
                //}




            }
            else
            {
                deviceTypeInfos.Clear();
            }
        }


        private void DeviceType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //if (DeviceType.SelectedIndex != -1 && loadStatus == 0)
            //{
            //    string assetType = assetTypeInfos[AssetType.SelectedIndex];

            //    string deviceType = deviceTypeInfos[DeviceType.SelectedIndex];

            //    LoadAssetInfos(assetType, deviceType);

            //}

        }

        ObservableCollection<AssetViewModel> assetViewModels = new ObservableCollection<AssetViewModel>();


        private void LoadAssetInfos(string? assetType, string? deviceType)
        {
            assetViewModels.Clear();

            string sql;

            if (FilterButton.IsChecked == true)
            {
                if (deviceType != null && deviceType.Replace(" ", "").Length > 0) //设备类型不为空
                {

                    sql = $"SELECT DISTINCT * FROM Asset WHERE AssetType ='{assetType}' AND DeviceType='{deviceType}' {filter}";

                }
                else
                {
                    sql = $"SELECT DISTINCT * FROM Asset WHERE AssetType ='{assetType}' {filter}";
                }

            }
            else
            {
                if (deviceType != null && deviceType.Replace(" ", "").Length > 0) //设备类型不为空
                {

                    sql = $"SELECT DISTINCT * FROM Asset WHERE AssetType ='{assetType}' AND DeviceType='{deviceType}'";

                }
                else
                {
                    sql = $"SELECT DISTINCT * FROM Asset WHERE AssetType ='{assetType}'";
                }

            }

            Console.WriteLine($"338:{sql}");

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
                item.UserGroup = row["UserGroup"].ToString();
                item.User = row["User"].ToString();
                item.UserPhone = row["UserPhone"].ToString();
                item.Consumer = row["Consumer"].ToString();
                item.AssetStatus = row["AssetStatus"].ToString();
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
            DataBridge.DataBridge.SelectAssetInfo = null;
            // 检查选中项是否非空，以避免空引用异常
            if (FindAssetDataGrid.SelectedItem != null)
            {
                // 通过 SelectedItem 属性获取选中的行数据
                var selectedRowData = FindAssetDataGrid.SelectedItem as AssetViewModel;

                DataBridge.DataBridge.SelectAssetInfo = selectedRowData;

                AssetTag.Text = selectedRowData.AssetNumber;
                Model.Text = selectedRowData.Model;
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

        }

        private void FilterButton_OnClick(object sender, RoutedEventArgs e)
        {


            //AssetTag.Text = string.Empty;
            //Model.Text = string.Empty;
            //Description.Text = string.Empty;

            //DataBridge.DataBridge.SelectAssetInfo = new();

            //LoadSelectAssetInfo();
            //AssetType.SelectedIndex = -1;
            //DeviceType.SelectedIndex = -1;

           
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var assetType = string.Empty;

            var deviceType = string.Empty;


            if (AssetType.SelectedIndex != -1)
            {
                 assetType = assetTypeInfos[AssetType.SelectedIndex];
            }

            if (DeviceType.SelectedIndex != -1)
            {
                deviceType = deviceTypeInfos[DeviceType.SelectedIndex];
            }


            if (string.IsNullOrWhiteSpace(assetType))
            {
                LoadSelectAssetInfo();
            }
            else
            {
                LoadAssetInfos(assetType, deviceType);
            }



            
        }

        private void AssetDataGrid_OnColumnReordered(object? sender, DataGridColumnEventArgs e)
        {
            Functions.DataGridColumn.DataGridColumnOrderClass.SaveColumnOrder(FindAssetDataGrid);
        }
    }
}
