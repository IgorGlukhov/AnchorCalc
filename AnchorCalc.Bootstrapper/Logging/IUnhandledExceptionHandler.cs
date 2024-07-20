using System.Windows.Threading;

namespace AnchorCalc.Bootstrapper.Logging;

public interface IUnhandledExceptionHandler
{
    void Handle(DispatcherUnhandledExceptionEventArgs args);
}