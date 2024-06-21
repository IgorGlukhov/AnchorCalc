using AnchorCalc.Domain.Settings;
using AnchorCalc.Domain.Version;
using AnchorCalc.Infrastructure.Common;
using AnchorCalc.Infrastructure.Settings;
using AnchorCalc.Infrastructure.Version;
using Autofac;

namespace AnchorCalc.Infrastructure;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MainWindowMementoWrapper>()
            .As<IMainWindowMementoWrapper>()
            .As<IWindowMementoWrapperInitializer>()
            .SingleInstance();
        builder.RegisterType<PathService>()
            .As<IPathService>()
            .As<IPathServiceInitializer>()
            .SingleInstance();
        builder.RegisterType<AboutWindowMementoWrapper>()
            .As<IAboutWindowMementoWrapper>()
            .As<IWindowMementoWrapperInitializer>()
            .SingleInstance();
        builder.RegisterType<ApplicationVersionProvider>().
            As<IApplicationVersionProvider>().
            SingleInstance();
    }
}