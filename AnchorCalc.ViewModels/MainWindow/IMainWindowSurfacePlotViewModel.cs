using System.Windows.Media.Media3D;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IMainWindowSurfacePlotViewModel : IDisposable
{
    public Point3D[,] DataPoints { get; }
    public string Title { get; set; }

    public string XAxisLabel { get; set; }

    public string YAxisLabel { get; set; }

    public string ZAxisLabel { get; set; }

    public bool ShowMiniCoordinates { get; }

    public double Force { get; set; }

    public double MomentX { get; set; }

    public double MomentY { get; set; }

    public double SealingDepth { get; set; }

    public double Diameter { get; set; }

    public double ConcreteResistance { get; set; }

    public double BasePlateWidth { get; set; }

    public double BasePlateLength { get; set; }

    public int Triangulation { get; set; }

    public int AnchorCount { get; set; }
}