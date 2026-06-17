using Avalonia;
using Avalonia.Styling;

namespace ThinkITAM.Services;

/// <summary>
/// 主题切换服务
/// 替代 WPF MaterialDesignThemes 的 PaletteHelper
/// </summary>
public static class ThemeService
{
    /// <summary>
    /// 切换明暗主题
    /// </summary>
    public static void ToggleTheme()
    {
        var app = Application.Current;
        if (app == null) return;
        
        app.RequestedThemeVariant = app.ActualThemeVariant == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }

    /// <summary>
    /// 设置指定主题
    /// </summary>
    public static void SetTheme(ThemeVariant variant)
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = variant;
    }

    /// <summary>
    /// 当前是否为暗色主题
    /// </summary>
    public static bool IsDarkTheme =>
        Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
}
