using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.Windows.Selection;

public partial class SelectionWindow : Window
{
    public SelectionWindow()
    {
        InitializeComponent();
    }

    private void MenuList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // TODO: 实现菜单导航逻辑
    }

    private void MenuList2_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // TODO: 实现底部菜单导航逻辑
    }
}
