using System.Windows;
using System.Windows.Media;

namespace ThinkITAM.Functions.FunctionClass;
public static class FrameworkElementExtensions
{
    /// <summary>
    /// 查找祖先元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="child"></param>
    /// <returns></returns>
    public static T FindAncestor<T>(this DependencyObject child) where T : DependencyObject
    {
        while (child != null && !(child is T))
        {
            child = VisualTreeHelper.GetParent(child);
        }
        return child as T;
    }

}
