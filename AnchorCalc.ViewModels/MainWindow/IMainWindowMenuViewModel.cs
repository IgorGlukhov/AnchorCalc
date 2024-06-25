using System.Windows.Input;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IMainWindowMenuViewModel
{
    ICommand CloseMainWindowCommand { get; }
    ICommand OpenAboutWindowCommand { get; }
    public event Action? MainWindowClosingRequested;
    void CloseAboutWindow();
}