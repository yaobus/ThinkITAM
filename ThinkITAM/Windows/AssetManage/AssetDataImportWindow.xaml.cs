using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;
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
            DbFilePath.Text = openFileDialog.FileName;
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
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Template\\AssetTemplate.xlsx");

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


    private void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {
       
    }


}
