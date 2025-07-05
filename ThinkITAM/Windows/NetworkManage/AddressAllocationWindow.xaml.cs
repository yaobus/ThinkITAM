using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddressAllocationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddressAllocationWindow : Window
    {

        // 定义一个事件，本窗口关闭时触发
        // 定义一个带布尔值参数的事件
        public event EventHandler<BoolEventArgs> AddressAllocationWindowClosed;


        public AddressAllocationWindow(ObservableCollection<IpAddressInfoListViewMode> addressInfos = null)
        {
            InitializeComponent();

            if (addressInfos != null)
            {
                infos = addressInfos;
            }


        }

        private ObservableCollection<IpAddressInfoListViewMode> infos;

        /// <summary>
        /// 编辑模式，True为一般分配模式，flase为启用禁用模式
        /// </summary>
        private bool EditMode = true;

        private void AddressAllocationWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            //清空可能在上一次打开窗口产生的关联数据
            DataBridge.DataBridge.SelectPeopleViewModel = null;//用户关联ID
            DataBridge.DataBridge.SelectAssetInfo = null;//资产关联ID




            //加载标签
            LoadTags();


            if (infos != null)
            {
                // 使用LINQ查询筛选出IsSelected为true的所有项
                var selectedItems = infos.Where(item => item.IsSelected == true).ToList();

                // 将这些筛选出来的项放入一个新的ObservableCollection中
                ObservableCollection<IpAddressInfoListViewMode> selectedItemsCollection = new ObservableCollection<IpAddressInfoListViewMode>(selectedItems);

                //选择多个IP的时候无法关联资产
                if (selectedItemsCollection.Count > 1)
                {
                    FindAsset.IsEnabled = false;
                }
                else
                {
                    FindAsset.IsEnabled = true;
                }

                this.DataContext = selectedItemsCollection[0];


                if (selectedItemsCollection.Count > 7)//数量太多，仅显示一部分
                {
                    string str = null;
                    int i = 0;
                    foreach (var item in selectedItemsCollection)
                    {
                        i++;
                        str += $"{item.Address}、";
                        if (i == 6)
                        {
                            break;
                        }
                    }
                    str = str.Substring(0, str.Length - 1);//删除最后一个顿号
                    SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str + $"等{DataBridge.DataBridge.SelectAddress.Count}个地址";
                }
                else//全部显示
                {
                    string str = null;
                    foreach (var item in selectedItemsCollection)
                    {
                        str += $"{item.Address}、";

                    }
                    str = str.Substring(0, str.Length - 1);//删除最后一个顿号
                    SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str;
                }

            }



            NowNetworkName.Text = DataBridge.DataBridge.SelectNetworkInfo.Name;
            NowNetwork.Text = DataBridge.DataBridge.SelectNetworkInfo.Network;
            NowMaskText.Text = DataBridge.DataBridge.SelectNetworkInfo.Netmask;





        }

        /// <summary>
        /// 加载网段自定义标签
        /// </summary>
        private void LoadTags()
        {
            var tags = DataBridge.DataBridge.SelectNetworkTags;

            if (tags != null)
            {
                LabelA.Content = tags.TagA;
                LabelB.Content = tags.TagB;

                LabelC.Content = tags.TagC;
                LabelD.Content = tags.TagD;

                LabelE.Content = tags.TagE;
                LabelF.Content = tags.TagF;
            }




        }





        /// <summary>
        /// 查找资产被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAsset_Click(object sender, RoutedEventArgs e)
        {
            FindAssetWindow findAsset = new FindAssetWindow(null, 0);

            findAsset.Owner = this;

            if (findAsset.ShowDialog() == true)
            {
                //查询选择的资产是否已关联IP地址

                //如果关联的资产ID在Computer表中，则写入IP信息到Computer表中LinkIp字段
                var query = $"SELECT LinkIp FROM Computer WHERE AssetId = '{DataBridge.DataBridge.LinkAssetId}'";

                if (GlobalVariables.DbService.ExecuteScalar(query) != null)
                {
                    var ip = GlobalVariables.DbService.ExecuteScalar(query).ToString();
                    var result = MessageBox.Show($"该终端设备已关联IP地址{ip}\r继续保存将覆盖已关联的地址\r是否继续？", "已关联地址", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        LinkAsset.Text = DataBridge.DataBridge.LinkSelectAssetId;
                    }
                    else
                    {
                        DataBridge.DataBridge.SelectAssetInfo = null;
                        DataBridge.DataBridge.LinkAssetId = null;
                        DataBridge.DataBridge.LinkSelectAssetId = null;
                    }

                }
                else
                {
                    LinkAsset.Text = DataBridge.DataBridge.LinkSelectAssetId;
                }





                // LoadTags();

            }
        }


        /// <summary>
        /// 保存分配信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            var oldInfo = this.DataContext as IpAddressInfoListViewMode;

            //取出用户ID
            string userId = null;
            string name = null;
            string organization = null;
            string department = null;
            string group = null;
            string phone = null;

            if (DataBridge.DataBridge.SelectPeopleViewModel != null)
            {
                userId = DataBridge.DataBridge.SelectPeopleViewModel.UserId;
                name = DataBridge.DataBridge.SelectPeopleViewModel.Name;
                organization = DataBridge.DataBridge.SelectPeopleViewModel.Organization;
                department = DataBridge.DataBridge.SelectPeopleViewModel.Department;
                group = DataBridge.DataBridge.SelectPeopleViewModel.Group;
                phone = DataBridge.DataBridge.SelectPeopleViewModel.Phone;
            }
            else
            {
                userId = oldInfo.User;
            }

            //取出资产ID
            string assetId = null;


            if (DataBridge.DataBridge.SelectAssetInfo != null)
            {
                assetId = DataBridge.DataBridge.SelectAssetInfo.AssetId;
            }
            else
            {
                assetId = oldInfo.LinkDeviceId;
            }



            // 使用LINQ查询筛选出IsSelected为true的所有项
            var selectedItems = infos.Where(item => item.IsSelected == true).ToList();
            // 如果需要，可以将这些筛选出来的项放入一个新的ObservableCollection中
            ObservableCollection<IpAddressInfoListViewMode> selectedItemsCollection = new ObservableCollection<IpAddressInfoListViewMode>(selectedItems);

            //地址启用和未启用状态
            int status = 3;

            if (IsAllocation.IsChecked == true)
            {
                status = 2;
            }

            foreach (var item in selectedItemsCollection)
            {
                item.AddressStatus = status;
                item.AddressColor = PortColor.SelectedIndex;
                item.User = userId;
                item.Name = name;
                item.Organization = organization;
                item.Department = department;
                item.Group = group;
                item.Phone = phone;
                item.HostName = HostNameBox.Text;
                item.MacAddress = MacAddressBox.Text;
                item.LinkDeviceId = assetId;
                item.TagA = TagA.Text;
                item.TagB = TagB.Text;
                item.TagC = TagC.Text;
                item.TagD = TagD.Text;
                item.TagE = TagE.Text;
                item.TagF = TagF.Text;

                SaveInfoToDb(item);

                //让资产表单当前显示资产编号，而不是资产ID
                item.LinkDevice = DataBridge.DataBridge.LinkSelectAssetId;

            }



            this.DialogResult = true;



        }

        private void SaveInfoToDb(IpAddressInfoListViewMode info)
        {
            string tableName = DataBridge.DataBridge.NetworkTableName;

            string sql = $"UPDATE {tableName} SET User = '{info.User}', AddressStatus = '{info.AddressStatus}', AddressColor = '{info.AddressColor}', HostName = '{info.HostName}', MacAddress = '{info.MacAddress}', LinkDevice = '{info.LinkDeviceId}', TagA = '{info.TagA}', TagB = '{info.TagB}', TagC = '{info.TagC}', TagD = '{info.TagD}', TagE = '{info.TagE}', TagF = '{info.TagF}' WHERE Address = {info.Address}";


            GlobalVariables.DbService.ExecuteNonQuery(sql);


            //如果关联的资产ID在Computer表中，则写入IP信息到Computer表中LinkIp字段
            var query = $" SELECT COUNT (*) FROM Computer WHERE AssetId = '{info.LinkDeviceId}'";

            var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

            if (count > 0)
            {
                query = $"UPDATE Computer SET LinkIp = '{DataBridge.DataBridge.SelectNetwork}{info.Address}' WHERE AssetId = '{info.LinkDeviceId}'";

                GlobalVariables.DbService.ExecuteNonQuery(query);
            }

        }




        /// <summary>
        /// 检查输入是否完整
        /// </summary>
        /// <returns></returns>
        private (int, string) CheckInput()
        {
            int index = 0;
            string message = "当前存在以下问题需要解决:\r";


            string userId = DataBridge.DataBridge.SelectPeopleViewModel.UserId;//用户关联ID
            string assetId = DataBridge.DataBridge.SelectAssetInfo.AssetId;//资产关联ID




            if (People.Text.Length < 1)
            {
                index++;
                message += index.ToString() + ":未选择用户\r";
            }




            return (index, message);
        }







        // 自定义事件参数类
        public class BoolEventArgs : EventArgs
        {
            public bool Result
            {
                get;
            }

            public BoolEventArgs(bool result)
            {
                Result = result;
            }
        }



        /// <summary>
        /// 清空所选地址的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show($"注意！\r确定是否清空所选地址分配信息?\r此操作不可逆，是否继续？", "注意", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string tableName = DataBridge.DataBridge.NetworkTableName;


                // 使用LINQ查询筛选出IsSelected为true的所有项
                var selectedItems = infos.Where(item => item.IsSelected == true).ToList();
                // 如果需要，可以将这些筛选出来的项放入一个新的ObservableCollection中
                ObservableCollection<IpAddressInfoListViewMode> selectedItemsCollection = new ObservableCollection<IpAddressInfoListViewMode>(selectedItems);




                foreach (var item in selectedItemsCollection)
                {
                    int address = item.Address;

                    string sql = $"UPDATE {tableName} SET AddressStatus = 1, User = '', AddressColor='0', HostName = '', MacAddress = '', LinkDevice = '', TagA = '', TagB = '', TagC = '', TagD = '', TagE = '', TagF = '' WHERE Address = {address}";

                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    DataBridge.DataBridge.IpAddressInfoLists[address].AddressStatus = 1;
                    DataBridge.DataBridge.IpAddressInfoLists[address].User = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].AddressColor = 0;
                    DataBridge.DataBridge.IpAddressInfoLists[address].Organization = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].Department = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].Phone = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].HostName = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].MacAddress = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].LinkDevice = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagA = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagB = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagC = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagD = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagE = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].TagF = "";
                    DataBridge.DataBridge.IpAddressInfoLists[address].IsSelected = false;

                }

                this.DialogResult = true;
                this.Close();
            }


        }


        /// <summary>
        /// 对所选地址进行ping测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StatusPing_OnClick(object sender, RoutedEventArgs e)
        {

            ButtonProgressAssist.SetIsIndeterminate(StatusPing, true);

            string hostName = await DeviceInfoUpdater.GetHostNameFromIpAsync(SelectedAddress.Text);
            string macAddress = await DeviceInfoUpdater.GetMacAddress(SelectedAddress.Text);

            NowHostName.Text = hostName;
            NowMacAddress.Text = macAddress;
            ButtonProgressAssist.SetIsIndeterminate(StatusPing, false);

           
        }


        /// <summary>
        /// 提取获取到的主机名和MAC信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyHostInfo_Click(object sender, RoutedEventArgs e)
        {
            if (NowHostName.Text.Length > 0 && NowHostName.Text != "N/A")
            {
                HostNameBox.Text = NowHostName.Text;

            }

            if (NowMacAddress.Text.Length > 0 && NowMacAddress.Text != "N/A")
            {
                MacAddressBox.Text = NowMacAddress.Text;
            }
        }

        private void FindUser_OnClick(object sender, RoutedEventArgs e)
        {
            FindUserWindow findAsset = new FindUserWindow();
            findAsset.Owner = this;
            if (findAsset.ShowDialog() == true)
            {
                People.Text = DataBridge.DataBridge.SelectPeopleViewModel.Name;
                Organization.Text = DataBridge.DataBridge.SelectPeopleViewModel.Organization;
                Department.Text = DataBridge.DataBridge.SelectPeopleViewModel.Department;
                Group.Text = DataBridge.DataBridge.SelectPeopleViewModel.Group;
                Unit.Text = DataBridge.DataBridge.SelectPeopleViewModel.Unit;
                Phone.Text = DataBridge.DataBridge.SelectPeopleViewModel.Phone;


            }
        }

        private void PortColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PortColor.SelectedIndex;

            int x = 0;



            if (index != -1)
            {
                foreach (var selectedItem in PortColor.Items)
                {
                    var item = selectedItem as ListBoxItem;

                    if (item != null && index == x)
                    {
                        item.Opacity = 1;
                        item.BorderBrush = SystemColors.ActiveBorderBrush;
                        item.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        item.Opacity = 0.1;
                        item.BorderBrush = null;
                        item.BorderThickness = new Thickness(0);
                    }

                    x++;
                }

            }
        }
    }
}
