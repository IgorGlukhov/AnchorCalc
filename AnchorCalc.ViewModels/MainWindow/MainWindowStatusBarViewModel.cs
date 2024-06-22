using AnchorCalc.Domain.Version;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowStatusBarViewModel : ViewModel, IMainWindowStatusBarViewModel
{
    public MainWindowStatusBarViewModel(IApplicationVersionProvider applicationVersionProvider)
    {
        Version = $"Version {applicationVersionProvider.Version.ToString(3)}";
    }

    public string Version { get; }

    public void Dispose()
    {
    }
}