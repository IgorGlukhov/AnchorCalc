using AnchorCalc.Domain.Version;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowStatusBarViewModel(IApplicationVersionProvider applicationVersionProvider)
    : ViewModel, IMainWindowStatusBarViewModel
{
    public string Version { get; } = $"Version {applicationVersionProvider.Version.ToString(3)}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}