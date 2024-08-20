using System.Windows;
using System.Windows.Input;

namespace ScreTran;

/// <summary>
/// Логика взаимодействия для SelectionWindow.xaml
/// </summary>
public partial class SelectionWindow : Window
{
    Point _mouseDelta;

    public SelectionWindow()
    {
        InitializeComponent();

        _mouseDelta = new Point(0, 0);

        this.DataContext = App.GetService<SelectionViewModel>();

        Loaded += Window_Loaded;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var windowService = App.GetService<IWindowService>();
        windowService.SetOwner(this);
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var mousePos = PointToScreen(e.GetPosition(this));
            Left = mousePos.X - _mouseDelta.X;
            Top = mousePos.Y - _mouseDelta.Y;
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePos = PointToScreen(e.GetPosition(this));
        _mouseDelta = new Point(mousePos.X - Left, mousePos.Y - Top);
        CaptureMouse();
    }

    private static void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ReleaseMouseCapture();
    }
}
