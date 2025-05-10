using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Resources;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.UserControls.InformationDisplay;
using ThinkITAM.ViewModels.DataBaseConfig;
using ThinkITAM.Windows.Project;
using ThinkITAM.DataBridge;
using ThinkITAM.Windows.Selection;
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
        
       
        InitializationStatus();
        GetEncryptString();
        ProjectListView.ItemsSource = configs;

        //Properties.Settings.Default.EncryptString = "";
        //Properties.Settings.Default.Save();


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
    }


    /// <summary>
    /// 设置初始主题
    /// </summary>
    private void SetThemeLight()
    {
        var paletteHelper = new PaletteHelper();
        var theme = Theme.Create(BaseTheme.Light, Colors.Tomato, Colors.Lime); // 使用默认颜色
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
        var theme = Theme.Create(BaseTheme.Dark, Colors.DarkCyan, Colors.Olive); // 使用默认颜色
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
            await DialogHost.Show(dialog, "MessageDialogHost");
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
                await DialogHost.Show(dialog, "MessageDialogHost");
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

        Console.WriteLine(passwordString);

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
            await DialogHost.Show(dialog, "MessageDialogHost");
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
        // 检查 数据库配置文件是否存在
        string dbConfigPath = AppDomain.CurrentDomain.BaseDirectory + @"DatabaseConfig\";
        string name = "DatabaseConfig.json";

        if (!Directory.Exists(dbConfigPath))
        {
            Directory.CreateDirectory(dbConfigPath);
        }

        dbConfigPath = Path.Combine(dbConfigPath, name);


        if (!File.Exists(dbConfigPath))
        {
            // 如果数据库文件不存在，则显示项目创建面板
            SetPasswordZone.Visibility = Visibility.Collapsed;
            LoginZone.Visibility = Visibility.Collapsed;
            ProjectZone.Visibility = Visibility.Visible;
        }
        else
        {
            configs.Clear();


            if (File.Exists(dbConfigPath))
            {
                var encryptJson = File.ReadAllText(dbConfigPath);

                Console.WriteLine(encryptJson);

                var json = Functions.Protector.PasswordProtector.Decrypt(encryptJson);

                Console.WriteLine(json);

                var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                var loaded = JsonSerializer.Deserialize<List<DataBaseConfigViewModel>>(json, options);

                if (loaded != null)
                {
                    foreach (var item in loaded)
                    {
                        //Console.WriteLine(PasswordProtector.Decrypt(item.Password));

                        configs.Add(item);
                    }
                }
            }

            //解密并加载配置文件

        }

    }



    private void AddProjectButton_OnClick(object sender, RoutedEventArgs e)
    {
        AddProjectWindow newWindow = new AddProjectWindow();
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
        DataBaseConfigViewModel dbConfig = configs[ProjectListView.SelectedIndex];

       // 创建服务实例
       GlobalVariables.DbService = DatabaseServiceFactory.CreateService(dbConfig);




       //string sql = $"Select * FROM Network";
       //var rows = GlobalVariables.DbService.ExecuteQuery(sql);

       //foreach (var row in rows)
       //{
       //    Console.WriteLine(row["Name"]);
       //}

    }

    private async void ProjectListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (GlobalVariables.DbService.TestConnection() == true)//连接成功
        {
            SelectionWindow newWindow=new SelectionWindow();

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
            await DialogHost.Show(dialog, "MessageDialogHost");
        }
    }
}