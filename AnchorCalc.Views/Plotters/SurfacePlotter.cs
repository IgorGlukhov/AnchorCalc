using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace AnchorCalc.Views.Plotters;

public class SurfacePlotter : ModelVisual3D
{
    public static readonly DependencyProperty DataPointsProperty = DependencyProperty.Register(nameof(DataPoints),
        typeof(Point3D[,]), typeof(SurfacePlotter), new UIPropertyMetadata(null, OnModelChanged));

    // _modelContainer содержит все компоненты модели
    private readonly ModelVisual3D _modelContainer;

    // Конструктор SurfacePlotVisual3D объекта.
    public SurfacePlotter()
    {
        _modelContainer = new ModelVisual3D();
        var fakeModel3DGroup = new Model3DGroup();
        var fakeGeometryModel3D = new GeometryModel3D
        {
            Geometry = new MeshGeometry3D
            {
                Positions =
                [
                    new Point3D(200, 0, 0), new Point3D(0, 200, 0), new Point3D(-200, 0, 0), new Point3D(0, -200, 0)
                ]
            }
        };
        fakeModel3DGroup.Children.Add(fakeGeometryModel3D);
        _modelContainer.Content = fakeModel3DGroup;
        Children.Add(_modelContainer);
    }

    // Инициализация точек 3д поверхности, как 2д массив Point3D объектов.
    public Point3D[,] DataPoints
    {
        get => (Point3D[,])GetValue(DataPointsProperty);
        set => SetValue(DataPointsProperty, value);
    }


    // Инициализация кисти для отображения 3д поверхности.
    public static Brush SurfaceBrush => BrushHelper.CreateGradientBrush(Colors.Blue, Colors.Red);

    public double IntervalX { get; set; }
    public double IntervalY { get; set; }
    public double IntervalZ { get; set; }
    public double FontSize { get; set; }

    public double LineThickness { get; set; }

    // Этот метод вызывается каждый раз, когда любое свойство SurfacePlotter меняется.
    public static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SurfacePlotter)d).UpdateModel();
    }

    //Этот метод обновляет всю модель.
    private void UpdateModel()
    {
        Children.Clear();
        Children.Add(_modelContainer);
        Content = CreateModel();
    }

    // Метод который непосредственно создает SurfacePlot 3D. 
    private Model3DGroup CreateModel()
    {
        var newModelGroup = new Model3DGroup();
        var lineThickness = 0.5;
        double axesOffset = 3;
        var numberOfRows = DataPoints.GetUpperBound(0) + 1;
        var numberOfColumns = DataPoints.GetUpperBound(1) + 1;
        var minX = double.MaxValue;
        var maxX = double.MinValue;
        var minY = double.MaxValue;
        var maxY = double.MinValue;
        var minZ = double.MaxValue;
        var maxZ = double.MinValue;
        for (var i = 0; i < numberOfRows; i++)
        for (var j = 0; j < numberOfColumns; j++)
        {
            var x = DataPoints[i, j].X;
            var y = DataPoints[i, j].Y;
            var z = DataPoints[i, j].Z;
            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
            maxZ = Math.Max(maxZ, z);
            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            minZ = Math.Min(minZ, z);
        }

        if (maxX == minX) maxX += 0.01;
        if (maxZ == minZ) maxZ += 0.01;
        if (maxY == minY) maxY += 0.01;
        var numberOfXAxisTicks = 10;
        var numberOfYAxisTicks = 10;
        var numberOfZAxisTicks = 5;
        var xAxisInterval = (maxX - minX) / numberOfXAxisTicks;
        var yAxisInterval = (maxY - minY) / numberOfYAxisTicks;
        var zAxisInterval = (maxZ - minZ) / numberOfZAxisTicks;
        // Привязываем цвет текстур к значению по z
        var textureCoordinates = new Point[numberOfRows, numberOfColumns];
        for (var i = 0; i < numberOfRows; i++)
        for (var j = 0; j < numberOfColumns; j++)
        {
            var tc = (DataPoints[i, j].Z - minZ) / (maxZ - minZ);
            textureCoordinates[i, j] = new Point(tc, tc);
        }

        var surfaceModelBuilder = new MeshBuilder();
        surfaceModelBuilder.AddRectangularMesh(DataPoints, textureCoordinates);
        var surfaceModel = new GeometryModel3D(surfaceModelBuilder.ToMesh(),
            MaterialHelper.CreateMaterial(SurfaceBrush, SurfaceBrush, null, 1, 0));
        surfaceModel.BackMaterial = surfaceModel.Material;
        var surfaceMeshLinesBuilder = new MeshBuilder();
        var gridBuilder = new MeshBuilder();
        var axesLabelsModel = new ModelVisual3D();
        for (var x = minX; x <= maxX + 0.0001; x += xAxisInterval)
        {
            // Поверхность
            var surfacePath = new List<Point3D>();
            var i = (x - minX) / (maxX - minX) * (numberOfColumns - 1);
            for (var j = 0; j < numberOfColumns; j++) surfacePath.Add(DoBilinearInterpolation(DataPoints, i, j));
            surfaceMeshLinesBuilder.AddTube(surfacePath, lineThickness, 9, false);
            if (x != minX)
            {
                // Оси
                var label = new BillboardTextVisual3D
                {
                    Text = $"{x:F0}",
                    Position = new Point3D(x, minY - axesOffset, minZ - axesOffset)
                };
                axesLabelsModel.Children.Add(label);
            }

            // Сетка
            var gridPath = new List<Point3D>
            {
                new(x, minY, minZ),
                new(x, maxY, minZ),
                new(x, maxY, maxZ)
            };
            gridBuilder.AddTube(gridPath, lineThickness, 9, false);
        }

        for (var y = minY; y < maxY + 0.0001; y += yAxisInterval)
        {
            // Поверхность
            var path = new List<Point3D>();
            var j = (y - minY) / (maxY - minY) * (numberOfRows - 1);
            for (var i = 0; i < numberOfRows; i++) path.Add(DoBilinearInterpolation(DataPoints, i, j));
            surfaceMeshLinesBuilder.AddTube(path, lineThickness, 9, false);
            // Оси
            var label = new BillboardTextVisual3D
            {
                Text = $"{y:F0}",
                Position = new Point3D(minX - axesOffset, y, minZ - axesOffset)
            };
            axesLabelsModel.Children.Add(label);
            // Сетка
            var gridPath = new List<Point3D>
            {
                new(minX, y, minZ),
                new(maxX, y, minZ),
                new(maxX, y, maxZ)
            };
            gridBuilder.AddTube(gridPath, lineThickness, 9, false);
        }

        for (var z = minZ; z <= maxZ + 0.0001; z += zAxisInterval)
        {
            // Сетка
            var path = new List<Point3D>
            {
                new(minX, maxY, z),
                new(maxX, maxY, z),
                new(maxX, minY, z)
            };
            gridBuilder.AddTube(path, lineThickness, 9, false);
            // Оси
            var label = new BillboardTextVisual3D
            {
                Text = $"{z * 0.1:F1}",
                Position = new Point3D(maxX + 5 * axesOffset, maxY + 5 * axesOffset, z)
            };
            axesLabelsModel.Children.Add(label);
        }

        // Наименования осей
        var xLabel = new BillboardTextVisual3D
        {
            Text = "X, мм",
            Position = new Point3D((maxX - minX) / 2, minY - 3 * axesOffset, minZ - 5 * axesOffset)
        };
        axesLabelsModel.Children.Add(xLabel);
        var yLabel = new BillboardTextVisual3D
        {
            Text = "Y, мм",
            Position = new Point3D(minX - 3 * axesOffset, (maxY - minY) / 2, minZ - 5 * axesOffset)
        };
        axesLabelsModel.Children.Add(yLabel);
        var zLabel = new BillboardTextVisual3D
        {
            Text = "Напряжения, МПа",
            Position = new Point3D(maxX + 5 * axesOffset, maxY + 5 * axesOffset, zAxisInterval * 2)
        };
        axesLabelsModel.Children.Add(zLabel);
        var surfaceMeshLinesModel = new GeometryModel3D(surfaceMeshLinesBuilder.ToMesh(), Materials.Black);
        var gridModel = new GeometryModel3D(gridBuilder.ToMesh(), Materials.Black);
        Children.Add(axesLabelsModel);
        newModelGroup.Children.Add(surfaceModel);
        newModelGroup.Children.Add(surfaceMeshLinesModel);
        newModelGroup.Children.Add(gridModel);
        newModelGroup.Children.Add(new AmbientLight(Colors.Gray));
        return newModelGroup;
    }

    // Метод билинейной интреполяции, делающий линии графика более плавными.
    private static Point3D DoBilinearInterpolation(Point3D[,] points, double i, double j)
    {
        var n = points.GetUpperBound(0);
        var m = points.GetUpperBound(1);
        var i0 = (int)i;
        var j0 = (int)j;
        if (i0 + 1 >= n) i0 = n - 2;
        if (j0 + 1 >= m) j0 = m - 2;
        if (i < 0) i = 0;
        if (j < 0) j = 0;
        var u = i - i0;
        var v = j - j0;
        var v00 = points[i0, j0].ToVector3D();
        var v01 = points[i0, j0 + 1].ToVector3D();
        var v10 = points[i0 + 1, j0].ToVector3D();
        var v11 = points[i0 + 1, j0 + 1].ToVector3D();
        var v0 = v00 * (1 - u) + v10 * u;
        var v1 = v01 * (1 - u) + v11 * u;
        return (v0 * (1 - v) + v1 * v).ToPoint3D();
    }
}