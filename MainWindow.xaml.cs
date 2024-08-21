using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Diagnostics;
using RawInput;

namespace ScreTran;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Thickness _restoredThickness;
    private readonly Thickness _maximizedThickness;

    public MainWindow()
    {
        InitializeComponent();
        StateChanged += MainWindowStateChangeRaised;
        RestoreButton.Visibility = Visibility.Collapsed;
        MaximizeButton.Visibility = Visibility.Collapsed;

        _restoredThickness = new Thickness(0);
        _maximizedThickness = new Thickness(8);

        MainWindowBorder.BorderThickness = _restoredThickness;

        Link.RequestNavigate += (sender, e) =>
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        };

        this.DataContext = App.GetService<MainWindowModel>();
    }

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }
    private void MaximizeWindow(object sender, RoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void RestoreWindow(object sender, RoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }

    // State change
    private void MainWindowStateChangeRaised(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            MainWindowBorder.BorderThickness = _maximizedThickness;
            RestoreButton.Visibility = Visibility.Visible;
            MaximizeButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            MainWindowBorder.BorderThickness = _restoredThickness;
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }
    }

    private void Expander_Expanded(object sender, RoutedEventArgs e)
    {
        if (ExpandedArea == null)
            return;

        Height += ExpandedArea.ActualHeight;
    }

    private void Expander_Collapsed(object sender, RoutedEventArgs e)
    {
        Height -= ExpandedArea.ActualHeight;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Expander.IsExpanded = false;

        var inputService = App.GetService<IInputService>();
        inputService.Initialize(new WindowInteropHelper(this).Handle);
    }
}