using AnchorCalc.Infrastructure.Settings;
using AnchorCalc.Domain.Settings;
using Autofac;

namespace AnchorCalc.Infrastructure;

public class RegistrationModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MainWindowMementoWrapper>()
            .As<IMainWindowMementoWrapper>()
            .As<IMainWindowMementoWrapperInitializer>()
            .SingleInstance();
    }
}