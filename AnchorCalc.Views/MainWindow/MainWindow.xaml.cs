using System.Windows.Media.Media3D;
using AnchorCalc.ViewModels.MainWindow;
using System.Windows.Controls;
using HelixToolkit.Wpf;
using System.Windows;
using AnchorCalc.ViewModels;

namespace AnchorCalc.Views.MainWindow;

public partial class MainWindow : IMainWindow
{
    public MainWindow(IMainWindowSurfacePlotViewModel mainWindowSurfacePlotViewModelViewModel,
        IMainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;

        SurfaceProperties.DataContext = mainWindowSurfacePlotViewModelViewModel;
        SurfacePlot.DataContext = mainWindowSurfacePlotViewModelViewModel;


    }
}