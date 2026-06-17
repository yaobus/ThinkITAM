using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class IndexPage : UserControl
{
    public IndexPage()
    {
        InitializeComponent();
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e) { }
    private void SearchButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void SearchKeyWord_OnKeyDown(object? sender, KeyEventArgs e) { }
    private void GroupsListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void AddButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddGroupButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e) { }
}
