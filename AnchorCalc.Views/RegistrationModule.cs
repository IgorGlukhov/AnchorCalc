using AnchorCalc.Views.MainWindow;
using Autofac;

namespace AnchorCalc.Views;

public class RegistrationModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder
            .RegisterType<MainWindow.MainWindow>()
            .As<IMainWindow>()
            .InstancePerDependency();
    }
}