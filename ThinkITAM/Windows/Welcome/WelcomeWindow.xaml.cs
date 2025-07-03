using System.IO;
using System.Text;
using System.Windows;


namespace ThinkITAM.Windows.Welcome;
/// <summary>
/// WelcomeWindow.xaml 的交互逻辑
/// </summary>
public partial class WelcomeWindow : Window
{
    /// <summary>
    /// 0为正常加载，1为手动加载
    /// </summary>
    /// <param name="mode"></param>
    public WelcomeWindow(int mode = 0)
    {
        InitializeComponent();

        if (mode == 1)
        {
            ShowWelcomeBox.Visibility = Visibility.Collapsed;
            CloseButton.Content = "知道了，退下吧";
            QrGrid.Visibility = Visibility.Collapsed;
        }

    }

    private void WelcomeWindow_OnLoaded(object sender, RoutedEventArgs e)
    {



        //从软件所在目录读取UpdateInfo.txt文件
        string updateInfo = ReadUpdateInfo();

        if (!string.IsNullOrEmpty(updateInfo))
        {


            UpdateInfo.Markdown = updateInfo;


        }

    }

    private string ReadUpdateInfo()
    {
        try
        {
            // 获取当前应用程序所在目录
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            // 构建 UpdateInfo.txt 的完整路径
            string filePath = Path.Combine(appDir, "Resources\\UpdateInfo\\UpdateInfo.txt");

            // 判断文件是否存在
            if (File.Exists(filePath))
            {
                // 读取文件内容到字符串
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            else
            {
                return null; // 文件不存在
            }
        }
        catch (Exception ex)
        {
            // 出错处理
            MessageBox.Show($"读取文件时发生错误：{ex.Message}");
            return null;
        }
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ShowWelcomeBox.IsChecked == true)
        {
            Properties.Settings.Default.ShowWellcome = false;
            Properties.Settings.Default.Save();
        }



        this.Close();
    }


}



