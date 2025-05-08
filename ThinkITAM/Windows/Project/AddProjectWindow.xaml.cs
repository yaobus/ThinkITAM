using Microsoft.Win32;
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

namespace ThinkITAM.Windows.Project
{
    /// <summary>
    /// AddProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddProjectWindow : Window
    {
        public AddProjectWindow()
        {
            InitializeComponent();
        }

        private void AddProjectWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DatabaseTypeGroups.SelectedIndex = 0;
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewToggleButton.IsChecked == false)
            {
                string title = (string)FindResource("ApwOpenFileTitle");
                string filter = (string)FindResource("ApwOpenFileFilter");

                // ToggleButton 未选中，选择 .db 文件
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = $"{filter}";
                openFileDialog.Title = $"{title}";

                if (openFileDialog.ShowDialog() == true)
                {
                    DbFilePath.Text = openFileDialog.FileName;
                }


            }
            else
            {
                // ToggleButton 选中，选择文件夹
                var dialog = new OpenFolderDialog
                {
                    Title = "选择文件夹"
                };

                if (dialog.ShowDialog() == true)
                {
                    DbFilePath.Text = dialog.FolderName;
                }

            }
        }

        private void NewToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            DbFilePath.Text = null;
        }

        /// <summary>
        /// 切换项目存储方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseTypeGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = DatabaseTypeGroups.SelectedIndex;
            if (index != -1)
            {
                if (index == 0)
                {
                    SqliteProjectPlane.Visibility= Visibility.Visible;
                    OtherDatabasePlane.Visibility = Visibility.Collapsed;
                    this.Height = 350;
                }
                else
                {
                    SqliteProjectPlane.Visibility = Visibility.Collapsed;
                    OtherDatabasePlane.Visibility = Visibility.Visible;
                    this.Height = 650;
                }
            }
        }
    }

}
