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

    private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int GWL_EXSTYLE = (-20);

    // Структура для хранения прямоугольника
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Right - Left, Bottom - Top);
        }
    }

    // Метод для скрытия окна при захвате изображения.
    [DllImport("user32.dll")]
    public static extern uint SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);

    // Метод необходимый для получение прямоугольника окна.
    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(nint hWnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    // Метод для удержания на переднем плане.
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern nint SetWindowPos(nint hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(nint hwnd, int index, int newStyle);

    // Владелец всех окон.
    private Window? _owner;

    // Его хендл.
    private nint _ownerHandle;

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
        _ownerHandle = nint.Zero;
        _windows = new();
        _createdWindows = new();

        _timer = new Timer(ProccessByTimerCommands, null, 0, 1000);
    }

    /// <summary>
    /// Timer execution.
    /// </summary>
    private void ProccessByTimerCommands(object? state)
    {
        if (_ownerHandle == nint.Zero)
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
    /// Get window handle by window name.
    /// </summary>
    /// <param name="windowName">Window name.</param>
    /// <returns>Window handle or nint.Zero.</returns>
    public nint GetHandle(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return nint.Zero;
        return new WindowInteropHelper(_createdWindows[windowName]).Handle;
    }


    /// <summary>
    /// Set window click thru style.
    /// </summary>
    public void SetWindowClickThru(string windowName)
    {
        var handle = GetHandle(windowName);
        if (handle == nint.Zero)
            return;

        var extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
        SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
    }

    /// <summary>
    /// Revert click thru style.
    /// </summary>
    public void SetWindowClickable(string windowName)
    {
        var handle = GetHandle(windowName);
        if (handle == nint.Zero)
            return;

        var extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
        SetWindowLong(handle, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
    }

    /// <summary>
    /// Exclude window from capture.
    /// </summary>
    public void ExcludeFromCapture(string windowName)
    {
        var handle = GetHandle(windowName);
        if (handle == nint.Zero)
            return;

        SetWindowDisplayAffinity(handle, WDA_EXCLUDEFROMCAPTURE);
    }


    /// <summary>
    /// Получить прямоугольник окна. Аналог GetWindowRect, но более современный, так как возвращает точные координаты, а не координаты с учетом тени.
    /// </summary>
    /// <param name="handle">Хэндл окна.</param>
    /// <param name="rectangle">Прямоугольник в который запишутся координаты окна.</param>
    /// <returns>true - если удалось получить координаты окна. Иначе - false.</returns>
    public bool GetWindowRectWithoutShadow(nint handle, out Rectangle rectangle)
    {
        var result = DwmGetWindowAttribute(handle, 9, out RECT rect, Marshal.SizeOf(typeof(RECT)));
        rectangle = rect.ToRectangle();
        return result >= 0;
    }

    /// <summary>
    /// Gets window coordinates.
    /// </summary>
    public Rectangle? GetWindowCoordinates(string windowName)
    {
        if (!_createdWindows.ContainsKey(windowName))
            return null;
        
        var handle = Application.Current?.Dispatcher.Invoke(() => new WindowInteropHelper(_createdWindows[windowName]).Handle);

        if (handle == null)
            return null;

        if (GetWindowRectWithoutShadow(handle.Value, out var rectangle))
            return rectangle;

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
