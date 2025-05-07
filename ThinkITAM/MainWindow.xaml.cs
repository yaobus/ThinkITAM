using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
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

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        SetTheme();
        LoadDatabaseConfig();
        Functions.Language.LanguageSet.LanguageSelect(Properties.Settings.Default.LanguageIndex);
    }

    /// <summary>
    /// 设置初始主题
    /// </summary>
    private void SetTheme()
    {
        var paletteHelper = new PaletteHelper();
        var theme = Theme.Create(BaseTheme.Light, Colors.DarkOrange, Colors.Lime); // 使用默认颜色
        paletteHelper.SetTheme(theme);
    }

    /// <summary>
    /// 加载数据库配置
    /// </summary>
    private void LoadDatabaseConfig()
    {
        // 检查 数据库配置文件是否存在
        string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"DatabaseConfig\";
        string dbName = "DatabaseConfig.json";

        if (!Directory.Exists(dbFilePath))
        {
            Directory.CreateDirectory(dbFilePath);
        }

        dbFilePath = Path.Combine(dbFilePath, dbName);


        if (!File.Exists(dbFilePath))
        {
            // 如果数据库文件不存在，则创建一个新的数据库文件
            
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
}