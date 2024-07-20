namespace AnchorCalc.Domain.Logging;

public interface ILogSubscriber
{
    event Action<LogArgs> LogAdded;
}