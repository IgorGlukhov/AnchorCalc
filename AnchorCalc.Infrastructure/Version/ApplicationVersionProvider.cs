using AnchorCalc.Domain.Version;

namespace AnchorCalc.Infrastructure.Version;

public class ApplicationVersionProvider : IApplicationVersionProvider
{
    public System.Version Version { get; } = new(1, 0, 0);
}