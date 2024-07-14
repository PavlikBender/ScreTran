using CommunityToolkit.Mvvm.ComponentModel;

namespace ScreTran;

public partial class WindowPositionModel : ObservableObject
{
    /// <summary>
    /// Window width.
    /// </summary>
    [ObservableProperty]
    private double _width;

    /// <summary>
    /// Window height.
    /// </summary>
    [ObservableProperty]
    private double _height;

    /// <summary>
    /// Window left position.
    /// </summary>
    [ObservableProperty]
    private double _left;

    /// <summary>
    /// Window top position.
    /// </summary>
    [ObservableProperty]
    private double _top;

    public WindowPositionModel()
    {
        Width = 1000.0;
        Height = 250.0;
        Left = 0;
        Top = 0;
    }
}
