using AnchorCalc.Bootstrapper.Factories;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Views.Factories;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using WindowFactory = AnchorCalc.Views.Factories.WindowFactory;

namespace AnchorCalc.Bootstrapper;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<WindowFactory>().As<IWindowFactory>().SingleInstance();
        builder.RegisterGeneric(typeof(Factory<>)).As(typeof(IFactory<>)).SingleInstance();
        builder.Register(_ =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddHttpClient()
                    .BuildServiceProvider();
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                return httpClientFactory;
            })
            .SingleInstance();
    }
}