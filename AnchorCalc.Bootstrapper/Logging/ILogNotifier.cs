using AnchorCalc.Domain.Logging;

namespace AnchorCalc.Bootstrapper.Logging;

public interface ILogNotifier
{
    void Notify(LogArgs args);
}