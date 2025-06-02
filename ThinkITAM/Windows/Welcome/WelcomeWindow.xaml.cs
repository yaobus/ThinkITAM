using System;
using System.IO;
using System.Windows;

namespace ThinkITAM.Windows.Welcome;
/// <summary>
/// WelcomeWindow.xaml 的交互逻辑
/// </summary>
public partial class WelcomeWindow : Window
{
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void WelcomeWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        //从软件所在目录读取UpdateInfo.txt文件
        string updateInfo = ReadUpdateInfo();

        if (!string.IsNullOrEmpty(updateInfo))
        {
            UpdateInfo.Text= updateInfo;
           
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
                return File.ReadAllText(filePath);
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
}



