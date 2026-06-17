using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class LinkUserControl : UserControl
{
    public LinkUserControl()
    {
        InitializeComponent();
    }

    private void AddRoomButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddCabinetButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddRackButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void RoomListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void RemoveDeviceButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void ClearRackButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void SaveLinkButton_OnClick(object? sender, RoutedEventArgs e) { }
}
