using CommunityToolkit.Mvvm.ComponentModel;
using RawInput;

namespace ScreTran;

public partial class SettingsModel : ObservableObject
{
    /// <summary>
    /// Font size.
    /// </summary>
    [ObservableProperty]
    private int _fontSize;

    /// <summary>
    /// Key.
    /// </summary>
    [ObservableProperty]
    private IKey _key;

    /// <summary>
    /// Timer period.
    /// </summary>
    [ObservableProperty]
    private float _period;


    /// <summary>
    /// Translator.
    /// </summary>
    [ObservableProperty]
    private Enumerations.Translator _translator;

    /// <summary>
    /// Model.
    /// </summary>
    [ObservableProperty]
    private Enumerations.Model _ocrModel;

    /// <summary>
    /// Selection window position.
    /// </summary>
    [ObservableProperty]
    private WindowPositionModel _selectionWindowPosition;

    /// <summary>
    /// Translation window position.
    /// </summary>
    [ObservableProperty]
    private WindowPositionModel _translationWindowPosition;


    public SettingsModel()
    {
        // Load default values.
        ResetToDefault();

        SelectionWindowPosition = new WindowPositionModel();
        TranslationWindowPosition = new WindowPositionModel();

    }

    /// <summary>
    /// Reset window positions to 0.
    /// </summary>
    public static void ResetWindowPositions()
    {
        SelectionWindowPosition.Left = 0;
        SelectionWindowPosition.Top = 0;
        TranslationWindowPosition.Left = 0;
        TranslationWindowPosition.Top = 0;
    }

    /// <summary>
    /// Reset settings to default values.
    /// </summary>
    public static void ResetToDefault()
    {
        FontSize = 21;
        Key = new Key(0x7B);
        Period = 1.0f;
        Translator = Enumerations.Translator.Google;
        OcrModel = Enumerations.Model.English;
    }
}
