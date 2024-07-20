using System.Windows.Threading;
using NLog;

namespace AnchorCalc.Bootstrapper.Logging;

public class UnhandledExceptionHandler : IUnhandledExceptionHandler
{
    private static readonly ILogger Logger = LogManager.GetLogger(nameof(UnhandledExceptionHandler));

    public void Handle(DispatcherUnhandledExceptionEventArgs args)
    {
        args.Handled = true;
        Logger.Error(args.Exception);
    }
}