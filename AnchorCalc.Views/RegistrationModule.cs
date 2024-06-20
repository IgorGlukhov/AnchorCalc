using AnchorCalc.ViewModels.Windows;
using AnchorCalc.Views.MainWindow;
using AnchorCalc.Views.Windows;
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
        builder.RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();
    }
}