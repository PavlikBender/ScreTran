using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ScreTran;

public class WindowService : IWindowService
{
    // Константы для скрытия и удержания окна на переднем плане.
    private const int SWP_NOMOVE = 0x0002;
    private const int SWP_NOSIZE = 0x0001;
    private const int SWP_NOACTIVATE = 0x0010;
    private const int HWND_TOP = 0;

    private const int SWP_SHOWWINDOW = 0x0040;
    private const int SWP_HIDEWINDOW = 0x0080;

    // Структура для хранения прямоугольника
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // Метод для получения прямоугольника окна.
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    // Метод для удержания на переднем плане.
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    // Владелец всех окон.
    private Window? _owner;

    // Его хендл.
    private IntPtr _ownerHandle;

    private readonly Timer _timer;

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
        _owner = null;
        _ownerHandle = IntPtr.Zero;
        _windows = new();
        _createdWindows = new();

        _timer = new Timer(ProccessByTimerCommands, null, 0, 1000);
    }

    /// <summary>
    /// Timer execution.
    /// </summary>
    private void ProccessByTimerCommands(object? state)
    {
        if (_ownerHandle == IntPtr.Zero)
            return;
        // Таймер каждую секунду старается выставить окно владельца и его наследников на передний план.
        SetWindowPos(_ownerHandle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    /// <summary>
    /// Установить владельца всех окон.
    /// </summary>
    /// <param name="owner">Владелец окон.</param>
    public void SetOwner(Window owner)
    {
        _owner = owner;
        _ownerHandle = new WindowInteropHelper(_owner).Handle;
        foreach (var window in _createdWindows.Values.Where(w => !Equals(w, owner)))
        {
            window.Owner = _owner;
        }
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
        {
            _createdWindows[windowName] = (Window)App.GetService(_windows[windowName]);
        }

        if (_owner?.IsLoaded == true && !Equals(_owner, _createdWindows[windowName]))
            _createdWindows[windowName].Owner = _owner;

        _createdWindows[windowName].Show();
    }

    /// <summary>
    /// Minimize window by window name.
    /// </summary>
    public void Minimize(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return;
        var handle = new WindowInteropHelper(_createdWindows[windowName]).Handle;
        SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_HIDEWINDOW);
    }

    /// <summary>
    /// Normalize window by window name.
    /// </summary>
    public void Normalize(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return;
        var handle = new WindowInteropHelper(_createdWindows[windowName]).Handle;
        SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    /// <summary>
    /// Gets window coordinates.
    /// </summary>
    public Rectangle? GetWindowCoordinates(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return null;
        
        var handle = Application.Current.Dispatcher.Invoke(() => new WindowInteropHelper(_createdWindows[windowName]).Handle);

        if (GetWindowRect(handle, out RECT rect))
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

        return null;
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
