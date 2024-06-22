using System.Windows.Input;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IMainWindowMenuViewModel
{
    public event Action? MainWindowClosingRequested;
    ICommand CloseMainWindowCommand { get; }
    ICommand OpenAboutWindowCommand { get; }
    void CloseAboutWindow();
}