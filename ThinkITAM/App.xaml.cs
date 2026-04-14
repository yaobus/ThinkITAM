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

        var message = $"程序出错啦！向开发者反馈一下吧！程序版本号：{DataBridge.DataBridge.Version}\r\r{e.Exception}";

        MessageBox.Show(message, "别紧张！发生了点意外", MessageBoxButton.OK, MessageBoxImage.Information);

        e.Handled = true; // 标记为已处理，防止应用程序终止
    }
}
