using CommunityToolkit.Mvvm.ComponentModel;

namespace ScreTran;

public partial class ParametersService : ObservableObject
{
    /// <summary>
    /// Translated line.
    /// </summary>
    [ObservableProperty]
    private string _translatedLine;

    /// <summary>
    /// Border thickness of selection area.
    /// </summary>
    [ObservableProperty]
    private int _selectionBorderThickness;

    public ParametersService()
    {
        SelectionBorderThickness = 5;
        TranslatedLine = "<Окно перевода>";
    }
}
