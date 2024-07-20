using AnchorCalc.Bootstrapper.Common;
using AnchorCalc.Bootstrapper.Logging;
using AnchorCalc.Domain.Logging;
using AnchorCalc.Infrastructure.Common;
using Autofac;

namespace AnchorCalc.Bootstrapper;

public class ApplicationBootstrapper : IDisposable
{
    private readonly IContainer _container;

    public ApplicationBootstrapper()
    {
        var containerBuilder = new ContainerBuilder();
        RegisterDependencies(containerBuilder);
        _container = containerBuilder.Build();
        InitializeDependencies();
    }

    private void InitializeDependencies()
    {
        _container.Resolve<IPathServiceInitializer>().Initialize();
        _container.Resolve<ILogManagerInitializer>();
    }

    private void RegisterDependencies(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<Application>().As<IApplication>().SingleInstance();
        containerBuilder.RegisterType<PathService>().As<IPathService>().As<IPathServiceInitializer>().SingleInstance();
        containerBuilder.RegisterType<UnhandledExceptionHandler>().As<IUnhandledExceptionHandler>().SingleInstance();
        containerBuilder.RegisterType<LogManagerInitializer>().As<ILogManagerInitializer>().SingleInstance();
        containerBuilder.RegisterType<LogNotifier>().As<ILogNotifier>().As<ILogSubscriber>().SingleInstance();
    }

    public IApplication CreateApplication()
    {
        return _container.Resolve<IApplication>();
    }

    public IUnhandledExceptionHandler CreateUnhandledExceptionHandler()
    {
        return _container.Resolve<IUnhandledExceptionHandler>();
    }

    public void Dispose()
    {
        _container.Dispose();
        GC.SuppressFinalize(this);
    }
}