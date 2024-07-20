using AnchorCalc.ViewModels.AboutWindow;
using AnchorCalc.ViewModels.Anchors;
using AnchorCalc.ViewModels.DevTools;
using AnchorCalc.ViewModels.Extensions;
using AnchorCalc.ViewModels.MainWindow;
using Autofac;

namespace AnchorCalc.ViewModels;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterViewModel<MainWindowViewModel, IMainWindowViewModel>();
        builder.RegisterViewModel<AboutWindowViewModel, IAboutWindowViewModel>();
        builder.RegisterViewModel<MainWindowMenuViewModel, IMainWindowMenuViewModel>();
        builder.RegisterViewModel<MainWindowStatusBarViewModel, IMainWindowStatusBarViewModel>();
        builder.RegisterViewModel<MainWindowSurfacePlotViewModel, IMainWindowSurfacePlotViewModel>();
        builder.RegisterViewModel<DevToolsMenuViewModel, IDevToolsMenuViewModel>();
        builder.RegisterViewModel<AnchorCollectionViewModel, IAnchorCollectionViewModel>();
        builder.RegisterViewModel<LogViewerViewModel, ILogViewerViewModel>();
        builder.RegisterType<LogEntryViewModelRepository>().As<ILogEntryViewModelRepository>().SingleInstance();
    }
}