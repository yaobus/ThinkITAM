using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ThinkITAM.Windows.AssetManage;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.UserControls.Asset;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.NetworkManage;
using Newtonsoft.Json;
using QRCoder;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;
using static MaterialDesignThemes.Wpf.Theme;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using CheckBox = System.Windows.Controls.CheckBox;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

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


        private DbClass dbClass;

        private void AssetManage_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();


            LoadTags();

            // 加载资产树
            LoadAssetTreeviewInfos();
        }

        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {
            var tags = dbClass.LoadWindowTag("AddAsset");

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

            var num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

            if (num > 0)
            {
                string query = "SELECT DISTINCT AssetType FROM AssetTag;";

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int index = 0;

                while (reader.Read())
                {
                    index++;
                    var info = new AssetTypeViewModel();

                    info.Index = index;

                    string assetTypeInfo = reader["AssetType"].ToString();

                    info.AssetType = assetTypeInfo;//资产类型

                    sqlTemp = $"SELECT * FROM AssetTag  WHERE AssetType = '{assetTypeInfo}'";

                    SQLiteCommand command2 = new SQLiteCommand(sqlTemp, dbClass.connection);
                    SQLiteDataReader reader2 = command2.ExecuteReader();

                    int index2 = 0;


                    var deviceTypeItems = new TreeViewItem();

                    while (reader2.Read())
                    {
                        index2++;
                        var device = new DeviceTypeUserControl();
                        var deviceInfo = new DeviceTypeViewModel();

                        deviceInfo.Index = index2;
                        deviceInfo.DeviceType = reader2["DeviceType"].ToString();
                        deviceInfo.AssetType = assetTypeInfo;

                        string sql = $"SELECT COUNT(*) FROM Asset WHERE AssetType = '{assetTypeInfo}'";
                        deviceInfo.AssetCount = dbClass.ExecuteScalarTableNum(sql, dbClass.connection);

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


                    //Console.WriteLine(assetType + " | " + deviceType);

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
                item.AssetQrCode= reader["AssetQrCode"].ToString();
                item.AssetType = reader["AssetType"].ToString();
                item.DeviceType = reader["DeviceType"].ToString();

                item.AssetTag = reader["AssetTag"].ToString();
                item.TagNumber = reader["AssetNumber"].ToString();
                item.AssetNumber = item.AssetTag + item.TagNumber;

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
                item.UserPhone= reader["UserPhone"].ToString();
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


        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {

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

            foreach (var item in assetViewModels)
            {
                if (item.IsSelected == true)
                {
                    string filePath = $"D:/Assets/{item.AssetId}.png";
                    GenerateAssetTagImage(item,filePath);
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

                // 当子窗口关闭后执行这里的代码



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
    }
}
