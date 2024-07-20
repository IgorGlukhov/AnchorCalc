using AnchorCalc.Domain.Logging;

namespace AnchorCalc.ViewModels.DevTools;

public class LogEntryViewModel
{
    public required DateTime TimeStamp { get; init; }
    public required string TimeStampValue { get; init; }
    public required LogLevel Level { get; init; }
    public required string Message { get; init; }
    public required string LoggerName { get; init; }
    public string? StackTrace { get; init; }
    public bool IsStackTraceVisible => StackTrace is not null;
}