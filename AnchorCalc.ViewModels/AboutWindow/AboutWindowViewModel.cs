using AnchorCalc.Domain.Settings;
using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.ViewModels.AboutWindow;

public class AboutWindowViewModel : WindowViewModel<IAboutWindowMementoWrapper>, IAboutWindowViewModel
{
    public AboutWindowViewModel(IAboutWindowMementoWrapper windowMementoWrapper) 
        : base(windowMementoWrapper)
    {
    }
}