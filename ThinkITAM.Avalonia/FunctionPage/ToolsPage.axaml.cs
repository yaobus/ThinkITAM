using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class ToolsPage : UserControl
{
    public ToolsPage()
    {
        InitializeComponent();
    }

    private void NetworkCalculate_OnClick(object? sender, RoutedEventArgs e) { }
    private void PortScanToolButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void MacVendorButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DiskCalculationButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void WakeOnLanButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void NotePadButton_OnClick(object? sender, RoutedEventArgs e) { }
}
