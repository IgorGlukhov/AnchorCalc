using System.Windows.Input;
using AnchorCalc.Domain.DevTools;
using AnchorCalc.Domain.Factories;
using AnchorCalc.ViewModels.Commands;
using AnchorCalc.ViewModels.DevTools;
using NLog;

namespace AnchorCalc.ViewModels.MainWindow;

public class DevToolsMenuViewModel : IDevToolsMenuViewModel
{
    private static readonly Logger Logger = LogManager.GetLogger(nameof(DevToolsMenuViewModel));
    private readonly IDevToolsStatusProvider _devToolsStatusProvider;
    private readonly IFactory<ILogViewerViewModel> _logViewerViewModelFactory;
    private readonly Command _openLogViewerCommand;

    public DevToolsMenuViewModel(IDevToolsStatusProvider devToolsStatusProvider,
        IFactory<ILogViewerViewModel> logViewerViewModelFactory,
        ILogEntryViewModelRepository logEntryViewModelRepository)
    {
        _devToolsStatusProvider = devToolsStatusProvider;
        _logViewerViewModelFactory = logViewerViewModelFactory;
        ClearLogsCommand = new Command(() => logEntryViewModelRepository.Clear());
        ThrowExceptionCommand = new Command(() => throw new Exception("Test"));
        _openLogViewerCommand = new Command(OpenLogViewer);
        WriteInfoLogCommand = new Command(() => Logger.Info("Testing info log"));
        WriteWarnLogCommand = new Command(() => Logger.Warn("Testing warn log"));
        WriteErrorLogCommand = new Command(() => Logger.Error("Testing error log"));
        WriteFatalLogCommand = new Command(() => Logger.Fatal("Testing fatal log"));
    }

    public ICommand ThrowExceptionCommand { get; }
    public ICommand OpenLogViewerCommand => _openLogViewerCommand;
    public ICommand WriteInfoLogCommand { get; }
    public ICommand WriteWarnLogCommand { get; }
    public ICommand WriteErrorLogCommand { get; }
    public ICommand WriteFatalLogCommand { get; }
    public ICommand ClearLogsCommand { get; }
    public event Action<IMainWindowContentViewModel>? ContentViewModelChanged;
    public bool IsVisible => _devToolsStatusProvider.IsEnabled;

    private void OpenLogViewer()
    {
        var logViewerViewModel = _logViewerViewModelFactory.Create();
        ContentViewModelChanged?.Invoke(logViewerViewModel);
    }
}