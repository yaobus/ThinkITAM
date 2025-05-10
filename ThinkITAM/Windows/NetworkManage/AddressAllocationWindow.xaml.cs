using System.Collections.ObjectModel;
using System.Net;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.NetworkManage;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Preset;
using MaterialDesignThemes.Wpf;
using Nmap.NET.Container;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;

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
            infos = addressInfos;
        }

        private ObservableCollection<IpAddressInfoListViewMode> infos =
            new ObservableCollection<IpAddressInfoListViewMode>();

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



            // 使用LINQ查询筛选出IsSelected为true的所有项
            var selectedItems = infos.Where(item => item.IsSelected == true).ToList();

            // 如果需要，可以将这些筛选出来的项放入一个新的ObservableCollection中
            ObservableCollection<IpAddressInfoListViewMode> selectedItemsCollection = new ObservableCollection<IpAddressInfoListViewMode>(selectedItems);


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



            #region 废案





            //if (DataBridge.DataBridge.AddressStatus == 1)//选择了未分配地址，准备分配
            //{

            //    if (DataBridge.DataBridge.SelectAddress.Count == 1)//单选模式
            //    {

            //        int address = DataBridge.DataBridge.SelectAddress[0];

            //        SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + address;

            //        //加载当前分配信息
            //        var info = DataBridge.DataBridge.IpAddressInfoLists.FirstOrDefault(d => d.Address == address);

            //        NowHostName.Text = info.NowHostName;
            //        NowMacAddress.Text = info.NowMacAddress;



            //        if (info.NowHostName != null)
            //        {
            //            if (info.HostName.Length == 0 && (info.NowHostName.Length > 0 && info.NowHostName != "N/A"))
            //            {
            //                HostNameBox.Text = info.NowHostName;

            //            }
            //        }

            //        if (info.NowMacAddress != null)
            //        {
            //            if (info.MacAddress.Length == 0 && (info.NowMacAddress.Length > 0) && info.NowMacAddress != "N/A")
            //            {
            //                MacAddressBox.Text = info.NowMacAddress;

            //            }
            //        }


            //    }
            //    else
            //    {
            //        StatusPing.IsEnabled = false;
            //        CopyHostInfo.IsEnabled = false;



            //        if (DataBridge.DataBridge.SelectAddress.Count > 7)//数量太多，仅显示一部分
            //        {
            //            string str = null;
            //            int i = 0;
            //            foreach (var ip in DataBridge.DataBridge.SelectAddress)
            //            {
            //                i++;
            //                str += $"{ip}、";
            //                if (i == 6)
            //                {
            //                    break;
            //                }
            //            }
            //            str = str.Substring(0, str.Length - 1);//删除最后一个顿号
            //            SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str + $"等{DataBridge.DataBridge.SelectAddress.Count}个地址";
            //        }
            //        else//全部显示
            //        {
            //            string str = null;
            //            foreach (var ip in DataBridge.DataBridge.SelectAddress)
            //            {
            //                str += $"{ip}、";

            //            }
            //            str = str.Substring(0, str.Length - 1);//删除最后一个顿号
            //            SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str;
            //        }


            //    }

            //}
            //else//2、修改或删除分配
            //{
            //    if (DataBridge.DataBridge.AddressStatus == 3)
            //    {
            //        IsAllocation.IsChecked = false;
            //    }


            //    EditMode = false;

            //    ClearButton.Visibility = Visibility.Visible;
            //    //2.1加载当前分配信息

            //    if (DataBridge.DataBridge.SelectAddress.Count == 1)//单个模式
            //    {
            //        SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + DataBridge.DataBridge.SelectAddress[0];

            //        int address = DataBridge.DataBridge.SelectAddress[0];


            //        //加载当前分配信息
            //        var info = DataBridge.DataBridge.IpAddressInfoLists.FirstOrDefault(d => d.Address == address);

            //        if (info != null)
            //        {
            //            People.Text = $"{info.Organization}-{info.Department}-{info.User}";
            //            Organization.Text = info.Organization;
            //            Department.Text = info.Department;
            //            Phone.Text = info.Phone;
            //            HostNameBox.Text = info.HostName;
            //            MacAddressBox.Text = info.MacAddress;
            //            LinkAsset.Text = info.LinkDeviceAssetTag+info.LinkDeviceAssetNumber;
            //            TagA.Text = info.TagA;
            //            TagB.Text = info.TagB;
            //            TagC.Text = info.TagC;
            //            TagD.Text = info.TagD;
            //            TagE.Text = info.TagE;
            //            TagF.Text = info.TagF;

            //        }



            //    }
            //    else//多个模式
            //    {


            //        if (DataBridge.DataBridge.SelectAddress.Count > 7)//数量太多，仅显示一部分
            //        {
            //            string str = null;
            //            int i = 0;
            //            foreach (var ip in DataBridge.DataBridge.SelectAddress)
            //            {
            //                i++;
            //                str += $"{ip}、";
            //                if (i == 6)
            //                {
            //                    break;
            //                }
            //            }
            //            str = str.Substring(0, str.Length - 1);//删除最后一个顿号
            //            SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str + $"等{DataBridge.DataBridge.SelectAddress.Count}个地址";
            //        }
            //        else//全部显示
            //        {
            //            string str = null;
            //            foreach (var ip in DataBridge.DataBridge.SelectAddress)
            //            {
            //                str += $"{ip}、";

            //            }
            //            str = str.Substring(0, str.Length - 1);//删除最后一个顿号
            //            SelectedAddress.Text = DataBridge.DataBridge.SelectNetwork + str;
            //        }


            //        //多个模式，只能同时解除是否启用分配，或者一键清空所选地址的分配信息

            //        int address = DataBridge.DataBridge.SelectAddress[0];
            //        //加载当前分配信息
            //        var info = DataBridge.DataBridge.IpAddressInfoLists.FirstOrDefault(d => d.Address == address);

            //        if (info.Status == 2)
            //        {
            //            IsAllocation.IsChecked = true;
            //        }
            //        else
            //        {
            //            IsAllocation.IsChecked = false;
            //        }

            //    }


            //}



            #endregion




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
            FindAssetWindow findAsset = new FindAssetWindow();
            findAsset.Owner = this;
            if (findAsset.ShowDialog() == true)
            {
                LinkAsset.Text = DataBridge.DataBridge.LinkSelectAssetId;
                // LoadTags();

            }
        }


        /// <summary>
        /// 保存分配信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            #region 废案


            //if (EditMode == true)
            //{
            //    //1、检查必要信息输入是否完整
            //    var info = CheckInput();

            //    if (info.Item1 > 0)
            //    {

            //        MessageBox.Show(info.Item2, "有问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);

            //        return;
            //    }


            //    //2、检查资产编号是否存在
            //    if (LinkAsset.Text.Length > 0)
            //    {
            //        string tag = LinkAsset.Text;

            //        string assetNumber = FunctionClass.AssetNumberCutClass.GetAssetNumberLastPart(tag);
            //        string assetTag = FunctionClass.AssetNumberCutClass.GetAssetNumberTagPart(tag);

            //        int num = 0;

            //        if (assetNumber != null && assetTag != null)
            //        {
            //            num = Convert.ToInt32(assetNumber);

            //            string sqlTemp = $"SELECT COUNT(*) FROM Asset WHERE AssetTag ='{assetTag}' AND AssetNumber={num}";

            //            var countNum = DbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

            //            if (countNum == 0)
            //            {
            //                MessageBox.Show("未发现资产编号对应的资产，请核实", "有问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            //                return;
            //            }

            //        }

            //    }

            //    //3、写入信息

            //    int status = 3;

            //    if (IsAllocation.IsChecked == true)
            //    {
            //        status = 2;
            //    }

            //    string tableName = DataBridge.DataBridge.NetworkTableName;

            //    string[] userSplit = People.Text.Split('-');
            //    string user = userSplit[2];


            //    string organization = Organization.Text;
            //    string department = Department.Text;
            //    string phone = Phone.Text;
            //    string hostName = HostNameBox.Text;
            //    string macAddress = MacAddressBox.Text;
            //    string linkAssetId = DataBridge.DataBridge.LinkAssetId;
            //    string tagA = TagA.Text;
            //    string tagB = TagB.Text;
            //    string tagC = TagC.Text;
            //    string tagD = TagD.Text;
            //    string tagE = TagE.Text;
            //    string tagF = TagF.Text;


            //    if (DataBridge.DataBridge.SelectAddress.Count == 1)
            //    {
            //        string sql = $"UPDATE \"{tableName}\" SET \"Status\" = {status}, \"User\" = '{user}', \"Organization\" = '{organization}', \"Department\" = '{department}', \"Phone\" = '{phone}', \"HostName\" = '{hostName}', \"MacAddress\" = '{macAddress}', \"LinkDevice\" = '{linkAssetId}', \"TagA\" = '{tagA}', \"TagB\" = '{tagB}', \"TagC\" = '{tagC}', \"TagD\" = '{tagD}', \"TagE\" = '{tagE}', \"TagF\" = '{tagF}' WHERE Address = {DataBridge.DataBridge.SelectAddress[0]}";

            //        dbClass.ExecuteQuery(sql);

            //        int address = DataBridge.DataBridge.SelectAddress[0];

            //        DataBridge.DataBridge.IpAddressInfoLists[address].IsSelected = false;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].Status = status;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].User = user;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].Organization = organization;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].Department = department;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].Phone = phone;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].HostName = hostName;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].MacAddress = macAddress;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].LinkDevice;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagA = tagA;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagB = tagB;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagC = tagC;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagD = tagD;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagE = tagE;
            //        DataBridge.DataBridge.IpAddressInfoLists[address].TagF = tagF;




            //    }
            //    else
            //    {
            //        // 创建SelectAddress的副本,避免修改原始集合
            //        var addressesCopy = new ObservableCollection<int>(DataBridge.DataBridge.SelectAddress);

            //        foreach (var address in addressesCopy)
            //        {
            //            string sql = $"UPDATE \"{tableName}\" SET \"Status\" = {status}, \"User\" = '{user}', \"Organization\" = '{organization}', \"Department\" = '{department}', \"Phone\" = '{phone}', \"HostName\" = '{hostName}', \"MacAddress\" = '{macAddress}', \"LinkDevice\" = '{linkAsset}', \"TagA\" = '{tagA}', \"TagB\" = '{tagB}', \"TagC\" = '{tagC}', \"TagD\" = '{tagD}', \"TagE\" = '{tagE}', \"TagF\" = '{tagF}' WHERE Address = {address}";
            //            dbClass.ExecuteQuery(sql);


            //            DataBridge.DataBridge.IpAddressInfoLists[address].IsSelected = false;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].Status = status;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].User = user;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].Organization = organization;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].Department = department;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].Phone = phone;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].HostName = hostName;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].MacAddress = macAddress;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].LinkDevice = linkAsset;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagA = tagA;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagB = tagB;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagC = tagC;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagD = tagD;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagE = tagE;
            //            DataBridge.DataBridge.IpAddressInfoLists[address].TagF = tagF;


            //        }


            //    }




            //}
            //else
            //{
            //    string tableName = DataBridge.DataBridge.NetworkTableName;

            //    int status = 3;

            //    if (IsAllocation.IsChecked == true)
            //    {
            //        status = 2;
            //    }


            //    foreach (var ip in DataBridge.DataBridge.SelectAddress)
            //    {
            //        string sql = $"UPDATE \"{tableName}\" SET \"Status\" = {status}  WHERE Address = {ip}";



            //        dbClass.ExecuteQuery(sql);
            //    }

            //}

            #endregion

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
                name= DataBridge.DataBridge.SelectPeopleViewModel.Name;
                organization = DataBridge.DataBridge.SelectPeopleViewModel.Organization;
                department = DataBridge.DataBridge.SelectPeopleViewModel.Department;
                group = DataBridge.DataBridge.SelectPeopleViewModel.Group;
                phone = DataBridge.DataBridge.SelectPeopleViewModel.Phone;
            }
            else
            {
                userId= oldInfo.User;
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
                item.Status = status;
                item.AddressColor = PortColor.SelectedIndex;
                item.User = userId;
                item.Name = name;
                item.Organization = organization;
                item.Department = department;
                item.Group = group;
                item.Phone = phone;
                item.HostName = HostNameBox.Text;
                item.MacAddress = MacAddressBox.Text;
                item.LinkDevice = assetId;
                item.TagA = TagA.Text;
                item.TagB = TagB.Text;
                item.TagC = TagC.Text;
                item.TagD = TagD.Text;
                item.TagE = TagE.Text;
                item.TagF = TagF.Text;

                SaveInfoToDb(item);
            }

            this.DialogResult = true;

            this.Close();

        }

        private void SaveInfoToDb(IpAddressInfoListViewMode info)
        {
            string tableName = DataBridge.DataBridge.NetworkTableName;

            string sql = $"UPDATE \"{tableName}\" SET \"User\" = '{info.User}', \"Status\" = '{info.Status}', \"AddressColor\" = '{info.AddressColor}', \"HostName\" = '{info.HostName}', \"MacAddress\" = '{info.MacAddress}', \"LinkDevice\" = '{info.LinkDevice}', \"TagA\" = '{info.TagA}', \"TagB\" = '{info.TagB}', \"TagC\" = '{info.TagC}', \"TagD\" = '{info.TagD}', \"TagE\" = '{info.TagE}', \"TagF\" = '{info.TagF}' WHERE Address = {info.Address}";


            //Console.WriteLine(sql);
            
            GlobalVariables.DbService.ExecuteNonQuery(sql);

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

                    string sql = $"UPDATE \"{tableName}\" SET \"Status\" = 1, \"User\" = '', \"AddressColor\"='0', \"HostName\" = '', \"MacAddress\" = '', \"LinkDevice\" = '', \"TagA\" = '', \"TagB\" = '', \"TagC\" = '', \"TagD\" = '', \"TagE\" = '', \"TagF\" = '' WHERE Address = {address}";
                  
                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    DataBridge.DataBridge.IpAddressInfoLists[address].Status = 1;
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


            //string hostName = await FunctionClass.DeviceInfoUpdater.GetHostNameFromIpAsync(SelectedAddress.Text);
            //string macAddress = await FunctionClass.DeviceInfoUpdater.GetMacAddress(SelectedAddress.Text);

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
