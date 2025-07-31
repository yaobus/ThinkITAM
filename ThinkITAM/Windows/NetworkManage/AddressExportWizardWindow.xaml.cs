using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
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
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddressExportWizardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddressExportWizardWindow : Window
    {
        public AddressExportWizardWindow()
        {
            InitializeComponent();
            NetworkInfosDataGrid.ItemsSource = networkInfos;
        }


        private ObservableCollection<NetworkInfoViewMode>
            networkInfos = new ObservableCollection<NetworkInfoViewMode>();

        /// <summary>
        /// 加载网段信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressExportWizardWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadNetworkInfo();

        }


        /// <summary>
        /// 加载网段信息
        /// </summary>
        private void LoadNetworkInfo()
        {
            networkInfos.Clear();

            string query = "SELECT * FROM Network WHERE Del IS NULL OR Del != 1 ;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                var info = new NetworkInfoViewMode();

                info.Index = index;
                info.NetworkId = row["NetworkId"].ToString();
                info.Name = row["Name"].ToString();
                info.Network = row["Network"].ToString();
                info.Netmask = row["Netmask"].ToString();
                info.Description = row["Description"].ToString();

                networkInfos.Add(info);
            }

        }


        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {

            // 使用LINQ查询筛选出IsSelected为true的所有项
            var selectedItems = networkInfos.Where(item => item.IsSelected == true).ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("请选择要导出的网段信息", "未选择要导出的数据", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            //要导出的全部表名
            var exportNetworkInfos = new ObservableCollection<ExportNetworkInfoClass>();


            //获取网段信息及网段自定义字段信息，并导出
            foreach (var item in selectedItems)
            {
                //获取子网掩码位数
                int netmask = SubnetCalculator.SubnetMaskToLength(item.Netmask);


                if (netmask >= 24) //小型网段
                {
                    //网段信息
                    var info = new ExportNetworkInfoClass();

                    info.Network = item.Network;
                    info.Netmask = item.Netmask;
                    info.TableName = $"Net_{item.NetworkId}";

                    //获取自定义字段
                    var tags = LoadCustomTag(info.TableName);
                    info.WindowTags = tags;
                    exportNetworkInfos.Add(info);
                }
                else //大型网段
                {
                    (string baseSubnet, ObservableCollection<string> subnetsRanges) =
                        SubnetCalculator.CalculateSubnets(item.Network, netmask);

                    int subIndex = 0;

                    foreach (string range in subnetsRanges)
                    {
                        subIndex++;

                        var name = $"Net_{item.NetworkId}_Sub{subIndex - 1}";

                        //网段信息
                        var info = new ExportNetworkInfoClass();

                        info.Network = item.Network;
                        info.Netmask = item.Netmask;
                        info.TableName = name;

                        //获取自定义字段
                        var tags = LoadCustomTag($"Net_{item.NetworkId}");
                        info.WindowTags = tags;

                        exportNetworkInfos.Add(info);

                    }

                }

                //获取所有表对应的表的数据
                foreach (var net in exportNetworkInfos)
                {
                    var data = GetNetworkAddressInfo(net.TableName);

                    A
                }

            }



        }



        /// <summary>
        /// 加载IP地址信息自定义标签
        /// </summary>
        private dynamic LoadCustomTag(string windowName)
        {
            var tagWindow = "IpAddressInfoTag" + windowName;

            string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE Window ='{tagWindow}'";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);



            if (num > 0) //存在本地自定义标签
            {
                var tags = DbClass.LoadWindowTag(tagWindow);

                if (tags != null)
                {
                    return JsonConvert.DeserializeObject(tags);


                }


            }
            else //全局标签
            {
                var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

                if (tags != null)
                {
                    return JsonConvert.DeserializeObject(tags);

                }


            }


            return null;
        }




        /// <summary>
        /// 获取指定表名的网段地址信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private ObservableCollection<IpAddressInfoListViewMode> GetNetworkAddressInfo(string tableName)
        {
            ObservableCollection < IpAddressInfoListViewMode > Infos = new ObservableCollection<IpAddressInfoListViewMode>();

            string query =$"SELECT  {tableName}.*,  UserInfo.Name,  UserInfo.Organization,  UserInfo.Department,  UserInfo.UserGroup, UserInfo.UserUnit,  UserInfo.Phone, Asset.AssetTag,  Asset.AssetNumber FROM  {tableName}  LEFT JOIN  UserInfo  ON  {tableName}.User = UserInfo.UserId LEFT JOIN  Asset  ON  {tableName}.LinkDevice = Asset.AssetId  ORDER BY Address ASC;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);



            int index2 = 0;

            foreach (var row in rows)
            {

                var info = new IpAddressInfoListViewMode();

                info.Index = index2;
                index2++;

                info.Address = Convert.ToInt32(row["Address"].ToString());
                int status = Convert.ToInt32(row["AddressStatus"].ToString());

                info.AddressStatus = status;

                Brush brush;


                if (status == 0 || status == 4)
                {
                    info.AddressType = false;
                }
                else
                {
                    info.AddressType = true;
                }


                try
                {
                    info.AddressColor = row["AddressColor"] != DBNull.Value ? Convert.ToInt32(row["AddressColor"]) : 0;
                }
                catch (Exception e)
                {
                    info.AddressColor = 0;
                }


                info.PingTime = "N/A";
                info.PingStatusColor = Brushes.Azure;
                info.User = row["User"].ToString();
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Group = row["UserGroup"].ToString();
                info.Unit = row["UserUnit"].ToString();
                info.Phone = row["Phone"].ToString();
                info.HostName = row["HostName"].ToString();
                info.MacAddress = row["MacAddress"].ToString();
                info.LinkDeviceAssetTag = row["AssetTag"].ToString();
                info.LinkDeviceAssetNumber = row["AssetNumber"].ToString();
                info.LinkDevice = info.LinkDeviceAssetTag + info.LinkDeviceAssetNumber;
                info.LinkDeviceId = row["LinkDevice"].ToString();
                info.TagA = row["TagA"].ToString();
                info.TagB = row["TagB"].ToString();
                info.TagC = row["TagC"].ToString();
                info.TagD = row["TagD"].ToString();
                info.TagE = row["TagE"].ToString();
                info.TagF = row["TagF"].ToString();


                Infos.Add(info);

            }

            return Infos;
        }
    }
}

