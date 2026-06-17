using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class DevicePortManage : UserControl
{
    public DevicePortManage()
    {
        InitializeComponent();
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e) { }
    private void SearchButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeviceListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void AddDeviceButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddSlotButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void EditSlotButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeleteSlotButton_OnClick(object? sender, RoutedEventArgs e) { }
}
