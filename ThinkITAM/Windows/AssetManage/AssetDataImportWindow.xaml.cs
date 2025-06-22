using System;
using System.Collections.Generic;
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

    private void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {
       
    }
}
