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
    /// App parameters.
    /// </summary>
    [ObservableProperty]
    private IParametersService _parameters;

    public SelectionViewModel(ISettingsService settingsService, IParametersService parametersService) 
    { 
        Position = settingsService.Settings.SelectionWindowPosition;
        Parameters = parametersService;
    }
}
