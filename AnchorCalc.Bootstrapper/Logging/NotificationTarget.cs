using AnchorCalc.Domain.Logging;
using NLog;
using NLog.Targets;
using LogLevel = AnchorCalc.Domain.Logging.LogLevel;

namespace AnchorCalc.Bootstrapper.Logging;

internal class NotificationTarget : Target
{
    private readonly ILogNotifier _logNotifier;

    public NotificationTarget(ILogNotifier logNotifier)
    {
        _logNotifier = logNotifier;
    }

    protected override void Write(LogEventInfo logEvent)
    {
        var logArgs = new LogArgs
        {
            LogLevel = ToLocal(logEvent.Level),
            LoggerName = logEvent.LoggerName ?? "noname",
            Message = logEvent.FormattedMessage,
            TimeStamp = logEvent.TimeStamp,
            StackTrace = logEvent.Exception?.StackTrace
        };
        _logNotifier.Notify(logArgs);
    }

    private static LogLevel ToLocal(NLog.LogLevel logLevel)
    {
        return logLevel.Name switch
        {
            "Fatal" => LogLevel.Fatal,
            "Error" => LogLevel.Error,
            "Warn" => LogLevel.Warn,
            _ => LogLevel.Info
        };
    }
}