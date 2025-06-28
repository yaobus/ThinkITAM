using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ThinkITAM.Functions.FunctionClass
{
    public static class TreeViewItemHelper
    {

        public static object GetParentDataContext(DependencyObject child)
        {
            // 获取视觉树中的父级元素
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            // 检查是否到达了顶级窗口或者不是TreeViewItem
            if (parent == null || !(parent is TreeViewItem))
            {
                return null;
            }

            // 如果父级是TreeViewItem，则返回它的DataContext
            var treeViewItem = parent as TreeViewItem;
            if (treeViewItem != null)
            {
                return treeViewItem.DataContext;
            }

            // 否则继续向上查找
            return GetParentDataContext(parent);
        }

        public static object GetParentOfType(DependencyObject child, Type parentType)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            // 检查是否到达了顶级窗口或者不是期望的类型
            if (parent == null || !parentType.IsInstanceOfType(parent))
            {
                // 如果还没有达到顶级并且不是期望的类型，则继续向上查找
                return parent != null ? GetParentOfType(parent, parentType) : null;
            }

            // 如果找到了指定类型的父级控件，则返回它的DataContext
            return parent;
        }


        /// <summary>
        /// 获取父级控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            // 检查父级是否是目标类型
            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                // 递归查找
                return FindParent<T>(parentObject);
            }
        }
    }


}
