using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;

namespace ThinkITAM;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();

        // 根据 AppSettings 恢复主题
        var themeIndex = DataBridge.AppSettings.ThemeIndex;
        RequestedThemeVariant = themeIndex == 1 ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
