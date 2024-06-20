using AnchorCalc.Views.Factories;
using Autofac;

namespace AnchorCalc.Bootstrapper;

public class RegistrationModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<WindowFactory>().As<IWindowFactory>().SingleInstance();
    }
}