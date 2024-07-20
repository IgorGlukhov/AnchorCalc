using System.Windows;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Infrastructure.Settings;
using AnchorCalc.ViewModels.DevTools;
using AnchorCalc.ViewModels.MainWindow;
using AnchorCalc.ViewModels.Windows;
using Autofac;
using NLog;

namespace AnchorCalc.Bootstrapper;

internal class Application : IApplication, IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLogger(nameof(Application));
    private readonly ILifetimeScope _applicationLifetimeScope;
    private IMainWindowViewModel? _mainWindowViewModel;

    public Application(ILifetimeScope lifetimeScope)
    {
        Logger.Info("Created");
        _applicationLifetimeScope = lifetimeScope.BeginLifetimeScope(RegisterDependencies);
    }

    public Window Run()
    {
        InitializeDependencies();
        var mainWindowViewModelFactory = _applicationLifetimeScope.Resolve<IFactory<IMainWindowViewModel>>();
        _mainWindowViewModel = mainWindowViewModelFactory.Create();
        var windowManager = _applicationLifetimeScope.Resolve<IWindowManager>();
        var mainWindow = windowManager.Show(_mainWindowViewModel);
        if (mainWindow is not Window window) throw new NotImplementedException();
        return window;
    }

    private static void RegisterDependencies(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterModule<Infrastructure.RegistrationModule>()
            .RegisterModule<ViewModels.RegistrationModule>()
            .RegisterModule<Views.RegistrationModule>()
            .RegisterModule<RegistrationModule>();
    }

    private void InitializeDependencies()
    {
        var windowMementoWrapperInitializers =
            _applicationLifetimeScope.Resolve<IEnumerable<IWindowMementoWrapperInitializer>>();
        foreach (var windowMementoWrapperInitializer in windowMementoWrapperInitializers)
            windowMementoWrapperInitializer.Initialize();
        _applicationLifetimeScope.Resolve<ILogEntryViewModelRepository>();
    }

    public void Dispose()
    {
        _mainWindowViewModel?.Dispose();
        _applicationLifetimeScope.Dispose();
        Logger.Info("Disposed");
        GC.SuppressFinalize(this);
    }
}