using AnchorCalc.Domain.Settings;
using AnchorCalc.Infrastructure.Common;

namespace AnchorCalc.Infrastructure.Settings;

internal class AboutWindowMementoWrapper(IPathService pathService)
    : WindowMementoWrapper<AboutWindowMemento>(pathService), IAboutWindowMementoWrapper
{
    protected override string MementoName => "AboutWindowMemento";
}