using System.Windows.Input;
using AnchorCalc.Domain.Factories;
using AnchorCalc.ViewModels.AboutWindow;
using AnchorCalc.ViewModels.Anchors;
using AnchorCalc.ViewModels.Commands;
using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowMenuViewModel : IMainWindowMenuViewModel
{
    private readonly IFactory<IAboutWindowViewModel> _aboutWindowViewModelFactory;
    private readonly IFactory<IAnchorCollectionViewModel> _anchorCollectionViewModelFactory;
    private readonly Command _closeMainWindowCommand;
    private readonly Command _openAboutWindowCommand;
    private readonly AsyncCommand _openAnchorCollectionCommand;
    private readonly IWindowManager _windowManager;
    private IAboutWindowViewModel? _aboutWindowViewModel;

    public MainWindowMenuViewModel(IWindowManager windowManager,
        IFactory<IAboutWindowViewModel> aboutWindowViewModelFactory,
        IFactory<IDevToolsMenuViewModel> devToolsMenuViewModelFactory,
        IFactory<IAnchorCollectionViewModel> anchorCollectionViewModelFactory)
    {
        _closeMainWindowCommand = new Command(CloseMainWindow);
        _openAboutWindowCommand = new Command(OpenAboutWindow);
        _openAnchorCollectionCommand = new AsyncCommand(OpenAnchorCollectionAsync);
        _aboutWindowViewModelFactory = aboutWindowViewModelFactory;
        _anchorCollectionViewModelFactory = anchorCollectionViewModelFactory;
        DevToolsMenuViewModel = devToolsMenuViewModelFactory.Create();
        _windowManager = windowManager;
        DevToolsMenuViewModel.ContentViewModelChanged += OnContentViewModelChanged;
    }

    public ICommand CloseMainWindowCommand => _closeMainWindowCommand;
    public ICommand OpenAboutWindowCommand => _openAboutWindowCommand;

    public IDevToolsMenuViewModel DevToolsMenuViewModel { get; }

    public event Action? MainWindowClosingRequested;
    public event Action<IMainWindowContentViewModel>? ContentViewModelChanged;
    public ICommand OpenAnchorCollectionCommand => _openAnchorCollectionCommand;

    public void CloseAboutWindow()
    {
        if (_aboutWindowViewModel != null) _windowManager.Close(_aboutWindowViewModel);
    }

    private void OnContentViewModelChanged(IMainWindowContentViewModel contentViewModel)
    {
        ContentViewModelChanged?.Invoke(contentViewModel);
    }

    private async Task OpenAnchorCollectionAsync()
    {
        var anchorCollectionViewModel = _anchorCollectionViewModelFactory.Create();
        await anchorCollectionViewModel.InitializeAsync();
        ContentViewModelChanged?.Invoke(anchorCollectionViewModel);
    }

    private void CloseMainWindow()
    {
        MainWindowClosingRequested?.Invoke();
    }

    private void OpenAboutWindow()
    {
        if (_aboutWindowViewModel == null)
        {
            _aboutWindowViewModel = _aboutWindowViewModelFactory.Create();
            var aboutWindow = _windowManager.Show(_aboutWindowViewModel);
            aboutWindow.Closed += OnAboutWindowClosed;
        }
        else
        {
            _windowManager.Show(_aboutWindowViewModel);
        }
    }

    private void OnAboutWindowClosed(object? sender, EventArgs e)
    {
        if (sender is IWindow window)
        {
            window.Closed -= OnAboutWindowClosed;
            _aboutWindowViewModel = null;
        }
    }

    public void Dispose()
    {
        DevToolsMenuViewModel.ContentViewModelChanged += OnContentViewModelChanged;
        GC.SuppressFinalize(this);
    }
}