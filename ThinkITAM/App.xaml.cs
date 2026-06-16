using System.Windows;
using System.Windows.Threading;
using Dapper;
using ThinkITAM.Properties;

namespace ThinkITAM;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {


        base.OnStartup(e);

        Settings.Default.Upgrade();


        //订阅全局异常信息
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;

    }

    /// <summary>
    /// 全局异常处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {

        var note = string.Empty;
        
        // 如果错误信息包含字符串“no such table”则提示信息“检测到数据库中缺失被引用的表单，可能是非程序内删除了指定表单导致”
        if (e.Exception != null && e.Exception.Message != null && e.Exception.Message.Contains("no such table"))
        {

            note = "检测到数据库中缺失被引用的表单，可能是非程序内删除了指定表单导致！";

        }
        
        // 处理异常
        // 记录异常信息、显示友好的错误提示框等
        var message = $"程序版本号：{DataBridge.DataBridge.Version}\r{note}\r\r{e.Exception}";

        
        
        
        //将错误信息写到桌面的ThinkITAM_Log文件夹
        
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var logFolderPath = System.IO.Path.Combine(desktopPath, "ThinkITAM_Log");
        
        if (!System.IO.Directory.Exists(logFolderPath))
        {
            System.IO.Directory.CreateDirectory(logFolderPath);
        }
        
        var date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filePath = System.IO.Path.Combine(logFolderPath, $"ThinkITAM-LOG-{date}.txt");
        System.IO.File.WriteAllText(filePath, message);

        var fileName = System.IO.Path.GetFileName(filePath);

        MessageBox.Show($"可恶！程序又出错啦!\r错误日志已保存到桌面ThinkITAM_Log文件夹,请向开发者反馈！\r错误日志文件名:{fileName}", "出错啦！", MessageBoxButton.OK, MessageBoxImage.Error);

        e.Handled = true; // 标记为已处理，防止应用程序终止
    }
}
