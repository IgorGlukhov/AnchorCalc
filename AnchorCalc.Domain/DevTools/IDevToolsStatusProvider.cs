namespace AnchorCalc.Domain.DevTools;

public interface IDevToolsStatusProvider
{
    bool IsEnabled { get; }
}