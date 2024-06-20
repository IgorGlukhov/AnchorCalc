using AnchorCalc.ViewModels.Windows;
using AnchorCalc.Views.Factories;
using System.Collections.Generic;

namespace AnchorCalc.Views.Windows;

internal class WindowManager : IWindowManager
{
    private readonly Dictionary<IWindowViewModel, IWindow> _viewModelToWindowMap = new();
    private readonly IWindowFactory _windowFactory;

    public WindowManager(IWindowFactory windowFactory)
    {
        _windowFactory = windowFactory;
    }

    public IWindow Show<TWindowViewModel>(TWindowViewModel viewModel) 
        where TWindowViewModel : IWindowViewModel
    {
        var window = _windowFactory.Create(viewModel);
        _viewModelToWindowMap.Add(viewModel, window);
        window.Show();
        return window;
    }

    public void Close<TWindowViewModel>(TWindowViewModel viewModel) 
        where TWindowViewModel : IWindowViewModel
    {
        if (_viewModelToWindowMap.TryGetValue(viewModel, out var window))
        {
            window.Close();
            _viewModelToWindowMap.Remove(viewModel);
        }
    }
}