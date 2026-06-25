using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ThinkITAM.Database;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.Protector;
using ThinkITAM.ViewModels.DataBaseConfig;
using ThinkITAM.Views;

namespace ThinkITAM;

/// <summary>
/// 主窗口 — 密码登录 + 项目选择
/// 移植自 WPF MainWindow.xaml.cs
/// </summary>
public partial class MainWindow : Window
{
    private ObservableCollection<DataBaseConfigViewModel> configs = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成
    /// </summary>
    private void MainWindow_OnOpened(object? sender, EventArgs e)
    {
        // 标题栏显示版本号
        VersionLabel.Text = "Ver " + DataBridge.DataBridge.Version;

        InitializationStatus();
        ShowWelcome();
        GetEncryptString();
        ProjectListView.ItemsSource = configs;
    }

    /// <summary>
    /// 初始化主题和语言
    /// </summary>
    private void InitializationStatus()
    {
        // 设置主题
        if (AppSettings.ThemeIndex == 0)
        {
            ThemeToggleButton.IsChecked = false;
            Services.ThemeService.SetTheme(Avalonia.Styling.ThemeVariant.Light);
        }
        else
        {
            ThemeToggleButton.IsChecked = true;
            Services.ThemeService.SetTheme(Avalonia.Styling.ThemeVariant.Dark);
        }

        // 设置语言
        LanguageComboBox.SelectedIndex = AppSettings.LanguageIndex;
    }

    /// <summary>
    /// 显示欢迎窗口
    /// </summary>
    private async void ShowWelcome()
    {
        if (AppSettings.Version != DataBridge.DataBridge.Version)
        {
            AppSettings.Version = DataBridge.DataBridge.Version;
            AppSettings.ShowWelcome = true;
            await ShowWelcomeDialog();
        }
        else if (AppSettings.ShowWelcome)
        {
            await ShowWelcomeDialog();
        }
    }

    private async Task ShowWelcomeDialog()
    {
        await Services.DialogService.ShowInfo(
            $"欢迎使用 ThinkITAM v{DataBridge.DataBridge.Version}！\n\n" +
            "这是一个集IP地址管理、设备端口管理、资产管理、链路可视化的综合网络管理系统。",
            "欢迎");
    }

    /// <summary>
    /// 判断是否存在加密字符串
    /// </summary>
    private void GetEncryptString()
    {
        LoadPassPortStr();

        if (string.IsNullOrWhiteSpace(AppSettings.EncryptString))
        {
            // 未设置密码 → 显示设置密码面板
            SetPasswordZone.IsVisible = true;
            LoginZone.IsVisible = false;
            ProjectZone.IsVisible = false;
        }
        else
        {
            // 已有密码 → 显示登录面板
            SetPasswordZone.IsVisible = false;
            LoginZone.IsVisible = true;
            ProjectZone.IsVisible = false;
        }
    }

    /// <summary>
    /// 从文件加载通行证
    /// </summary>
    private void LoadPassPortStr()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appDataPath = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
        var configFilePath = Path.Combine(appDataPath, "PassPort.e");

        try
        {
            if (File.Exists(configFilePath))
            {
                var str = File.ReadAllText(configFilePath);
                if (string.IsNullOrWhiteSpace(AppSettings.EncryptString))
                    AppSettings.EncryptString = str;
            }
        }
        catch
        {
            AppSettings.EncryptString = string.Empty;
        }
    }

    /// <summary>
    /// 保存密码到文件
    /// </summary>
    private void SavePasswordStrToFile(string passwordStr)
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appDataPath = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
        var configFilePath = Path.Combine(appDataPath, "PassPort.e");

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);

        File.WriteAllText(configFilePath, passwordStr);
    }

    /// <summary>
    /// 保存密码按钮
    /// </summary>
    private async void SavePasswordButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var password = PasswordBox.Text ?? "";
        var passwordAgain = PasswordBoxAgain.Text ?? "";

        if (string.IsNullOrWhiteSpace(password))
        {
            await Services.DialogService.ShowConfirm(
                "密码不能为空，是否继续使用空密码？",
                "注意");
            return;
        }

        if (passwordAgain != password)
        {
            await Services.DialogService.ShowWarning(
                "两次输入的密码不一致，请重新输入。",
                "密码不一致");
            return;
        }

        // 使用硬件ID加密密码
        var passwordString = PasswordProtector.Encrypt(password);
        AppSettings.EncryptString = passwordString;

        // 保存密码文件
        SavePasswordStrToFile(passwordString);

        // 切换到登录界面
        SetPasswordZone.IsVisible = false;
        LoginZone.IsVisible = true;
    }

    /// <summary>
    /// 登录按钮
    /// </summary>
    private async void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var inputPassword = InputPasswordBox.Text ?? "";
        var passwordString = PasswordProtector.Encrypt(inputPassword);

        Console.WriteLine("PASS:"+passwordString);
        
        if (passwordString != AppSettings.EncryptString)
        {
            await Services.DialogService.ShowWarning(
                "密码验证失败，请重新输入。",
                "验证失败");
        }
        else
        {
            // 验证成功 → 显示项目列表
            ProjectZone.IsVisible = true;
            LoginZone.IsVisible = false;
            LoadDatabaseConfig();
        }
    }

    /// <summary>
    /// 忘记密码按钮
    /// </summary>
    private async void ForgetPasswordButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var result = await Services.DialogService.ShowConfirm(
            "忘记密码将清除所有数据库配置文件。\n是否继续？",
            "忘记密码");

        if (result)
        {
            AppSettings.EncryptString = null;

            // 删除配置文件
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var appDataPath = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
            var configFilePath = Path.Combine(appDataPath, "DatabaseConfig.json");
            var passPortPath = Path.Combine(appDataPath, "PassPort.e");

            try
            {
                if (File.Exists(configFilePath)) File.Delete(configFilePath);
                if (File.Exists(passPortPath)) File.Delete(passPortPath);
            }
            catch { }

            GetEncryptString();
        }
    }

    /// <summary>
    /// 密码框回车键
    /// </summary>
    private void InputPasswordBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton_OnClick(null, e);
        }
    }

    /// <summary>
    /// 加载数据库配置文件（解密 JSON）
    /// </summary>
    private void LoadDatabaseConfig()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appDataPath = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
        var configFilePath = Path.Combine(appDataPath, "DatabaseConfig.json");

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);

        if (!File.Exists(configFilePath))
        {
            // 配置文件不存在 → 显示项目面板（空）
            ProjectZone.IsVisible = true;
            return;
        }

        configs.Clear();

        try
        {
            var encryptJson = File.ReadAllText(configFilePath);
            var json = PasswordProtector.Decrypt(encryptJson);

            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var loaded = JsonSerializer.Deserialize<List<DataBaseConfigViewModel>>(json, options);

            if (loaded != null)
            {
                foreach (var item in loaded)
                    configs.Add(item);
            }
        }
        catch (Exception)
        {
            try { File.Delete(configFilePath); } catch { }
            Services.DialogService.ShowWarning("配置信息解密失败，请重新添加项目", "错误");
        }
    }

    /// <summary>
    /// 保存数据库配置到加密文件
    /// </summary>
    private void SaveConfigsToFile()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appDataPath = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
        var configFilePath = Path.Combine(appDataPath, "DatabaseConfig.json");

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(configs.ToList(), options);
        var encryptJson = PasswordProtector.Encrypt(json);

        File.WriteAllText(configFilePath, encryptJson);
    }

    /// <summary>
    /// 添加项目按钮
    /// </summary>
    private async void AddProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: Phase 2 — 打开 AddProjectWindow
        await Services.DialogService.ShowInfo("添加项目功能将在后续阶段实现", "提示");
    }

    /// <summary>
    /// 删除项目按钮
    /// </summary>
    private void DelProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ProjectListView.SelectedIndex >= 0)
        {
            configs.RemoveAt(ProjectListView.SelectedIndex);
            SaveConfigsToFile();
        }
    }

    /// <summary>
    /// 项目列表选择变更
    /// </summary>
    private void ProjectListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ProjectListView.SelectedIndex >= 0)
        {
            var dbConfig = configs[ProjectListView.SelectedIndex];

            GlobalVariables.dbConfig = dbConfig;

            // ViewModel → DatabaseConfig
            var databaseConfig = new DatabaseConfig
            {
                Type = dbConfig.Type,
                NickName = dbConfig.NickName,
                Path = dbConfig.Path,
                Host = dbConfig.Host,
                Port = dbConfig.Port,
                UserName = dbConfig.UserName,
                Password = dbConfig.Password,
                DatabaseName = dbConfig.DatabaseName
            };

            // 创建数据库服务
            GlobalVariables.DbService = DatabaseServiceFactory.CreateService(databaseConfig);

            DataBridge.DataBridge.NowOpenedDataBaseType = dbConfig.Type?.ToLower() switch
            {
                "sqlite" => "sqlite",
                "mysql" or "mariadb" => "mysql",
                "sqlserver" => "sqlserver",
                _ => "sqlite"
            };
        }
    }

    /// <summary>
    /// 项目双击 → 打开主工作区
    /// </summary>
    private async void ProjectListView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (GlobalVariables.DbService == null) return;

        if (await GlobalVariables.DbService.TestConnectionAsync())
        {
            var selectionWindow = new SelectionWindow();
            selectionWindow.Closed += (s, args) => this.Show();
            this.Hide();
            selectionWindow.Show();
        }
        else
        {
            await Services.DialogService.ShowWarning(
                "数据库连接失败，请检查配置。",
                "连接失败");
        }
    }

    /// <summary>
    /// 主题切换按钮
    /// </summary>
    private void ThemeToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Services.ThemeService.ToggleTheme();
        AppSettings.ThemeIndex = Services.ThemeService.IsDarkTheme ? 1 : 0;

        if (ThemeIcon != null)
            ThemeIcon.Kind = Services.ThemeService.IsDarkTheme
                ? Material.Icons.MaterialIconKind.WeatherNight
                : Material.Icons.MaterialIconKind.WeatherSunny;
    }

    /// <summary>
    /// 语言切换
    /// </summary>
    private void LanguageComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        AppSettings.LanguageIndex = LanguageComboBox.SelectedIndex;
        // 实际语言切换逻辑待 Phase 2 后续实现
    }

    /// <summary>
    /// 帮助按钮
    /// </summary>
    private void HelpButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Document", "ThinkITAM使用手册.pdf");

        try
        {
            if (File.Exists(sourceFilePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = sourceFilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                // 打开在线帮助
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://thinkitam.goeasy.work/",
                    UseShellExecute = true
                });
            }
        }
        catch { }
    }

    /// <summary>
    /// 版本标签点击
    /// </summary>
    private void VersionLabel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // TODO: 显示更新日志
    }
}
