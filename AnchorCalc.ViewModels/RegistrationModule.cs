using AnchorCalc.ViewModels.AboutWindow;
using AnchorCalc.ViewModels.Extensions;
using AnchorCalc.ViewModels.MainWindow;
using Autofac;

namespace AnchorCalc.ViewModels;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterViewModel<MainWindowViewModel,IMainWindowViewModel>();
        builder.RegisterViewModel<AboutWindowViewModel,IAboutWindowViewModel>();
        builder.RegisterViewModel<MainWindowMenuViewModel,IMainWindowMenuViewModel>();
        builder.RegisterViewModel<MainWindowStatusBarViewModel,IMainWindowStatusBarViewModel>();
    }
}