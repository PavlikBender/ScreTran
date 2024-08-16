namespace ScreTran;

public interface IParametersService
{
    /// <summary>
    /// Current translated line.
    /// </summary>
    string TranslatedLine
    {
        get;
        set;
    }

    /// <summary>
    /// Border thickness of selection area.
    /// </summary>
    int SelectionBorderThickness 
    {
        get; 
    }
}
