using CommunityToolkit.Mvvm.ComponentModel;

namespace ScreTran;

public partial class TranslationViewModel : ObservableObject
{
    /// <summary>
    /// Window position.
    /// </summary>
    [ObservableProperty]
    private WindowPositionModel _position;

    /// <summary>
    /// App settings.
    /// </summary>
    [ObservableProperty]
    private SettingsModel _settings;

    /// <summary>
    /// App parameters.
    /// </summary>
    [ObservableProperty]
    private IParametersService _parameters;

    public TranslationViewModel(ISettingsService settingsService, IParametersService parametersService) 
    {
        Parameters = parametersService;
        Settings = settingsService.Settings;
        Position = Settings.TranslationWindowPosition;
    }
}
