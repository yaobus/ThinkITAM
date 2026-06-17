using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class ComputerPage : UserControl
{
    public ComputerPage()
    {
        InitializeComponent();
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e) { }
    private void SearchButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeviceListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void AddDeviceButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void EditButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e) { }
}
