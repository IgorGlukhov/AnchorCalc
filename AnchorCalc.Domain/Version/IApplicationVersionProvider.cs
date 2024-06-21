namespace AnchorCalc.Domain.Version;

public interface IApplicationVersionProvider
{
    System.Version Version { get; }
}