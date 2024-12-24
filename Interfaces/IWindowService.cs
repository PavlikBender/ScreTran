using System.Drawing;
using System.Windows;

namespace ScreTran;

public interface IWindowService
{
    /// <summary>
    /// Gets window coordinates.
    /// </summary>
    Rectangle? GetWindowCoordinates(string windowName);

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
    /// Set window click thru style.
    /// </summary>
    void SetWindowClickThru(string windowName);

    /// <summary>
    /// Revert click thru style.
    /// </summary>
    void SetWindowClickable(string windowName);

    /// <summary>
    /// Exclude window from capture.
    /// </summary>
    void ExcludeFromCapture(string windowName);

    /// <summary>
    /// Установить владельца всех окон.
    /// </summary>
    /// <param name="owner">Владелец окон.</param>
    void SetOwner(Window owner);
}
