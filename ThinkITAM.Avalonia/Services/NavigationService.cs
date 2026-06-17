using Avalonia.Controls;

namespace ThinkITAM.Services
{
    /// <summary>
    /// 页面导航服务
    /// 基于 ContentControl + UserControl 的页面切换
    /// 替代原始 WPF SelectionWindow 中的 FunctionPanel.Children.Clear()/Add() 模式
    /// </summary>
    public static class NavigationService
    {
        /// <summary>
        /// 页面容器引用
        /// </summary>
        private static ContentControl? _contentHost;

        /// <summary>
        /// 初始化导航服务
        /// </summary>
        public static void Initialize(ContentControl contentHost)
        {
            _contentHost = contentHost;
        }

        /// <summary>
        /// 导航到指定页面
        /// </summary>
        public static void NavigateTo(UserControl page)
        {
            if (_contentHost == null)
                throw new InvalidOperationException("NavigationService 未初始化。请先调用 Initialize()。");

            _contentHost.Content = page;
        }

        /// <summary>
        /// 清除当前页面
        /// </summary>
        public static void Clear()
        {
            if (_contentHost != null)
                _contentHost.Content = null;
        }
    }
}
