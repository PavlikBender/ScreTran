namespace ScreTran;

public interface ISettingsService
{
    /// <summary>
    /// App settings.
    /// </summary>
    SettingsModel Settings { get; }

    /// <summary>
    /// Saves object to file.
    /// </summary>
    /// <param name="obj">Object to save.</param>
    void Save(object obj);
}
