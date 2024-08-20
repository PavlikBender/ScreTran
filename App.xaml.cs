using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RawInput;

namespace ScreTran;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        ServiceProvider = ConfigureServices();

        RegisterWindows();
    }

    /// <summary>
    /// Existing windows registration for windows service.
    /// </summary>
    private static void RegisterWindows()
    {
        var windowService = GetService<IWindowService>();
        windowService.Register<SelectionWindow>();
        windowService.Register<TranslationWindow>();
    }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider ServiceProvider
    {
        get;
    }

    /// <summary>
    /// Returns current application instance.
    /// </summary>
    public static App Instance => Current as App;

    /// <summary>
    /// Gets service by type.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <returns>Service.</returns>
    public static T GetService<T>() => Instance.ServiceProvider.GetService<T>();

    /// <summary>
    /// Gets service by type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns>Service.</returns>
    public static object GetService(Type type) => Instance.ServiceProvider.GetService(type);

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IParametersService, ParametersService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IExecutionService, ExecutionService>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<IInputService, RawHook>();

        services.AddTransient<MainWindowModel>();
        services.AddTransient<SelectionViewModel>();
        services.AddTransient<TranslationViewModel>();

        services.AddTransient<SelectionWindow>();
        services.AddTransient<TranslationWindow>();

        return services.BuildServiceProvider();
    }
}

