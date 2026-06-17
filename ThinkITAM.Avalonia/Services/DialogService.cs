using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ThinkITAM.Services;

/// <summary>
/// 全局对话框服务
/// 基于 MessageBox.Avalonia v12.0.0，替代 WPF MessageBox 和 materialDesign:DialogHost
/// </summary>
public static class DialogService
{
    /// <summary>
    /// 获取主窗口引用
    /// </summary>
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    /// <summary>
    /// 显示信息提示
    /// </summary>
    public static async Task ShowInfo(string message, string? title = "提示")
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null) return;
        
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title: title!,
            text: message,
            @enum: ButtonEnum.Ok,
            icon: Icon.Info);

        await msgBox.ShowWindowDialogAsync(mainWindow);
    }

    /// <summary>
    /// 显示错误提示
    /// </summary>
    public static async Task ShowError(string message, string? title = "错误")
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null) return;
        
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title: title!,
            text: message,
            @enum: ButtonEnum.Ok,
            icon: Icon.Error);

        await msgBox.ShowWindowDialogAsync(mainWindow);
    }

    /// <summary>
    /// 显示警告提示
    /// </summary>
    public static async Task ShowWarning(string message, string? title = "警告")
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null) return;
        
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title: title!,
            text: message,
            @enum: ButtonEnum.Ok,
            icon: Icon.Warning);

        await msgBox.ShowWindowDialogAsync(mainWindow);
    }

    /// <summary>
    /// 显示确认对话框
    /// </summary>
    public static async Task<bool> ShowConfirm(string message, string? title = "确认")
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null) return false;
        
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title: title!,
            text: message,
            @enum: ButtonEnum.YesNo,
            icon: Icon.Question);

        var result = await msgBox.ShowWindowDialogAsync(mainWindow);
        return result == ButtonResult.Yes;
    }

    /// <summary>
    /// 显示删除确认对话框
    /// </summary>
    public static async Task<bool> ShowDeleteConfirm(string itemName)
    {
        return await ShowConfirm(
            $"确定要删除「{itemName}」吗？此操作不可恢复。",
            "确认删除");
    }

    /// <summary>
    /// 显示自定义按钮对话框
    /// </summary>
    public static async Task<ButtonResult> ShowCustom(
        string message,
        string title,
        Icon icon = Icon.None,
        ButtonEnum buttons = ButtonEnum.OkCancel)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null) return ButtonResult.Cancel;
        
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title: title,
            text: message,
            @enum: buttons,
            icon: icon);

        return await msgBox.ShowWindowDialogAsync(mainWindow);
    }
}
