using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.Views.Factories;

public interface IWindowFactory
{
    IWindow Create<TWindowViewModel>(TWindowViewModel viewModel)
        where TWindowViewModel : IWindowViewModel;
}