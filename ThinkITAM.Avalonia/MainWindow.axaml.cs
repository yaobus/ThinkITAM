using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ThinkITAM;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ProjectListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // TODO: implement selection logic
    }

    private void ProjectListView_OnDoubleTapped(object? sender, global::Avalonia.Input.TappedEventArgs e)
    {
        // TODO: implement double-tap logic
    }

    private void AddProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement add project
    }

    private void DelProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement delete project
    }

    private void SavePasswordButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement save password
    }

    private void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement login
    }

    private void ForgetPasswordButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement forget password
    }

    private void InputPasswordBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton_OnClick(null, null);
        }
    }

    private void ThemeToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement theme toggle
        if (Application.Current != null)
        {
            if (Application.Current.RequestedThemeVariant == Avalonia.Styling.ThemeVariant.Dark)
            {
                Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
            }
            else
            {
                Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
            }
        }
    }

    private void LanguageComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // TODO: implement language change
    }

    private void HelpButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: implement help
    }

    private void VersionLabel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // TODO: implement version label click
    }
}
