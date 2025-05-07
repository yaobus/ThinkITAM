using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

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
    }

    /// <summary>
    /// 设置初始主题
    /// </summary>
    private void SetTheme()
    {
        var paletteHelper = new PaletteHelper();
        var theme = Theme.Create(BaseTheme.Dark, Colors.DarkOrange, Colors.Lime); // 使用默认颜色
        paletteHelper.SetTheme(theme);
    }
}