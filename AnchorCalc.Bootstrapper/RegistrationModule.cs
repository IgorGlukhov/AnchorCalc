using AnchorCalc.Bootstrapper.Factories;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Views.Factories;
using Autofac;
using WindowFactory = AnchorCalc.Views.Factories.WindowFactory;

namespace AnchorCalc.Bootstrapper;

public class RegistrationModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<WindowFactory>().As<IWindowFactory>().SingleInstance();
        builder.RegisterGeneric(typeof(Factory<>)).As(typeof(IFactory<>)).SingleInstance();
    }
}