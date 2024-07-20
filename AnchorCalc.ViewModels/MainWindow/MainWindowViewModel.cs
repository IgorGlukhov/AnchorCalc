using AnchorCalc.Domain.Factories;
using AnchorCalc.Domain.Settings;
using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowViewModel : WindowViewModel<IMainWindowMementoWrapper>, IMainWindowViewModel
{
    private readonly IWindowManager _windowManager;
    private IMainWindowContentViewModel? _contentViewModel;


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
        MenuViewModel.ContentViewModelChanged += OnContentViewModelChanged;
        MenuViewModel.MainWindowClosingRequested += OnMainWindowClosingRequested;
    }


    public IMainWindowMenuViewModel MenuViewModel { get; }
    public IMainWindowSurfacePlotViewModel SurfacePlotViewModel { get; }
    public IMainWindowStatusBarViewModel StatusBarViewModel { get; }

    public IMainWindowContentViewModel? ContentViewModel
    {
        get => _contentViewModel;
        private set
        {
            if (_contentViewModel is IDisposable disposableViewModel) disposableViewModel.Dispose();
            ;
            _contentViewModel = value;
            OnPropertyChanged();
        }
    }

    public static string Title => "AnchorCalc";


    public override void WindowClosing()
    {
        base.WindowClosing();
        MenuViewModel.CloseAboutWindow();
    }

    private void OnContentViewModelChanged(IMainWindowContentViewModel contentViewModel)
    {
        ContentViewModel = contentViewModel;
    }


    private void OnMainWindowClosingRequested()
    {
        _windowManager.Close(this);
    }

    public void Dispose()
    {
        MenuViewModel.MainWindowClosingRequested -= OnMainWindowClosingRequested;
        StatusBarViewModel.Dispose();
        ContentViewModel = null;
        MenuViewModel.Dispose();
        GC.SuppressFinalize(this);
    }
}