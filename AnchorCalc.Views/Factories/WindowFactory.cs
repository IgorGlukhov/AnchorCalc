using AnchorCalc.ViewModels.AboutWindow;
using AnchorCalc.ViewModels.MainWindow;
using AnchorCalc.ViewModels.Windows;
using AnchorCalc.Views.AboutWindow;
using AnchorCalc.Views.MainWindow;
using Autofac;

namespace AnchorCalc.Views.Factories;

public class WindowFactory : IWindowFactory
{
    private readonly IComponentContext _componentContext;

    private readonly Dictionary<Type, Type> _map = new()
    {
        { typeof(IMainWindowViewModel), typeof(IMainWindow) },
        { typeof(IAboutWindowViewModel), typeof(IAboutWindow) }
    };

    public WindowFactory(IComponentContext componentContext)
    {
        _componentContext = componentContext;
    }

    public IWindow Create<TWindowViewModel>(TWindowViewModel viewModel) where TWindowViewModel : IWindowViewModel
    {
        if (!_map.TryGetValue(typeof(TWindowViewModel), out var windowType))
            throw new InvalidOperationException($"There is no window registered for {typeof(TWindowViewModel)}");

        var instance = _componentContext.Resolve(windowType, TypedParameter.From(viewModel));
        return (IWindow)instance;
    }
}