using System.Windows.Input;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Domain.Settings;
using AnchorCalc.Domain.Version;
using AnchorCalc.ViewModels.AboutWindow;
using AnchorCalc.ViewModels.Commands;
using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowViewModel : WindowViewModel<IMainWindowMementoWrapper>, IMainWindowViewModel
{
    private readonly IFactory<IAboutWindowViewModel> _aboutWindowViewModelFactory;
    private IAboutWindowViewModel? _aboutWindowViewModel;
    private readonly Command _closeMainWindowCommand;
    private readonly IWindowManager _windowManager;
    private readonly Command _openAboutWindowCommand;

    public MainWindowViewModel(
        IMainWindowMementoWrapper mainWindowMementoWrapper,
        IWindowManager windowManager,
        IFactory<IAboutWindowViewModel> aboutWindowViewModelFactory,
        IApplicationVersionProvider applicationVersionProvider)
        : base(mainWindowMementoWrapper)
    {
        _windowManager = windowManager;
        _aboutWindowViewModelFactory = aboutWindowViewModelFactory;
        _closeMainWindowCommand = new Command(CloseMainWindow);
        _openAboutWindowCommand = new Command(OpenAboutWindow);
        Version = $"Version {applicationVersionProvider.Version.ToString(3)}";
    }

    public string Title => "AnchorCalc";
    public string Version { get; }
    public ICommand CloseMainWindowCommand => _closeMainWindowCommand;
    public ICommand OpenAboutWindowCommand => _openAboutWindowCommand;

    private void OpenAboutWindow()
    {
        if (_aboutWindowViewModel==null)
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

    private void CloseMainWindow()
    {
        _windowManager.Close(this);
    }

    public override void WindowClosing()
    {
        base.WindowClosing();
        _windowManager.Close(_aboutWindowViewModel);
    }

    public void Dispose()
    {
    }
}