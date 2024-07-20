using AnchorCalc.Domain.DevTools;

namespace AnchorCalc.Infrastructure.DevTools;

public class DevToolsStatusProvider : IDevToolsStatusProvider
{
    public bool IsEnabled =>
#if DEBUG
        true;
#else
        false;
#endif
}