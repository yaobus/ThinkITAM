using System.Windows;
using System.Windows.Threading;

namespace ThinkITAM;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {


        base.OnStartup(e);

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

        var message = $"完了完了完了，我是真的崩溃了啊！！请向开发者反馈以下信息\r反馈QQ群 957648723\r\r{e.Exception}";

        MessageBox.Show(message, "呃，发生了点意外", MessageBoxButton.OK, MessageBoxImage.Information);

        e.Handled = true; // 标记为已处理，防止应用程序终止
    }
}
