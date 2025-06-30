using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.InformationDisplay;
using ThinkITAM.ViewModels.DataBaseConfig;
using ThinkITAM.Windows.Project;
using ThinkITAM.Windows.Selection;
using ThinkITAM.Windows.Welcome;
using Path = System.IO.Path;


namespace ThinkITAM;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private ObservableCollection<DataBaseConfigViewModel> configs = new ObservableCollection<DataBaseConfigViewModel>();

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        VersionLabel.Content="Ver "+DataBridge.DataBridge.Version;

        InitializationStatus();
        ShowWelcome();

        GetEncryptString();
        ProjectListView.ItemsSource = configs;

    }

    /// <summary>
    /// 显示 欢迎信息
    /// </summary>
    private void ShowWelcome()
    {
        if (Properties.Settings.Default.Version != DataBridge.DataBridge.Version)
        {
            Properties.Settings.Default.Version = DataBridge.DataBridge.Version;

            Properties.Settings.Default.ShowWellcome = true;
            Properties.Settings.Default.Save();

            var newWindow = new WelcomeWindow();
            newWindow.Owner = this;
            newWindow.ShowDialog();

        }
        else
        {

            if (Properties.Settings.Default.ShowWellcome == true)
            {
                var newWindow = new WelcomeWindow();
                newWindow.Owner = this;
                newWindow.ShowDialog();
            }


        }



    }


    /// <summary>
    /// 初始化程序语言和主题
    /// </summary>
    private void InitializationStatus()
    {


        //设置主题
        if (Properties.Settings.Default.ThemeIndex == 0)
        {
            SetThemeLight();

            ThemeToggleButton.IsChecked = false;
        }
        else
        {
            SetThemeDark();
            ThemeToggleButton.IsChecked = true;
        }



        //设置语言
        Functions.Language.LanguageSet.LanguageSelect(Properties.Settings.Default.LanguageIndex);

        ApplyTheme();

    }


    /// <summary>
    /// 设置初始主题
    /// </summary>
    private void SetThemeLight()
    {
        var paletteHelper = new PaletteHelper();
        var theme = Theme.Create(BaseTheme.Light, Colors.DarkCyan, Colors.YellowGreen); // 使用默认颜色
        paletteHelper.SetTheme(theme);
        Properties.Settings.Default.ThemeIndex = 0;
        Properties.Settings.Default.Save();



    }

    /// <summary>
    /// 设置初始主题
    /// </summary>
    private void SetThemeDark()
    {
        var paletteHelper = new PaletteHelper();
        var theme = Theme.Create(BaseTheme.Dark, Colors.DarkCyan, Colors.YellowGreen); // 使用默认颜色
        paletteHelper.SetTheme(theme);
        Properties.Settings.Default.ThemeIndex = 1;
        Properties.Settings.Default.Save();





    }

    private void ThemeToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ThemeToggleButton.IsChecked == true)
        {
            SetThemeDark();

        }
        else
        {
            SetThemeLight();

        }

        //Nodify应用面板主题
        ApplyTheme();

    }

    /// <summary>
    /// 切换画板主题
    /// </summary>
    /// <param name="themeName"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ApplyTheme()
    {

        // 加载主题资源字典
        var _darkTheme = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Nodify;component/Themes/Dark.xaml")
        };

        var _lightTheme = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Nodify;component/Themes/Light.xaml")
        };

        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;




        int index = Properties.Settings.Default.ThemeIndex;

        switch (index)
        {
            case 1:
                mergedDictionaries.Remove(_lightTheme);
                mergedDictionaries.Add(_darkTheme);

                break;
            default:
                mergedDictionaries.Remove(_darkTheme);
                mergedDictionaries.Add(_lightTheme);

                break;
        }


    }



    /// <summary>
    /// 判断是否存在加密字符串，不存在则显示密码输入框
    /// </summary>
    private void GetEncryptString()
    {


        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.EncryptString))
        {


            SetPasswordZone.Visibility = Visibility.Visible;
            LoginZone.Visibility = Visibility.Collapsed;
            ProjectZone.Visibility = Visibility.Collapsed;
        }
        else
        {
            SetPasswordZone.Visibility = Visibility.Collapsed;
            LoginZone.Visibility = Visibility.Visible;
        }


    }


    private void LanguageComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        SaveSetting();
        Functions.Language.LanguageSet.LanguageSelect(LanguageComboBox.SelectedIndex);
    }


    /// <summary>
    /// 保存设置
    /// </summary>
    private void SaveSetting()
    {


        Properties.Settings.Default.LanguageIndex = LanguageComboBox.SelectedIndex;


        Properties.Settings.Default.Save();

    }

    /// <summary>
    /// 保存密码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SavePasswordButton_OnClick(object sender, RoutedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(PasswordBox.Password)) //密码为空
        {

            string title = (string)FindResource("CdTitle");
            string prompt = (string)FindResource("CdPrompt");
            string confirm = (string)FindResource("CdConfirm");

            var dialog = new ConfirmationDialog
            {
                Title = $"{title}",
                Prompt = $"{prompt}",
                ConfirmButtonText = $"{confirm}"

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MainWindowMessageDialogHost");
        }
        else
        {
            if (PasswordBoxAgain.Password != PasswordBox.Password) //两次密码不一致
            {
                string title = (string)FindResource("CdTitle");
                string prompt = (string)FindResource("CdPrompt2");
                string confirm = (string)FindResource("CdConfirm");

                var dialog = new ConfirmationDialog
                {
                    Title = $"{title}",
                    Prompt = $"{prompt}",
                    ConfirmButtonText = $"{confirm}"

                };

                // 显示对话框
                await DialogHost.Show(dialog, "MainWindowMessageDialogHost");
            }
            else //保存密码
            {

                string passwordString = Functions.Protector.PasswordProtector.Encrypt2(PasswordBoxAgain.Password);

                Properties.Settings.Default.EncryptString = passwordString;

                Properties.Settings.Default.Save();

                SetPasswordZone.Visibility = Visibility.Collapsed;
                LoginZone.Visibility = Visibility.Visible;
            }


        }

    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        string passwordString = Functions.Protector.PasswordProtector.Encrypt2(InputPasswordBox.Password);



        if (passwordString != Properties.Settings.Default.EncryptString)
        {
            string title = (string)FindResource("CdFailedVerificationTitle");
            string prompt = (string)FindResource("CdFailedVerificationPrompt");
            string confirm = (string)FindResource("CdConfirm");

            var dialog = new ConfirmationDialog
            {
                Title = $"{title}",
                Prompt = $"{prompt}",
                ConfirmButtonText = $"{confirm}"

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MainWindowMessageDialogHost");
        }
        else //验证成功,解密加载数据库配置文件
        {


            ProjectZone.Visibility = Visibility.Visible;
            LoginZone.Visibility = Visibility.Collapsed;
            //加载数据库配置文件
            LoadDatabaseConfig();

        }


    }


    /// <summary>
    /// 加载数据库配置
    /// </summary>
    private void LoadDatabaseConfig()
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

        if (!File.Exists(configFilePath))
        {
            // 如果数据库配置文件不存在，则显示项目创建面板
            SetPasswordZone.Visibility = Visibility.Collapsed;
            LoginZone.Visibility = Visibility.Collapsed;
            ProjectZone.Visibility = Visibility.Visible;
        }
        else
        {
            configs.Clear();

            var encryptJson = File.ReadAllText(configFilePath);

            try
            {
                var json = Functions.Protector.PasswordProtector.Decrypt(encryptJson);

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
            catch (Exception e)
            {

                File.Delete(configFilePath);
                MessageBox.Show("配置信息解密失败，请重新添加项目", "错误", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private void AddProjectButton_OnClick(object sender, RoutedEventArgs e)
    {
        var newWindow = new AddProjectWindow();
        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            newWindow.Owner = window;
        }

        if (newWindow.ShowDialog() == true)
        {
            LoadDatabaseConfig();

        }
    }
    /// <summary>
    /// 项目列表选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void ProjectListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProjectListView.SelectedIndex != -1)
        {
            var dbConfig = configs[ProjectListView.SelectedIndex];

            GlobalVariables.dbConfig = dbConfig;

            // 创建服务实例
            GlobalVariables.DbService = DatabaseServiceFactory.CreateService(dbConfig);
        }







    }

    private async void ProjectListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (GlobalVariables.DbService.TestConnection() == true)//连接成功
        {
            SelectionWindow newWindow = new SelectionWindow();

            var window = Window.GetWindow(this);


            if (window != null)
            {
                newWindow.Owner = window;
            }
            newWindow.Closed += (s, args) => this.Show();

            window.Hide();


            if (newWindow.ShowDialog() == true)
            {


            }

        }
        else
        {
            string title = (string)FindResource("MainOpenFailedTitle");
            string prompt = (string)FindResource("MainOpenFailedPrompt");
            string confirm = (string)FindResource("CdConfirm");

            var dialog = new ConfirmationDialog
            {
                Title = $"{title}",
                Prompt = $"{prompt}",
                ConfirmButtonText = $"{confirm}"

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MainWindowMessageDialogHost");
        }
    }

    private void InputPasswordBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton_OnClick(null, null);
        }
    }






    private void DelProjectButton_OnClick(object sender, RoutedEventArgs e)
    {
        configs.RemoveAt(ProjectListView.SelectedIndex);

        SaveConfigsToFile();

    }

    /// <summary>
    /// 忘记密码,删除配置文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ForgetPasswordButton_OnClick(object sender, RoutedEventArgs e)
    {

        string title = (string)FindResource("CdForgetPasswordTitle");
        string prompt = (string)FindResource("CdForgetPasswordPrompt");
        string confirm = (string)FindResource("CdConfirm");

        var dialog = new ConfirmationDialog
        {
            Title = $"{title}",
            Prompt = $"{prompt}",
            ConfirmButtonText = $"{confirm}"

        };

        // 显示对话框


        bool? result = await DialogHost.Show(dialog, "MainWindowMessageDialogHost") as bool?;





        if (result == true)
        {
            Properties.Settings.Default.EncryptString = null;

            Properties.Settings.Default.Save();
            //删除配置文件

            // 获取当前用户的文档目录
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string appDataPath = Path.Combine(documentsPath, "ThinkITAM");  // 自定义应用数据目录
            string dbConfigPath = Path.Combine(appDataPath, "DatabaseConfig");
            string configFilePath = Path.Combine(dbConfigPath, "DatabaseConfig.json");


            File.Delete(configFilePath);

            GetEncryptString();
        }
    }

    private void HelpButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 1. 弹出保存对话框让用户选择路径
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "PDF 文件 (*.pdf)|*.pdf";
        saveFileDialog.FileName = "ThinkITAM使用手册.pdf";

        if (saveFileDialog.ShowDialog() == true)
        {
            string destinationPath = saveFileDialog.FileName;

            // 2. 获取嵌入资源或者本地文件内容
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Document\\ThinkITAM使用手册.pdf");

            try
            {
                // 3. 复制文件到目标位置
                File.Copy(sourceFilePath, destinationPath, overwrite: true);

                // 4. 打开资源管理器并定位到该文件夹
                Process.Start("explorer.exe", $"/select,\"{destinationPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法导出帮助文档：{ex.Message}");
            }
        }

    }
}