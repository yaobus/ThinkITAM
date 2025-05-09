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
using ThinkITAM.DatabaseOperation;
using Microsoft.Win32;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddBrowserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddBrowserWindow : Window
    {
        public AddBrowserWindow()
        {
            InitializeComponent();
        }



        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string name = BrowserName.Text;
            string path = BrowserPath.Text;

            if (name.Replace(" ", "").Length >= 2 && path.Replace(" ", "").Length >= 2)
            {
                string sqlTemp = $"SELECT COUNT(*) FROM Browser WHERE Path ='{path}'";
                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num <= 0)
                {
                    sqlTemp = $"INSERT INTO \"Browser\" (\"Browser\", \"Path\", \"Note\") VALUES ('{name}', '{path}', '{Note.Text}')";

                   
                    GlobalVariables.DbService.ExecuteNonQuery(sqlTemp);

                    this.DialogResult = true;

                    this.Close();


                }
                else
                {
                    MessageBox.Show("该浏览器已添加", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AddBrowserWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void FindBrowserPathButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 创建 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件类型筛选，仅允许选择 CSV 文件
            openFileDialog.Filter = "Browser (*.exe)|*.exe";

            // 显示对话框并获取用户选择的结果
            bool? result = openFileDialog.ShowDialog();

            // 如果用户选择了文件，则将文件路径加载到 TextBox 中
            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                BrowserPath.Text = selectedFilePath;
            }
        }
    }
}
