using AnchorCalc.Domain.Settings;
using AnchorCalc.Domain.Version;
using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.ViewModels.AboutWindow;

public class AboutWindowViewModel : WindowViewModel<IAboutWindowMementoWrapper>, IAboutWindowViewModel
{
    public AboutWindowViewModel(IAboutWindowMementoWrapper windowMementoWrapper,
        IApplicationVersionProvider applicationVersionProvider) 
        : base(windowMementoWrapper)
    {
        Version = $"Version {applicationVersionProvider.Version.ToString(3)}";
    }

    public string Version { get; }
}