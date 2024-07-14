using System.IO;
using Newtonsoft.Json;

namespace ScreTran;

/// <summary>
/// Service responsible for Settings load/saving.
/// </summary>
public class SettingsService : ISettingsService
{
    /// <summary>
    /// Settings folder.
    /// </summary>
    private readonly string _path;

    /// <summary>
    /// Settings file name.
    /// </summary>
    private readonly string _filename;

    /// <summary>
    /// App settings.
    /// </summary>
    public SettingsModel Settings
    {
        get;
    }

    public SettingsService()
    {
        _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScreTran");
        _filename = Path.Combine(_path, "settings.json");
        Settings = Load();
    }

    /// <summary>
    /// Loads object from file.
    /// </summary>
    /// <returns>Filled settings object.</returns>
    private SettingsModel Load()
    {
        // Get object if file doesn't exist
        if (!File.Exists(_filename))
            return new SettingsModel();
        // Or empty.
        var json = File.ReadAllText(_filename);
        if (string.IsNullOrEmpty(json))
            return new SettingsModel();

        // Deserialize object from JSON.
        var settings = JsonConvert.DeserializeObject<SettingsModel>(json);

        if (settings == null)
            return new SettingsModel();

        return settings;
    }

    /// <summary>
    /// Saves object to file.
    /// </summary>
    /// <param name="obj">Object to save.</param>
    public void Save(object obj)
    {
        // Serialize to JSON.
        var serializedSettings = JsonConvert.SerializeObject(obj, Formatting.Indented);

        // Create path.
        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        // Write to file.
        var settingsFile = File.CreateText(_filename);
        settingsFile.Write(serializedSettings);
        settingsFile.Close();
    }
}
