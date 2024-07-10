using AnchorCalc.Domain.Settings;
using AnchorCalc.Domain.Version;
using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.ViewModels.AboutWindow;

public class AboutWindowViewModel(
    IAboutWindowMementoWrapper windowMementoWrapper,
    IApplicationVersionProvider applicationVersionProvider)
    : WindowViewModel<IAboutWindowMementoWrapper>(windowMementoWrapper), IAboutWindowViewModel
{
    public string Version { get; } = $"Version {applicationVersionProvider.Version.ToString(3)}";
}