using AnchorCalc.Domain.Settings;
using AnchorCalc.Infrastructure.Common;

namespace AnchorCalc.Infrastructure.Settings;

internal class AboutWindowMementoWrapper : WindowMementoWrapper<AboutWindowMemento>, IAboutWindowMementoWrapper
{
    public AboutWindowMementoWrapper(IPathService pathService) : base(pathService)
    {
    }

    protected override string MementoName => "AboutWindowMemento";
}
