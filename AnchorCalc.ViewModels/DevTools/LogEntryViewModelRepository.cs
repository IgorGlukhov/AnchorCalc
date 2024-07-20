using AnchorCalc.Domain.Collections;
using AnchorCalc.Domain.Logging;

namespace AnchorCalc.ViewModels.DevTools;

public class LogEntryViewModelRepository : ILogEntryViewModelRepository, IDisposable
{
    private readonly IRotatableCollection<LogEntryViewModel> _items;
    private readonly ILogSubscriber _logSubscriber;

    public LogEntryViewModelRepository(IRotatableCollectionFactory rotatableCollectionFactory,
        ILogSubscriber logSubscriber)
    {
        _logSubscriber = logSubscriber;
        _items = rotatableCollectionFactory.Create<LogEntryViewModel>(1000);
        _logSubscriber.LogAdded += OnLogAdded;
    }

    public IRotatableReadonlyCollection<LogEntryViewModel> Items => _items;

    public void Clear()
    {
        _items.Clear();
    }

    private void OnLogAdded(LogArgs args)
    {
        var logEntryViewModel = new LogEntryViewModel
        {
            TimeStamp = args.TimeStamp,
            TimeStampValue = args.TimeStamp.ToString("F"),
            Level = args.LogLevel,
            Message = args.Message,
            LoggerName = args.LoggerName,
            StackTrace = args.StackTrace
        };
        _items.Add(logEntryViewModel);
    }

    public void Dispose()
    {
        _logSubscriber.LogAdded -= OnLogAdded;
        GC.SuppressFinalize(this);
    }
}