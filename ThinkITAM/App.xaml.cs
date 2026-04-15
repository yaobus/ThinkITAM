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
        // 处理异常
        // 记录异常信息、显示友好的错误提示框等
        var message = $"程序版本号：{DataBridge.DataBridge.Version}\r\r{e.Exception}";

        //将错误信息写到桌面
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filePath = System.IO.Path.Combine(desktopPath, $"ThinkITAM-LOG-{date}.txt");
        System.IO.File.WriteAllText(filePath, message);

        var fileName = System.IO.Path.GetFileName(filePath);

        MessageBox.Show($"可恶！程序又出错啦!\r错误日志已保存到桌面,请向开发者反馈！\r错误日志文件名:{fileName}", "出错啦！", MessageBoxButton.OK, MessageBoxImage.Error);

        e.Handled = true; // 标记为已处理，防止应用程序终止
    }
}
