namespace AnchorCalc.Domain.Logging;

public class LogArgs
{
    public required DateTime TimeStamp { get; init; }
    public required LogLevel LogLevel { get; init; }
    public required string LoggerName { get; init; }
    public required string Message { get; init; }
    public string? StackTrace { get; init; }
}