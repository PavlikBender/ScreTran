using CommunityToolkit.Mvvm.ComponentModel;

namespace ScreTran;

public partial class SelectionViewModel : ObservableObject
{
    /// <summary>
    /// Window position.
    /// </summary>
    [ObservableProperty]
    public WindowPositionModel _position;

    /// <summary>
    /// Selection area border thickness.
    /// </summary>
    [ObservableProperty]
    public int _borderThickness;

    public SelectionViewModel(ISettingsService settingsService, IParametersService parametersService) 
    { 
        Position = settingsService.Settings.SelectionWindowPosition;
        BorderThickness = parametersService.SelectionBorderThickness;
    }
}
