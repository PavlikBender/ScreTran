using System.Windows;

namespace ScreTran;

public class WindowService : IWindowService
{
    /// <summary>
    /// Registered windows.
    /// </summary>
    private readonly Dictionary<string, Type> _windows;

    /// <summary>
    /// Created windows.
    /// </summary>
    private readonly Dictionary<string, Window> _createdWindows;

    public WindowService()
    {
        _windows = new();
        _createdWindows = new();
    }

    /// <summary>
    /// Show window by window name.
    /// </summary>
    public void Show(string windowName)
    {
        // Если окно не зарегистрировано пропустить обработку.
        if (!_windows.ContainsKey(windowName))
            return;

        // Если окно не создано, создать его.
        if (!_createdWindows.ContainsKey(windowName))
            _createdWindows[windowName] = (Window)App.GetService(_windows[windowName]);

        _createdWindows[windowName].Show();
    }

    /// <summary>
    /// Minimize window by window name.
    /// </summary>
    public void Minimize(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return;

        // HACK если не устанавливать значение ShowInTaskbar - true, то окна не будут полность сворачиваться, а оставаться маленьким прямоугольником на экране.
        var showInTaskBar = _createdWindows[windowName].ShowInTaskbar;
        _createdWindows[windowName].ShowInTaskbar = true;

        _createdWindows[windowName].WindowState = WindowState.Minimized;

        _createdWindows[windowName].ShowInTaskbar = showInTaskBar;
    }

    /// <summary>
    /// Normalize window by window name.
    /// </summary>
    public void Normalize(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return;

        _createdWindows[windowName].WindowState = WindowState.Normal;
    }

    /// <summary>
    /// Close all showing windows.
    /// </summary>
    public void CloseAll()
    {
        _createdWindows.Select(item => item.Value).ToList().ForEach(w => w.Close());
    }

    /// <summary>
    /// Register window in service.
    /// </summary>
    public void Register<T>() where T : Window
    {
        lock (_windows)
        {
            var windowName = typeof(T).Name!;
            _windows[windowName] = typeof(T);
        }
    }
}
