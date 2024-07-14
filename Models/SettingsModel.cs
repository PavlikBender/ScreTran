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
    /// Confidence threshold.
    /// </summary>
    [ObservableProperty]
    private float _confidenceThreshold;

    /// <summary>
    /// Removes short lines.
    /// </summary>
    [ObservableProperty]
    private int _shortLineThreshold;

    /// <summary>
    /// Screenshot brightness for Tesseract Engine.
    /// </summary>
    [ObservableProperty]
    private float _brightness;

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
    public void ResetWindowPositions()
    {
        SelectionWindowPosition.Left = 0;
        SelectionWindowPosition.Top = 0;
        TranslationWindowPosition.Left = 0;
        TranslationWindowPosition.Top = 0;
    }

    /// <summary>
    /// Reset settings to default values.
    /// </summary>
    public void ResetToDefault()
    {
        FontSize = 21;
        Key = new Key(0x7B);
        ConfidenceThreshold = 0.75f;
        ShortLineThreshold = 3;
        Brightness = -1.94f;
    }
}
