using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors;
using RawInput;

namespace ScreTran;

public partial class MainWindowModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IWindowService _windowService;
    private readonly IExecutionService _executionService;
    private readonly IInputService _inputSevice;

    private bool _isMinimized;

    /// <summary>
    /// Is execution service started?
    /// </summary>
    [ObservableProperty]
    private bool _isStarted;

    /// <summary>
    /// Is key sets in current moment.
    /// </summary>
    [ObservableProperty]
    private bool _isKeySetting;

    /// <summary>
    /// App settings.
    /// </summary>
    [ObservableProperty]
    private SettingsModel _settings;

    /// <summary>
    /// List of behaviours.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Enumerations.Translator> _translators;

    /// <summary>
    /// Start execution.
    /// </summary>
    [RelayCommand]
    private void Start()
    {
        _executionService.Start();
        IsStarted = true;
    }

    /// <summary>
    /// Stop execution.
    /// </summary>
    [RelayCommand]
    private void Stop()
    {
        _executionService.Stop();
        IsStarted = false;
    }

    /// <summary>
    /// App close event.
    /// </summary>
    [RelayCommand]
    private void Close()
    {
        _settingsService.Save(Settings);
        _windowService.CloseAll();
    }

    /// <summary>
    /// Clear hotkey.
    /// </summary>
    [RelayCommand]
    private void ClearKey()
    {
        Settings.Key = new Key();
    }

    /// <summary>
    /// Set hotkey.
    /// </summary>
    [RelayCommand]
    private void SetKey()
    {
        IsKeySetting = true;
    }

    /// <summary>
    /// Resets windows position.
    /// </summary>
    [RelayCommand]
    private void ResetWindowsPosition()
    {
        Settings.ResetWindowPositions();
    }

    /// <summary>
    /// Reset settings to default.
    /// </summary>
    [RelayCommand]
    private void ResetSettingsToDefault()
    {
        Settings.ResetToDefault();
    }

    public MainWindowModel(ISettingsService settingsService, IParametersService parametersService, IWindowService windowService, IExecutionService executionService, IInputService inputService)
    {
        _settingsService = settingsService;
        Settings = _settingsService.Settings;

        _translators =
        [
            Enumerations.Translator.Google,
            Enumerations.Translator.Yandex,
            Enumerations.Translator.Bing,
        ];

        IsStarted = false;

        IsKeySetting = false;

        _executionService = executionService;

        _windowService = windowService;
        _windowService.Show("SelectionWindow");
        _windowService.Show("TranslationWindow");

        _inputSevice = inputService;
        _inputSevice.KeyDown += InputSevice_KeyDown;
        _isMinimized = false;
    }

    /// <summary>
    /// Key down event processing.
    /// </summary>
    private void InputSevice_KeyDown(HookEventArgs e)
    {
        if (IsKeySetting)
        {
            if (e.Key.Code != KeyCodes.Esc)
                Settings.Key = e.Key;

            IsKeySetting = false;
            return;
        }

        if (Equals(e.Key, Settings.Key))
        {
            if (_isMinimized)
            {
                _windowService.Normalize("SelectionWindow");
                _windowService.Normalize("TranslationWindow");
                Start();
            }
            else
            {
                _windowService.Minimize("SelectionWindow");
                _windowService.Minimize("TranslationWindow");
                Stop();
            }

            _isMinimized = !_isMinimized;
        }
    }
}
