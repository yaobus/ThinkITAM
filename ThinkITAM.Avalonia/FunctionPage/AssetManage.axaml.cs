using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class AssetManage : UserControl
{
    public AssetManage()
    {
        InitializeComponent();
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e) { }
    private void SearchButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AssetTypeListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void AddAssetButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void ImportAssetButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void ExportAssetButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void AddButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void EditButton_OnClick(object? sender, RoutedEventArgs e) { }
    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e) { }
}
