using System.Windows.Media.Media3D;
using AnchorCalc.ViewModels.MainWindow;
using HelixToolkit.Wpf;

namespace AnchorCalc.Views.MainWindow;

public partial class MainWindow : IMainWindow
{
    public MainWindow(IMainWindowSurfacePlotViewModel mainWindowSurfacePlotViewModelViewModel,
        IMainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
        SurfacePlot.DataContext = mainWindowSurfacePlotViewModelViewModel;
        
    }
}