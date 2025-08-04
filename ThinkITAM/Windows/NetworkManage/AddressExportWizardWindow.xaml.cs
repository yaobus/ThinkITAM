using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.Export;
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
        private async void ExportButton_OnClick(object sender, RoutedEventArgs e)
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
                        
                        info.NetworkId = item.NetworkId;
                        info.SubId = $"_Sub{subIndex - 1}";
                        info.Network = item.Network;
                        info.Netmask = item.Netmask;
                        info.TableName = name;
                        info.Range = range;
                        //获取自定义字段
                        var tags = LoadCustomTag($"Net_{item.NetworkId}");

                        info.WindowTags = tags;

                        exportNetworkInfos.Add(info);

                    }

                }





            }

            //导出

            var fileName = $"MultipleNetwork-{DateTime.Now.ToString("yyyyMMddHHmmss")}";

            // 创建保存文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel 文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = fileName  // 默认文件名
            };


            var progress = new Progress<(int Progress, string Message)>(report =>
            {
                ExportProgressBar.Value = report.Progress;
                ExportTextBlock.Text=report.Message;
            });


            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = saveFileDialog.FileName;


                // 调用导出方法
               await ExportMultipleSheetsToExcelAsync(exportNetworkInfos, selectedFilePath, progress);
                

            }



        }




        /// <summary>
        /// 异步将多个网络表数据导出到一个 Excel 文件的多个工作表中（支持进度报告）
        /// </summary>
        /// <param name="exportNetworkInfos">要导出的表信息列表</param>
        /// <param name="filePath">导出文件路径</param>
        /// <param name="progress">进度报告接口，用于更新UI</param>
        /// <param name="cancellationToken">取消令牌（可选）</param>
        /// <returns>任务</returns>
        /// <summary>
        /// 异步导出多个Sheet，支持进度报告（百分比 + 消息）
        /// </summary>
        private async Task ExportMultipleSheetsToExcelAsync(
            IEnumerable<ExportNetworkInfoClass> exportNetworkInfos,
            string filePath,
            IProgress<(int Progress, string Message)> progress, // (0-100, 状态文本)
            CancellationToken cancellationToken = default)
        {
            var exportList = exportNetworkInfos.ToList();
            int totalCount = exportList.Count;

            await Task.Run(async () =>
            {
                using (var workbook = new XLWorkbook())
                {
                    int currentSheet = 0;
                    foreach (var net in exportList)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        currentSheet++;
                        int progressPercent = (int)((double)currentSheet / totalCount * 100);
                        string message = $"正在导出 {net.TableName} ({currentSheet}/{totalCount})...";
                        progress?.Report((progressPercent, message));

                        try
                        {
                            var data = GetNetworkAddressInfo(net);
                            var headerNames = GetNetworkAddressHeader(net);

                            if (headerNames == null || headerNames.Count == 0)
                            {
                                progress?.Report((progressPercent, $"跳过空配置表：{net.TableName}"));
                                continue;
                            }

                            var sheetName = $"{GetNetworkNamePrefix(net)}X-({net.TableName})";
                            var worksheet = workbook.Worksheets.Add(sheetName);
                            var properties = typeof(IpAddressInfoListExportViewModel).GetProperties()
                                .Where(p => headerNames.ContainsKey(p.Name))
                                .ToArray();

                            // 写入表头
                            int colIndex = 1;
                            foreach (var kvp in headerNames)
                            {
                                if (properties.Any(p => p.Name == kvp.Key))
                                {
                                    worksheet.Cell(1, colIndex).Value = kvp.Value;
                                    colIndex++;
                                }
                            }

                            // 写入数据
                            int row = 2;
                            foreach (var item in data)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    cancellationToken.ThrowIfCancellationRequested();

                                colIndex = 1;
                                foreach (var kvp in headerNames)
                                {
                                    var property = properties.FirstOrDefault(p => p.Name == kvp.Key);
                                    if (property != null)
                                    {
                                        var value = property.GetValue(item);
                                        worksheet.Cell(row, colIndex).Value = value?.ToString() ?? "";
                                    }
                                    colIndex++;
                                }
                                row++;
                            }

                            worksheet.Columns().AdjustToContents();
                        }
                        catch (Exception ex)
                        {
                            progress?.Report((progressPercent, $"导出表 '{net.TableName}' 失败：{ex.Message}"));
                        }
                    }

                    // 保存文件
                    progress?.Report((95, "正在保存文件..."));
                    workbook.SaveAs(filePath);

                    // 完成
                    progress?.Report((100, $"导出完成！文件已保存至：\n{filePath}"));
                }
            }, cancellationToken);
        }
        
        
        
        /// <summary>
        /// 获取要导出的数据列
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetNetworkAddressHeader(ExportNetworkInfoClass expInfo)
        {
            var headers = new Dictionary<string, string>
           {
               {"Address","IP地址"},
               {"AddressStatus","地址状态"},
               {"Name","用户名"},
               {"Organization","组织"},
               {"Department","部门"},
               {"Group","群组"},
               {"Unit","单元"},
               {"Phone","电话"},
               {"HostName","主机名"},
               {"MacAddress","Mac地址"},
               {"TagA","TagA"},
               {"TagB","TagB"},
               {"TagC","TagC"},
               {"TagD","TagD"},
               {"TagE","TagE"},
               {"TagF","TagF"},
           };
            
            if (expInfo.WindowTags != null)
            {
                var tags = expInfo.WindowTags;

                headers["TagA"] = tags.TagA;
                headers["TagB"] = tags.TagB;
                headers["TagC"] = tags.TagC;
                headers["TagD"] = tags.TagD;
                headers["TagE"] = tags.TagE;
                headers["TagF"] = tags.TagF;
            }

            return headers;
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
                else
                {
                    return null;
                }


            }
            else //全局标签
            {
                var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

                if (tags != null)
                {
                    return JsonConvert.DeserializeObject(tags);
                }
                else
                {
                    return null;
                }


            }


           
        }







        /// <summary>
        /// 获取指定表名的网段地址信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private ObservableCollection<IpAddressInfoListExportViewModel> GetNetworkAddressInfo(ExportNetworkInfoClass expInfo)
        {
            var infos = new ObservableCollection<IpAddressInfoListExportViewModel>();

            var tableName = expInfo.TableName;

            string query =$"SELECT  {tableName}.*,  UserInfo.Name,  UserInfo.Organization,  UserInfo.Department,  UserInfo.UserGroup, UserInfo.UserUnit,  UserInfo.Phone, Asset.AssetTag,  Asset.AssetNumber FROM  {tableName}  LEFT JOIN  UserInfo  ON  {tableName}.User = UserInfo.UserId LEFT JOIN  Asset  ON  {tableName}.LinkDevice = Asset.AssetId  ORDER BY Address ASC;";



            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            //取出网段地址
            string prefix = GetNetworkNamePrefix(expInfo);

           
            
            int index2 = 0;

            foreach (var row in rows)
            {

                var info = new IpAddressInfoListExportViewModel();

                info.Index = index2;
                index2++;
                


                info.Address = $"{prefix}{row["Address"]}"; 

                int status = Convert.ToInt32(row["AddressStatus"].ToString());

                info.AddressStatus = status;



                if (status == 0 || status == 4)
                {
                    info.AddressType = false;
                }
                else
                {
                    info.AddressType = true;
                }


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


                infos.Add(info);

            }

            return infos;
        }


        /// <summary>
        /// 获取网段前缀
        /// </summary>
        /// <param name="expInfo"></param>
        /// <returns></returns>
        private string GetNetworkNamePrefix(ExportNetworkInfoClass expInfo)
        {
            var prefix = string.Empty;

            if (!string.IsNullOrWhiteSpace(expInfo.Range))
            {
                var ipRange = expInfo.Range;


                string[] parts = ipRange.Split('.');
                string result = string.Join(".", parts.Take(3));

                prefix = result + ".";

            }
            else
            {
                prefix = expInfo.Network.Substring(0, expInfo.Network.LastIndexOf('.') + 1);
            }


            return prefix;
        }
    }
}

