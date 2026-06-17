using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ThinkITAM.FunctionPage;

public partial class PresetPage : UserControl
{
    public PresetPage()
    {
        InitializeComponent();
    }

    private void PresetTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void PresetSubPanel_OnSizeChanged(object? sender, SizeChangedEventArgs e) { }
}
