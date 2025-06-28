using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Functions.Import;
using ThinkITAM.ViewModels.AssetManage;
using Path = System.IO.Path;

namespace ThinkITAM.Windows.AssetManage;

/// <summary>
/// AssetDataImportWindow.xaml 的交互逻辑
/// </summary>
public partial class AssetDataImportWindow : Window
{
    public AssetDataImportWindow()
    {
        InitializeComponent();
        AssetType.ItemsSource = assetTypeInfos;
        DeviceType.ItemsSource = deviceTypeInfos;
    }

    private void AssetDataImportWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAssetType();
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







    }



    private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
    {

        var title = (string)FindResource("AawOpenFileTitle");
        var filter = (string)FindResource("AawOpenFileFilter");

        // ToggleButton 未选中，选择 .db 文件
        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Filter = $"{filter}";
        openFileDialog.Title = $"{title}";

        if (openFileDialog.ShowDialog() == true)
        {
            ExcelFilePath.Text = openFileDialog.FileName;
        }




    }

    private void TemplateGetButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 1. 弹出保存对话框让用户选择路径
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
        saveFileDialog.FileName = "AssetTemplate.xlsx";

        if (saveFileDialog.ShowDialog() == true)
        {
            string destinationPath = saveFileDialog.FileName;

            // 2. 获取嵌入资源或者本地文件内容
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\Template\\AssetTemplate.xlsx");

            try
            {
                // 3. 复制文件到目标位置
                File.Copy(sourceFilePath, destinationPath, overwrite: true);

                // 4. 打开资源管理器并定位到该文件夹
                Process.Start("explorer.exe", $"/select,\"{destinationPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法导出数据模板：{ex.Message}");
            }
        }
    }


    /// <summary>
    /// 资产类型列表
    /// </summary>
    private ObservableCollection<string> deviceTypeInfos = new ObservableCollection<string>();


    private void AssetType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AssetType.SelectedIndex != -1)
        {
            deviceTypeInfos.Clear();


            string query =
                $"SELECT  DeviceType FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}';";

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

    /// <summary>
    /// 点击导入按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (AssetType.SelectedIndex != -1 && DeviceType.SelectedIndex != -1)
        {
            if (!string.IsNullOrWhiteSpace(ExcelFilePath.Text))
            {
                var assetType = assetTypeInfos[AssetType.SelectedIndex].ToString();
                var deviceType = deviceTypeInfos[DeviceType.SelectedIndex].ToString();

                //获取资产编号前缀
                string assetTag = GetAssetTag();


                if (ExcelImporter.IsFileLocked(ExcelFilePath.Text))
                {
                    MessageBox.Show("该文件正被其他程序使用，请关闭后再尝试导入。", "文件被占用", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var importDatas = ExcelImporter.ImportFromExcel<ImportAssetDataViewModel>(ExcelFilePath.Text);

                //设置进度条最大值
                Dispatcher.Invoke(() =>
                {
                    ImportProgressBar.Maximum = importDatas.Count;
                    ImportProgressBar.Value = 0;
                });

                var filter = $" WHERE AssetType='{assetType}' AND DeviceType='{deviceType}'";
                int index = 0;

                foreach (var data in importDatas)
                {
                    index++;

                    var assetNumber = DbClass.GetNextAvailableNumber("Asset", "AssetNumber", filter);

                    //创建资产ID字符串，0为机房，1为机柜，2为设备,3为机架，4为通用终端（计算机、IP电话）
                    string assetId = $"2{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(Guid.NewGuid().ToString())).ToUpper()}";

                    //创建资产二维码,0为机房，1为机柜，2为设备
                    string qrCode = "ITAM:" + AssetCodeClass.GenerateChecksum(assetId).ToUpper();



                    data.AssetId = assetId;
                    data.AssetQrCode = qrCode;
                    data.AssetType = assetType;
                    data.DeviceType = deviceType;
                    data.AssetTag = assetTag;
                    data.AssetNumber = assetNumber.ToString();

                    string date;
                    try
                    {
                        date = DateConverClass.ConvertExcelDateToDateTime(Convert.ToDouble(data.PurchaseDate)).ToString();
                    }
                    catch (Exception exception)
                    {
                        date = string.Empty;
                    }

                    data.PurchaseDate = date;

                    GlobalVariables.DbService.InsertEntity("Asset", data);

                    await UpdateProgressBarAsync(index);


                }


                DialogResult = true;

            }
            else
            {
                MessageBox.Show("请选择需要导入的数据", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        else
        {
            MessageBox.Show("请选择资产类型和设备类型", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// 更新极度条
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private async Task UpdateProgressBarAsync(int value)
    {

        // 同样使用Dispatcher更新进度值
        Dispatcher.Invoke(() =>
        {
            ImportProgressBar.Value = value;
        });

    }




    /// <summary>
    /// 获取资产编号前缀
    /// </summary>
    /// <returns></returns>
    private string GetAssetTag()
    {
        string query =
            $"SELECT  AssetTag FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}' AND DeviceType='{deviceTypeInfos[DeviceType.SelectedIndex].ToString()}';";


        var tag = GlobalVariables.DbService.ExecuteScalar(query).ToString();

        if (tag != null)
        {
            return tag;
        }
        else
        {
            return string.Empty;
        }


    }
}
