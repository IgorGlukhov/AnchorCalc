using AnchorCalc.Domain.Settings;
using AnchorCalc.Infrastructure.Common;

namespace AnchorCalc.Infrastructure.Settings;

internal class MainWindowMementoWrapper : WindowMementoWrapper<MainWindowMemento>, IMainWindowMementoWrapper
{
    public MainWindowMementoWrapper(IPathService pathService)
        : base(pathService)
    {
    }

    protected override string MementoName => "MainWindowMemento";
}