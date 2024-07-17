using System.Windows;

namespace ScreTran;

public interface IWindowService
{
    /// <summary>
    /// Close all showing windows.
    /// </summary>
    void CloseAll();

    /// <summary>
    /// Register window in service.
    /// </summary>
    void Register<T>() where T : Window;

    /// <summary>
    /// Show window by window name.
    /// </summary>
    void Show(string windowName);

    /// <summary>
    /// Minimize window by window name.
    /// </summary>
    void Minimize(string windowName);

    /// <summary>
    /// Normalize window by window name.
    /// </summary>
    void Normalize(string windowName);

    /// <summary>
    /// Установить владельца всех окон.
    /// </summary>
    /// <param name="owner">Владелец окон.</param>
    void SetOwner(Window owner);
}
