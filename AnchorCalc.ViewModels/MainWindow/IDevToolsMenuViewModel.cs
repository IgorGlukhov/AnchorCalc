using System.Windows.Input;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IDevToolsMenuViewModel
{
    bool IsVisible { get; }
    ICommand ThrowExceptionCommand { get; }
    ICommand OpenLogViewerCommand { get; }
    ICommand WriteInfoLogCommand { get; }
    ICommand WriteWarnLogCommand { get; }
    ICommand WriteErrorLogCommand { get; }
    ICommand WriteFatalLogCommand { get; }
    ICommand ClearLogsCommand { get; }

    event Action<IMainWindowContentViewModel> ContentViewModelChanged;
}