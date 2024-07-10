using AnchorCalc.Domain.Settings;
using AnchorCalc.Infrastructure.Common;

namespace AnchorCalc.Infrastructure.Settings;

internal class MainWindowMementoWrapper(IPathService pathService)
    : WindowMementoWrapper<MainWindowMemento>(pathService), IMainWindowMementoWrapper
{
    protected override string MementoName => "MainWindowMemento";
}