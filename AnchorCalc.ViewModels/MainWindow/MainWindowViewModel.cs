using AnchorCalc.Domain.Factories;
using AnchorCalc.Domain.Settings;
using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowViewModel : WindowViewModel<IMainWindowMementoWrapper>, IMainWindowViewModel
{
    private readonly IWindowManager _windowManager;

    public MainWindowViewModel(
        IMainWindowMementoWrapper mainWindowMementoWrapper,
        IWindowManager windowManager,
        IFactory<IMainWindowStatusBarViewModel> mainWindowStatusBarViewModelFactory,
        IFactory<IMainWindowMenuViewModel> mainWindowMenuViewModelFactory,
        IFactory<IMainWindowSurfacePlotViewModel> mainWindowSurfacePlotViewModelFactory)
        : base(mainWindowMementoWrapper)
    {
        _windowManager = windowManager;
        StatusBarViewModel = mainWindowStatusBarViewModelFactory.Create();
        MenuViewModel = mainWindowMenuViewModelFactory.Create();
        SurfacePlotViewModel = mainWindowSurfacePlotViewModelFactory.Create();
        MenuViewModel.MainWindowClosingRequested += OnMainWindowClosingRequested;
    }

    public IMainWindowMenuViewModel MenuViewModel { get; }
    public IMainWindowSurfacePlotViewModel SurfacePlotViewModel { get; }
    public IMainWindowStatusBarViewModel StatusBarViewModel { get; }

    public static string Title => "AnchorCalc";


    public override void WindowClosing()
    {
        base.WindowClosing();
        MenuViewModel.CloseAboutWindow();
    }


    private void OnMainWindowClosingRequested()
    {
        _windowManager.Close(this);
    }

    public void Dispose()
    {
        MenuViewModel.MainWindowClosingRequested -= OnMainWindowClosingRequested;
        StatusBarViewModel.Dispose();
        GC.SuppressFinalize(this);
    }
}