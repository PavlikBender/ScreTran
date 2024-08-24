using System.Windows;
using System.Windows.Input;

namespace ScreTran;

/// <summary>
/// Логика взаимодействия для SelectionWindow.xaml
/// </summary>
public partial class SelectionWindow : Window
{
    public SelectionWindow()
    {
        InitializeComponent();

        this.DataContext = App.GetService<SelectionViewModel>();
        Loaded += Window_Loaded;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var windowService = App.GetService<IWindowService>();
        windowService.SetOwner(this);
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }
}
