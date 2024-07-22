using System.Windows.Media.Media3D;

namespace AnchorCalc.ViewModels.Plotters;

public static class PlotFromArray
{
    public static Point3D[,] Plot(double[,] xData2DArray, double[,] yData2DArray, double[,] zData2DArray)
    {
        var n = zData2DArray.GetLength(0);
        var m = zData2DArray.GetLength(1);
        var newDataArray = new Point3D[n, m];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
            {
                var point = new Point3D(xData2DArray[i, j], yData2DArray[i, j], zData2DArray[i, j]);
                newDataArray[i, j] = point;
            }

        return newDataArray;
    }
}