using AnchorCalc.Domain.Logging;

namespace AnchorCalc.Bootstrapper.Logging;

public class LogNotifier : ILogNotifier, ILogSubscriber
{
    public void Notify(LogArgs args)
    {
        LogAdded?.Invoke(args);
    }

    public event Action<LogArgs>? LogAdded;
}