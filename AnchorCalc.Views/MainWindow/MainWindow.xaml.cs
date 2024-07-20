using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.Views.MainWindow;

public partial class MainWindow : IMainWindow
{
    public MainWindow(
        IMainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }
}