using System.Windows.Input;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IMainWindowMenuViewModel : IDisposable
{
    ICommand CloseMainWindowCommand { get; }
    ICommand OpenAboutWindowCommand { get; }
    ICommand OpenAnchorCollectionCommand { get; }
    IDevToolsMenuViewModel DevToolsMenuViewModel { get; }
    public event Action? MainWindowClosingRequested;
    event Action<IMainWindowContentViewModel> ContentViewModelChanged;
    void CloseAboutWindow();
}