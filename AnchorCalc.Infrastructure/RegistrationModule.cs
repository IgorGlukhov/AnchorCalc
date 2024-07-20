using AnchorCalc.Domain.Collections;
using AnchorCalc.Domain.DevTools;
using AnchorCalc.Domain.Rest;
using AnchorCalc.Domain.Settings;
using AnchorCalc.Domain.Version;
using AnchorCalc.Infrastructure.Collections;
using AnchorCalc.Infrastructure.DevTools;
using AnchorCalc.Infrastructure.Rest;
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
        builder.RegisterType<AboutWindowMementoWrapper>()
            .As<IAboutWindowMementoWrapper>()
            .As<IWindowMementoWrapperInitializer>()
            .SingleInstance();
        builder.RegisterType<ApplicationVersionProvider>().As<IApplicationVersionProvider>().SingleInstance();
        builder.RegisterType<DevToolsStatusProvider>().As<IDevToolsStatusProvider>().SingleInstance();
        builder.RegisterType<ApiRequestExecutor>().As<IApiRequestExecutor>().SingleInstance();
        builder.RegisterType<RotatableCollectionFactory>().As<IRotatableCollectionFactory>().SingleInstance();
    }
}