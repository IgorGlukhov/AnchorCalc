using System.Windows;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Infrastructure.Common;
using AnchorCalc.Infrastructure.Settings;
using AnchorCalc.ViewModels.MainWindow;
using AnchorCalc.ViewModels.Windows;
using Autofac;

namespace AnchorCalc.Bootstrapper;

public class Bootstrapper : IDisposable
{
    private readonly IContainer _container;
    private IMainWindowViewModel _mainWindowViewModel;

    public Bootstrapper()
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder
            .RegisterModule<Infrastructure.RegistrationModule>()
            .RegisterModule<ViewModels.RegistrationModule>()
            .RegisterModule<Views.RegistrationModule>()
            .RegisterModule<RegistrationModule>();
        _container = containerBuilder.Build();
    }

    public Window Run()
    {
        InitializeDependencies();
        var mainWindowViewModelFactory = _container.Resolve<IFactory<IMainWindowViewModel>>();
        _mainWindowViewModel = mainWindowViewModelFactory.Create();
        var windowManager = _container.Resolve<IWindowManager>();
        var mainWindow = windowManager.Show(_mainWindowViewModel);
        if (mainWindow is not Window window) throw new NotImplementedException();
        return window;
    }

    private void InitializeDependencies()
    {
        _container.Resolve<IPathServiceInitializer>().Initialize();
        var windowMementoWrapperInitializers = _container.Resolve<IEnumerable<IWindowMementoWrapperInitializer>>();
        foreach (var windowMementoWrapperInitializer in windowMementoWrapperInitializers)
            windowMementoWrapperInitializer.Initialize();
    }

    public void Dispose()
    {
        _mainWindowViewModel.Dispose();
        _container.Dispose();
    }
}