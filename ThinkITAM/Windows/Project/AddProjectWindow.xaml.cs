using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThinkITAM.Functions.Protector;
using ThinkITAM.ViewModels.DataBaseConfig;
using Path = System.IO.Path;
using MaterialDesignThemes.Wpf;
using ThinkITAM.UserControls.InformationDisplay;
using Microsoft.Data.Sqlite;

namespace ThinkITAM.Windows.Project
{
    /// <summary>
    /// AddProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddProjectWindow : Window
    {
        public AddProjectWindow(DataBaseConfigViewModel config = null)
        {
            InitializeComponent();
            inputConfig = config;
            if (config!=null)
            {
                DataContext = config;
               
            }

            
            LoadExistingConfigs();
        }

        private DataBaseConfigViewModel inputConfig;
        private void AddProjectWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (inputConfig == null)
            {
                DatabaseTypeGroups.SelectedIndex = 0;
            }


        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewToggleButton.IsChecked == false)
            {
                var title = (string)FindResource("ApwOpenFileTitle");
                var filter = (string)FindResource("ApwOpenFileFilter");

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

                var title = (string)FindResource("ApwSaveFileMessage");
                // ToggleButton 选中，选择文件夹
                var dialog = new OpenFolderDialog
                {
                    Title = $"{title}"
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
            var index = DatabaseTypeGroups.SelectedIndex;
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

        /// <summary>
        /// 保存项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {

            if (inputConfig != null)
            {
                configs.Remove(inputConfig);
            }


            var dataBaseConfig = new DataBaseConfigViewModel();

            var index = DatabaseTypeGroups.SelectedIndex;


            var type = "";

            switch (index)
            {
                case 0:
                    type = "Sqlite";
                    break;
                case 1:
                    type = "Mysql";
                    break;
                case 2:
                    type = "MariaDB";
                    break;
                case 3:
                    type = "SqlServer";
                    break;
            }

            dataBaseConfig.Type = type;

            if (index == 0)//sqlite数据库
            {

                if (!string.IsNullOrWhiteSpace(DbFilePath.Text) && !string.IsNullOrWhiteSpace(DbNickName.Text))
                {
                    dataBaseConfig.NickName = DbNickName.Text;
                    if (NewToggleButton.IsChecked == true)
                    {
                        
                        dataBaseConfig.Path =$"{DbFilePath.Text}\\{dataBaseConfig.NickName}.db" ;

                        

                        CreateEmptySqliteDatabase(dataBaseConfig.Path);
                    }
                    else
                    {
                        
                        dataBaseConfig.Path = DbFilePath.Text;
                    }



                    
                    configs.Add(dataBaseConfig);
                    SaveConfigsToFile();
                    DialogResult = true;

                }
                else//信息不完整
                {

                    var title = (string)FindResource("ApwMdTitle");
                    var prompt = (string)FindResource("ApwMdPrompt");
                    var confirm = (string)FindResource("CdConfirm");

                    var dialog = new ConfirmationDialog
                    {
                        Title = $"{title}",
                        Prompt = $"{prompt}",
                        ConfirmButtonText = $"{confirm}"

                    };

                    // 显示对话框
                    await DialogHost.Show(dialog, "AddProjectWindowMessageDialogHost");


                }

            }
            else//其他数据库
            {
                if (!string.IsNullOrWhiteSpace(DbHost.Text)&&!string.IsNullOrWhiteSpace(DbPort.Text)&&!string.IsNullOrWhiteSpace(DbUserName.Text)&&!string.IsNullOrWhiteSpace(DbPassword.Text)&&!string.IsNullOrWhiteSpace(DbDatabaseName.Text)&&!string.IsNullOrWhiteSpace(DbProjectName.Text))
                {

                    dataBaseConfig.NickName = DbProjectName.Text;
                    dataBaseConfig.Host = DbHost.Text;
                    dataBaseConfig.Port = int.Parse(DbPort.Text);
                    dataBaseConfig.UserName = DbUserName.Text;
                    dataBaseConfig.Password = DbPassword.Text;
                    dataBaseConfig.DatabaseName = DbDatabaseName.Text;

                    configs.Add(dataBaseConfig);
                    SaveConfigsToFile();
                    DialogResult = true;

                }
                else
                {
                    var title = (string)FindResource("ApwMdTitle");
                    var prompt = (string)FindResource("ApwMdPrompt");
                    var confirm = (string)FindResource("CdConfirm");

                    var dialog = new ConfirmationDialog
                    {
                        Title = $"{title}",
                        Prompt = $"{prompt}",
                        ConfirmButtonText = $"{confirm}"

                    };

                    // 显示对话框
                    await DialogHost.Show(dialog, "AddProjectWindowMessageDialogHost");

                }



            }




        }

        // 创建空 SQLite 数据库的方法
        private void CreateEmptySqliteDatabase(string filePath)
        {
            // 如果文件已存在，可以选择删除或提示用户
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var connection = new SqliteConnection($"Data Source={filePath}");
            connection.Open(); // 打开连接即会创建空数据库文件
        }


        private ObservableCollection<DataBaseConfigViewModel> configs = new ObservableCollection<DataBaseConfigViewModel>();
        private void LoadExistingConfigs()
        {
            configs.Clear();
            // 获取当前用户的文档目录
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string appDataPath = Path.Combine(documentsPath, "ThinkITAM");  // 自定义应用数据目录
            string dbConfigPath = Path.Combine(appDataPath, "DatabaseConfig");
            string configFilePath = Path.Combine(dbConfigPath, "DatabaseConfig.json");

            // 如果目录不存在，则创建
            if (!Directory.Exists(dbConfigPath))
            {
                Directory.CreateDirectory(dbConfigPath);
            }



            if (File.Exists(configFilePath))
            {
                var encryptJson = File.ReadAllText(configFilePath);

                var json = PasswordProtector.Decrypt(encryptJson);

                

                var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                var loaded = JsonSerializer.Deserialize<List<DataBaseConfigViewModel>>(json, options);

                if (loaded != null)
                {
                    foreach (var item in loaded)
                    {

                        configs.Add(item);
                    }
                }
            }
        }


        /// <summary>
        /// 保存配置文件到文件
        /// </summary>
        private void SaveConfigsToFile()
        {
            // 获取当前用户的文档目录
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string appDataPath = Path.Combine(documentsPath, "ThinkITAM");  // 自定义应用数据目录
            string dbConfigPath = Path.Combine(appDataPath, "DatabaseConfig");
            string configFilePath = Path.Combine(dbConfigPath, "DatabaseConfig.json");

            // 如果目录不存在，则创建
            if (!Directory.Exists(dbConfigPath))
            {
                Directory.CreateDirectory(dbConfigPath);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(configs.ToList(), options);
            var encryptJson = Functions.Protector.PasswordProtector.Encrypt(json);

            File.WriteAllText(configFilePath, encryptJson);
        }
    }

}
