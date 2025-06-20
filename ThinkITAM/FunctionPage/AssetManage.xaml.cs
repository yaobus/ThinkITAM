using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ThinkITAM.Windows.AssetManage;
using ThinkITAM.UserControls.Asset;
using ThinkITAM.ViewModels.AssetManage;
using Newtonsoft.Json;
using QRCoder;
using ThinkITAM.DatabaseOperation;
using Size = System.Windows.Size;
using ThinkITAM.DataBridge;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using ThinkITAM.Windows.PresetWindows;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Functions.Export;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// AssetManage.xaml 的交互逻辑
    /// </summary>
    public partial class AssetManage : UserControl
    {
        public AssetManage()
        {
            InitializeComponent();
        }


       

        private void AssetManage_OnLoaded(object sender, RoutedEventArgs e)
        {


            LoadTags();

            // 加载资产树
            LoadAssetTreeviewInfos();
        }

        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {
            var tags =DbClass.LoadWindowTag("AddAsset");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;
                AssetTagCard.TagA.Content = settings.TagA;
            }

        }


        private ObservableCollection<AssetTypeViewModel> assetTypes = new ObservableCollection<AssetTypeViewModel>();

        private async void LoadAssetTreeviewInfos()
        {
            assetTypes.Clear();
            AssetTreeView.Items.Clear();

            string sqlTemp = $"SELECT COUNT(*) FROM AssetTag";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (num > 0)
            {
                string query = "SELECT DISTINCT AssetType FROM AssetTag ;";



                var rows = await GlobalVariables.DbService.ExecuteQueryAsync(query);



                int index = 0;


                foreach (var row in rows)
                {

                    index++;
                    var info = new AssetTypeViewModel();

                    info.Index = index;

                    string assetTypeInfo = row["AssetType"].ToString();

                    info.AssetType = assetTypeInfo;//资产类型

                    sqlTemp = $"SELECT * FROM AssetTag  WHERE AssetType = '{assetTypeInfo}'";


                    var rows2=await GlobalVariables.DbService.ExecuteQueryAsync(sqlTemp);


                    int index2 = 0;


                    var deviceTypeItems = new TreeViewItem();


                    foreach (var row2 in rows2)
                    {

                        index2++;
                        var device = new DeviceTypeUserControl();
                        var deviceInfo = new DeviceTypeViewModel();

                        deviceInfo.Index = index2;
                        deviceInfo.DeviceType = row2["DeviceType"].ToString();
                        deviceInfo.AssetType = assetTypeInfo;

                        string sql = $"SELECT COUNT(*) FROM Asset WHERE AssetType = '{assetTypeInfo}' AND DeviceType = '{deviceInfo.DeviceType}'";
                        deviceInfo.AssetCount = DbClass.ExecuteScalarTableNum(sql);

                        device.DataContext = deviceInfo;
                        deviceTypeItems.Items.Add(device);


                    }



                    info.DeviceTypeCount = "设备类型总数:" + index2;//设备类型总数

                    var assetTypeControl = new AssetTypeUserControl();
                    assetTypeControl.DataContext = info;

                    deviceTypeItems.Header = assetTypeControl;


                    AssetTreeView.Items.Add(deviceTypeItems);

                    await Task.Delay(50);

                    assetTypes.Add(info);



                }


 

                //Organization.ItemsSource = organizationInfo;







            }
        }

        private async void LoadAssetTreeviewInfos2(string keyWord=null)
        {
            assetTypes.Clear();

            AssetTreeView.Items.Clear();

            string filter = string.Empty;
            string filter2 = string.Empty;
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                filter = $" WHERE ( AssetTag LIKE '%{keyWord}%' OR Manufacturer LIKE '%{keyWord}%' OR Model LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%' OR TagA LIKE '%{keyWord}%')";

                filter2 = $" AND ( AssetTag LIKE '%{keyWord}%' OR Manufacturer LIKE '%{keyWord}%' OR Model LIKE '%{keyWord}%' OR User LIKE '%{keyWord}%' OR TagA LIKE '%{keyWord}%')";

            }


            var sql2 = $"SELECT DISTINCT AssetType , DeviceType FROM Asset {filter}";


            string sqlTemp = $"SELECT COUNT(*) FROM Asset {filter}";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (num > 0)
            {
                //string query = "SELECT DISTINCT AssetType FROM AssetTag ;";

                var rows = await GlobalVariables.DbService.ExecuteQueryAsync(sql2);



                int index = 0;


                foreach (var row in rows)
                {

                    index++;
                    var info = new AssetTypeViewModel();

                    info.Index = index;

                    string assetTypeInfo = row["AssetType"].ToString();
                    string deviceType2= row["DeviceType"].ToString();

                    info.AssetType = assetTypeInfo;//资产类型

                    sqlTemp = $"SELECT * FROM AssetTag WHERE AssetType = '{assetTypeInfo}' AND DeviceType ='{deviceType2}'";


                    var rows2 = await GlobalVariables.DbService.ExecuteQueryAsync(sqlTemp);


                    int index2 = 0;


                    var deviceTypeItems = new TreeViewItem();


                    foreach (var row2 in rows2)
                    {

                        index2++;
                        var device = new DeviceTypeUserControl();
                        var deviceInfo = new DeviceTypeViewModel();

                        deviceInfo.Index = index2;
                        deviceInfo.DeviceType = row2["DeviceType"].ToString();
                        deviceInfo.AssetType = assetTypeInfo;

                        string sql = $"SELECT COUNT(*) FROM Asset WHERE AssetType = '{assetTypeInfo}' AND DeviceType = '{deviceInfo.DeviceType}' {filter2}";
                        deviceInfo.AssetCount = DbClass.ExecuteScalarTableNum(sql);

                        device.DataContext = deviceInfo;
                        deviceTypeItems.Items.Add(device);


                    }



                    info.DeviceTypeCount = "设备类型总数:" + index2;//设备类型总数

                    var assetTypeControl = new AssetTypeUserControl();
                    assetTypeControl.DataContext = info;

                    deviceTypeItems.Header = assetTypeControl;


                    AssetTreeView.Items.Add(deviceTypeItems);

                    await Task.Delay(50);

                    assetTypes.Add(info);



                }




                //Organization.ItemsSource = organizationInfo;







            }
        }


        private string assetType;
        private string deviceType;
        

        private async void AssetTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null)
            {
                //重置全选复选框状态
                NumberBlock.Text = "0";
                HeaderCheckBox.IsChecked=false;
                


                AssetTreeView.IsEnabled = false;

                assetViewModels.Clear();

                var selectedNode = e.NewValue;

                if (selectedNode is DeviceTypeUserControl) //如果是设备类型
                {
                    // 如果选择的是设备类型节点，则处理子节点的逻辑
                    DeviceTypeUserControl childNode = selectedNode as DeviceTypeUserControl;

                    DeviceTypeViewModel info = childNode.DataContext as DeviceTypeViewModel;

                     deviceType = info.DeviceType;

                     assetType = info.AssetType;


                    LoadAssetInfos(assetType, deviceType);

                }
                else if (selectedNode is TreeViewItem) //如果是资产类型
                {
                    TreeViewItem selectedItem = selectedNode as TreeViewItem;


                    if (selectedItem != null)
                    {
                        // 判断节点是否展开
                        if (selectedItem.IsExpanded)
                        {
                            // 如果已经展开，就折叠节点
                            selectedItem.IsExpanded = false;
                        }
                        else
                        {
                            // 如果未展开，就展开节点
                            selectedItem.IsExpanded = true;
                        }
                    }


                    var selected = e.NewValue as TreeViewItem;


                    // 如果选择的是树节点类型，则处理树节点的逻辑
                    AssetTypeUserControl treeNode = (AssetTypeUserControl)selected.Header;

                    await LoadSelectAssetInfo(treeNode);



                }



                AssetTreeView.IsEnabled = true;

            }
        }


        /// <summary>
        /// 加载所选资产类型的全部资产信息
        /// </summary>
        /// <param name="treeNode"></param>
        private async Task LoadSelectAssetInfo(AssetTypeUserControl treeNode)
        {
            

            //将表名存到全局变量，便于其他地方调用
            //DataBridge.DataBridge.NetworkTableName = treeNode.TableName;

            //将当前选中的网段信息存到全局变量，便于其他地方调用

            var info = treeNode.DataContext as AssetTypeViewModel;

            DataBridge.DataBridge.SelectAssetTypeViewmodel = info; //父节点信息

            //Console.WriteLine(info.AssetType);

            assetType = info.AssetType;

            deviceType = null;

            LoadAssetInfos(assetType,deviceType);

            AssetDataGrid.ItemsSource = assetViewModels;
            //加载网段标签
            //LoadCustomTag();



            //加载网段备注
            // LoadNetworkNote(info);

            //try
            //{
            //    await LoadAddressInfo(treeNode.TableName);
            //}
            //catch
            //{
            //    MessageQueue.Enqueue("已选择大型网段，当前显示第一个分表");
            //    await LoadAddressInfo(treeNode.TableName + "_Sub0");

            //}





        }

        ObservableCollection<AssetViewModel> assetViewModels = new ObservableCollection<AssetViewModel>();

        private void LoadAssetInfos(string assetType, string? deviceType)
        {
            assetViewModels.Clear();
           
            string sql;

            if (deviceType!=null && deviceType.Replace(" ", "").Length > 0) //设备类型不为空
            {
                sql = $"SELECT * FROM Asset WHERE AssetType ='{assetType}' AND DeviceType='{deviceType}' AND  (Del != 1 OR Del IS NULL)";

            }
            else
            {
                sql = $"SELECT * FROM Asset WHERE AssetType ='{assetType}'  AND  (Del != 1 OR Del IS NULL)";
            }



            var rows =  GlobalVariables.DbService.ExecuteQuery(sql);

            int i = 0;


            foreach (var row in rows)
            {
                var item = new AssetViewModel();
                i++;
                item.Index = i;
                item.Id = Convert.ToInt32(row["Id"].ToString());
                item.AssetId = row["AssetId"].ToString();
                item.AssetQrCode = row["AssetQrCode"].ToString();
                item.AssetType = row["AssetType"].ToString();
                item.DeviceType = row["DeviceType"].ToString();

                item.AssetTag = row["AssetTag"].ToString();
                item.TagNumber = row["AssetNumber"].ToString();
                item.AssetNumber = item.AssetTag + item.TagNumber;

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


        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAssetTagWindow addAssetTag = new AddAssetTagWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addAssetTag.Owner = window;
            }

            if (addAssetTag.ShowDialog() == true)
            {

                LoadAssetTreeviewInfos();

            }



        }



        /// <summary>
        /// 当前选中的表项
        /// </summary>
        private AssetViewModel NowSelectedItem;

        /// <summary>
        /// 选择资产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssetDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (AssetInfoExpander.IsExpanded == true)
            //{
            if (AssetDataGrid.SelectedItem != null)
            {
                EditAssetButton.IsEnabled = true;
                DeleteAssetButton.IsEnabled = true;

                var selectedItem = (AssetViewModel)AssetDataGrid.SelectedItem;
                var info = selectedItem as AssetViewModel;

                NowSelectedItem = info;

                AssetTagCard.DataContext = info;

                Bitmap qrCodeImage = GenerateQRCode(info.AssetQrCode);

                AssetTagCard.BarcodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                    qrCodeImage.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()); ;
            }
            else
            {
                NowSelectedItem =null;
                EditAssetButton.IsEnabled = false;
                DeleteAssetButton.IsEnabled = false;
            }

            // }
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Bitmap GenerateQRCode(string text)
        {
            // 创建 QR code 生成器
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // 创建 QR code 数据
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
           
            
            // 生成 QR code 图像
            QRCode qrCode = new QRCode(qrCodeData);



            Bitmap qrCodeImage = qrCode.GetGraphic(20,"#2f9f9f","#FFFFFF" );

            return qrCodeImage;
        }


        /// <summary>
        /// 全选或者取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox HeaderCheckBox = sender as CheckBox;
            
            if (HeaderCheckBox != null)
            {
                if (HeaderCheckBox.IsChecked == true)
                {
                    foreach (var item in AssetDataGrid.Items)
                    {
                        (item as AssetViewModel).IsSelected = true;

                    }
                }
                else
                {
                    foreach (var item in AssetDataGrid.Items)
                    {
                        (item as AssetViewModel).IsSelected = false;

                    }
                }



                UpdateHeaderCheckBoxState();
            }
        }


        private void UpdateHeaderCheckBoxState()
        {
            int checkedCount = 0;

            foreach (var item in AssetDataGrid.Items)
            {

                if ((item as AssetViewModel).IsSelected)
                {
                    checkedCount++;
                }


            }

            bool allChecked = checkedCount == AssetDataGrid.Items.Count;
            bool noneChecked = checkedCount == 0;

            HeaderCheckBox.IsChecked = allChecked ? true : (noneChecked ? false : null);

            if (checkedCount > 0)
            {
                
                NumberBlock.Text = checkedCount.ToString();

                QrCodeExport.IsEnabled = true;
            }
            else
            {
                NumberBlock.Text = "0";
                QrCodeExport.IsEnabled = false;
            }


            

        }



        /// <summary>
        /// 添加选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {

            UpdateHeaderCheckBoxState();
        }

        /// <summary>
        /// 删除选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {


            UpdateHeaderCheckBoxState();
        }


        /// <summary>
        /// 一键导出资产标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QrCodeExport_OnClick(object sender, RoutedEventArgs e)
        {
            //弹出路径选项对话框

            var path = string.Empty;
            var title = (string)FindResource("AmSaveQrCodeTitle");
            // ToggleButton 选中，选择文件夹
            var dialog = new OpenFolderDialog
            {
                Title = $"{title}"
            };

            if (dialog.ShowDialog() == true)
            {
                path = dialog.FolderName;

                int index = 0;
                foreach (var item in assetViewModels)
                {
                    if (item.IsSelected == true)
                    {
                        string filePath = $"{path}/{item.AssetId}.png";

                        GenerateAssetTagImage(item, filePath);
                    }
                    index++;
                }

                var message = $"导出完毕,共{index}个文件\r是否打开文件夹？";

                var result = MessageBox.Show(message, "导出完毕", MessageBoxButton.OKCancel, MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    //打开导出路径
                    Process.Start("explorer.exe", path);
                }

            }


        }

        /// <summary>
        /// 生成资产标签图片
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public  void GenerateAssetTagImage(AssetViewModel data, string filePath)
        {
            // 创建控件实例
            var assetTagControl = new AssetTagTemplateUserControl();

            // 设置数据上下文
            assetTagControl.DataContext = data;


            Bitmap qrCodeImage = GenerateQRCode(data.AssetQrCode);

            assetTagControl.BarcodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                qrCodeImage.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()); ;




            // 测量和布置控件
            assetTagControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            assetTagControl.Arrange(new Rect(new Size(assetTagControl.DesiredSize.Width, assetTagControl.DesiredSize.Height)));
            assetTagControl.UpdateLayout();

            // 创建 RenderTargetBitmap
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)assetTagControl.ActualWidth, (int)assetTagControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            // 渲染控件内容
            bitmap.Render(assetTagControl);

            // 保存为 PNG 文件
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }



        private void AssetDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // 迭代视觉树以找到 DataGridRow
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            // 获取 DataGridRow
            DataGridRow row = dep as DataGridRow;
            if (row == null)
                return;

            // 获取行数据对象
            var rowData = row.Item as AssetViewModel;
            
            if (rowData != null)
            {
                // 逻辑代码
                RunOnDoubleClick(rowData);
            }


        }


        private void RunOnDoubleClick(AssetViewModel rowData)
        {
           

            AddAssetWindow addAssetWindow = new AddAssetWindow(rowData);


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addAssetWindow.Owner = window;
            }

            // addressAllocationWindow.AddressAllocationWindowClosed += AddressAllocationWindow_AddressAllocationWindowClosed;

            if (addAssetWindow.ShowDialog() == true)
            {
                //加载资产数据
                Console.WriteLine("加载资产数据");
            }
        }


        /// <summary>
        /// 清空搜索关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
        {
            SearchKeyWord.Text = null;

            LoadAssetTreeviewInfos();
        }


        /// <summary>
        /// 添加资产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAssetWindow addAsset = new AddAssetWindow(null);


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addAsset.Owner = window;
            }


            if (addAsset.ShowDialog() == true)
            {

                LoadAssetInfos(assetType, deviceType);
                //加载网段信息备注标签
                //LoadTags();
            }


        }

        private void EditAssetButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (NowSelectedItem != null)
            {
                // 逻辑代码
                RunOnDoubleClick(NowSelectedItem);
            }
        }

        private void DeleteAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = AssetDataGrid.SelectedIndex;

            if (index != -1)
            {
                
                var info = AssetDataGrid.SelectedItem as AssetViewModel;

                var message = $"确定要删除选中资产吗？\r资产类型:{info.AssetType}\r设备类型:{info.DeviceType}\r资产序列号:{info.AssetTag}{info.AssetNumber}\r型号:{info.Model}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE Asset SET Del = 1 WHERE AssetId ='{info.AssetId}';";


                    GlobalVariables.DbService.ExecuteNonQuery(query);


                    LoadAssetInfos(assetType, deviceType);
                }


            }
        }



        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchKeyWord.Text))
            {
                LoadAssetTreeviewInfos2(SearchKeyWord.Text);
            }
            else
            {
                LoadAssetTreeviewInfos();
            }

            
        }


        private void SearchKeyWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            SearchButton_OnClick(null, null);
        }



        /// <summary>
        /// 导出资产信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataExport_OnClick(object sender, RoutedEventArgs e)
        {


            if (assetViewModels == null || assetViewModels.Count == 0)
            {
                MessageBox.Show("没有可导出的数据。");
                return;
            }

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

            // 创建保存文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel 文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"AssetInfo{fileName}"  // 默认文件名
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = saveFileDialog.FileName;
                        

                // 调用导出方法
                ExcelExporter.ExportToExcel(assetViewModels, selectedFilePath);
            }

        }
    }
}
